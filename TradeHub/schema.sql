USE [trading]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[trade]') AND type in (N'U'))
   DROP TABLE [brady_trading].[trade]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[nexthigh]') AND type in (N'U'))
   DROP TABLE [brady_trading].[nexthigh]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[schemaversion]') AND type in (N'U'))
   DROP TABLE [brady_trading].[schemaversion]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[prc_checkversion]') AND type in (N'P'))
   DROP PROCEDURE [brady_trading].[prc_checkversion]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[prc_updateversion]') AND type in (N'P'))
   DROP PROCEDURE [brady_trading].[prc_updateversion]
GO

IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'brady_trading')
   DROP SCHEMA [brady_trading] 
GO

CREATE SCHEMA [brady_trading] AUTHORIZATION [dbo]
GO

CREATE TABLE [brady_trading].[schemaversion](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[version] [nvarchar](50) NOT NULL,
	[major] [int] NOT NULL,
	[minor] [int] NOT NULL,
	[patch] [int] NOT NULL,
	[compile] [int] NOT NULL,
	[timestamp] AS GetDate()
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[schemaversion]
   ADD CONSTRAINT [pk_dbversion]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

CREATE PROCEDURE brady_trading.prc_checkversion
    ( @OldMajor    INT
    , @OldMinor    INT
    , @OldPatch    INT
    , @OldCompile  INT
    , @NewMajor    INT
    , @NewMinor    INT
    , @NewPatch    INT
    , @NewCompile  INT
   )
AS
BEGIN
   --SET NOCOUNT ON
   
   IF NOT EXISTS (SELECT version FROM brady_trading.schemaversion WHERE major = @OldMajor AND minor = @OldMinor AND patch = @OldPatch AND compile = @OldCompile)
   BEGIN 
      RAISERROR('Cannot upgrade to %d.%d.%d.%d before database is upgraded to %d.%d.%d.%d', 16, 1, @NewMajor, @NewMinor, @NewPatch, @NewCompile, @OldMajor, @OldMinor, @OldPatch, @OldCompile)
   END 
   
   IF EXISTS (SELECT version FROM brady_trading.schemaversion WHERE major = @NewMajor AND minor = @NewMinor AND patch = @NewPatch AND compile = @NewCompile)
   BEGIN
      RAISERROR('Cannot upgrade to %d.%d.%d.%d as database is already at this version or greater', 16, 1, @NewMajor, @NewMinor, @NewPatch, @NewCompile)
   END 
END
GO

CREATE PROCEDURE brady_trading.prc_updateversion
    ( @NewMajor    INT
    , @NewMinor    INT
    , @NewPatch    INT
    , @NewCompile  INT
   )
AS
BEGIN
   DECLARE @version NVARCHAR(43)
   SET @version = (CAST(@NewMajor AS NVARCHAR(10)) + '.' + CAST(@NewMinor AS NVARCHAR(10)) + '.' + CAST(@NewPatch AS NVARCHAR(10)) + '.' + CAST(@NewCompile AS NVARCHAR(10)))
   
   INSERT INTO brady_trading.schemaversion (version, major, minor, patch, compile)
      VALUES (@version, @NewMajor, @NewMinor, @NewPatch, @NewCompile)
END
GO

CREATE TABLE [brady_trading].[nexthigh]
(
	[id] [int]    IDENTITY(1,1) NOT NULL,
	[nexthigh]    [bigint] NOT NULL,
	[entityname]  [nvarchar](30) NOT NULL
) ON [PRIMARY]
GO
 
ALTER TABLE [brady_trading].[nexthigh]
   ADD CONSTRAINT [pk_nexthigh] 
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[nexthigh]
   ADD CONSTRAINT [uk_nexthigh1]
      UNIQUE (entityname)
GO

CREATE TABLE [brady_trading].[trade](
	[id] 			[bigint] 	   NOT NULL,
	[systemcode] 	[nvarchar](64) NOT NULL,
    [systemid]    	[nvarchar](64) NOT NULL,
	[tradetype]    	[nvarchar](64) NOT NULL,
	[effectivedate] [datetime]     NOT NULL,
	[contractcode]  [nvarchar](64),
	[marketcode]  	[nvarchar](64),
	[islive]  		[int],
	[entity]  		[nvarchar](64),
	[counterparty]  [nvarchar](64),
	[portfolio]  	[nvarchar](64),
	[tradedby]  	[nvarchar](64),
	[tradedon]  	[datetime],
	[enteredby]  	[nvarchar](64),
	[enteredon]  	[datetime],
	[updatedby]  	[nvarchar](64),
	[updatedon]  	[datetime]
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[trade]
   ADD CONSTRAINT [pk_trade]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[trade]
   ADD CONSTRAINT [uk_trade1]
      UNIQUE ([systemcode], [systemid])
GO

GRANT INSERT ON [brady_trading].[trade] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[trade] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[trade] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[trade] TO [brady_trading_select]
GO

CREATE TABLE [brady_trading].[commoditytrade](
	[id] 				[bigint] 		NOT NULL,
	[bs]				[nvarchar2](4),
	[term]				[nvarchar2](64),
	[deliverymonth]		[datetime],
	[deliverydate]		[datetime],
	[lots]				[float],
	[commodityamount]   [float],
	[commodityunits]	[nvarchar2](64),
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commoditytrade]
   ADD CONSTRAINT [pk_commoditytrade]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commoditytrade]
   ADD CONSTRAINT [fk_commoditytrade_trade]
      FOREIGN KEY ([id])
         REFERENCES [brady_trading].[trade]([id])
            ON DELETE CASCADE
GO

GRANT INSERT ON [brady_trading].[commoditytrade] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[commoditytrade] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[commoditytrade] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[commoditytrade] TO [brady_trading_select]
GO

CREATE TABLE [brady_trading].[commodityfuture](
	[id] 				[bigint] 		NOT NULL,
	[currencyamount]    [float],
	[baseprice]   	    [float],
	[price]   			[float],
	[spread]			[float],
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityfuture]
   ADD CONSTRAINT [pk_commodityfuture]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityfuture]
   ADD CONSTRAINT [fk_commodityfuture_commoditytrade]
      FOREIGN KEY ([id])
         REFERENCES [brady_trading].[commoditytrade]([id])
            ON DELETE CASCADE
GO

GRANT INSERT ON [brady_trading].[commodityfuture] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[commodityfuture] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[commodityfuture] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[commodityfuture] TO [brady_trading_select]
GO

CREATE TABLE [brady_trading].[commodityforward](
	[id] 				[bigint] 		NOT NULL,
	[currencyamount]    [float],
	[baseprice]   	    [float],
	[price]   			[float],
	[spread]			[float],
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityforward]
   ADD CONSTRAINT [pk_commodityforward]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityforward]
   ADD CONSTRAINT [fk_commodityforward_commoditytrade]
      FOREIGN KEY ([id])
         REFERENCES [brady_trading].[commoditytrade]([id])
            ON DELETE CASCADE
GO

GRANT INSERT ON [brady_trading].[commodityforward] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[commodityforward] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[commodityforward] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[commodityforward] TO [brady_trading_select]
GO

CREATE TABLE [brady_trading].[averagedetails](
	[id] 			[bigint] 	   NOT NULL,
	[averagetype]   [nvarchar](64) NOT NULL,
	[isfixedprice] 	[int],
	[fixedprice] 	[float]
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[averagedetails]
   ADD CONSTRAINT [pk_averagedetails]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

GRANT INSERT ON [brady_trading].[averagedetails] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[averagedetails] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[averagedetails] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[averagedetails] TO [brady_trading_select]
GO

CREATE TABLE [brady_trading].[vanillaaverage](
	[id] 					[bigint] 		NOT NULL,
	[fixingindex]	    	[nvarchar2](64),
	[startdate]		    	[datetime],
	[enddate]		    	[datetime],
	[additivepremium]      	[float],
	[additivepremiumunits]	[nvarchar2](64),
	[percentagepremium] 	[float]
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[vanillaaverage]
   ADD CONSTRAINT [pk_vanillaaverage]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[vanillaaverage]
   ADD CONSTRAINT [fk_vanillaaverage_averagedetails]
      FOREIGN KEY ([id])
         REFERENCES [brady_trading].[averagedetails]([id])
            ON DELETE CASCADE
GO

GRANT INSERT ON [brady_trading].[vanillaaverage] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[vanillaaverage] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[vanillaaverage] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[vanillaaverage] TO [brady_trading_select]
GO

CREATE TABLE [brady_trading].[optiondetails](
	[id] 			[bigint] 	   NOT NULL,
	[optiontype]    [nvarchar](64) NOT NULL,
	[optionstatus]  [nvarchar](64)
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[optiondetails]
   ADD CONSTRAINT [pk_optiondetails]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

GRANT INSERT ON [brady_trading].[optiondetails] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[optiondetails] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[optiondetails] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[optiondetails] TO [brady_trading_select]
GO

CREATE TABLE [brady_trading].[vanillaoption](
	[id] 					[bigint] 		NOT NULL,
	[cp]	    			[nvarchar2](4),
	[currencyamount]		[float],
	[model]	    			[nvarchar2](64),
	[strikeprice]			[float],
	[expirymonth]		    [datetime],
	[expirydate]		    [datetime],
	[premiumdate]		    [datetime],
	[premiumcurrency]	   	[nvarchar2](64),
	[premiumrate]			[float],
	[premiumamount] 		[float]
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[vanillaoption]
   ADD CONSTRAINT [pk_vanillaoption]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[vanillaoption]
   ADD CONSTRAINT [fk_vanillaoption_optiondetails]
      FOREIGN KEY ([id])
         REFERENCES [brady_trading].[vanillaoption]([id])
            ON DELETE CASCADE
GO

GRANT INSERT ON [brady_trading].[vanillaoption] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[vanillaoption] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[vanillaoption] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[vanillaoption] TO [brady_trading_select]
GO

CREATE TABLE [brady_trading].[commodityaverage](
	[id] 				[bigint] NOT NULL,
	[averagedetailsid]  [bigint] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityaverage]
   ADD CONSTRAINT [pk_commodityaverage]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityaverage]
   ADD CONSTRAINT [fk_commodityaverage_commoditytrade]
      FOREIGN KEY ([id])
         REFERENCES [brady_trading].[commodityaverage]([id])
            ON DELETE CASCADE
GO

GRANT INSERT ON [brady_trading].[commodityaverage] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[commodityaverage] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[commodityaverage] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[commodityaverage] TO [brady_trading_select]
GO

CREATE TABLE [brady_trading].[commodityaverageswap](
	[id] 					[bigint] NOT NULL,
	[averagedetailsid]  	[bigint] NOT NULL,
	[strikeprice]			[float],
	[strikecurrencyamount] 	[float]
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityaverageswap]
   ADD CONSTRAINT [pk_commodityaverageswap]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityaverageswap]
   ADD CONSTRAINT [fk_commodityaverageswap_commoditytrade]
      FOREIGN KEY ([id])
         REFERENCES [brady_trading].[commodityaverageswap]([id])
            ON DELETE CASCADE
GO

GRANT INSERT ON [brady_trading].[commodityaverageswap] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[commodityaverageswap] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[commodityaverageswap] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[commodityaverageswap] TO [brady_trading_select]
GO

CREATE TABLE [brady_trading].[commodityoption](
	[id] 				[bigint] NOT NULL,
	[optiondetailsid]   [bigint] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityoption]
   ADD CONSTRAINT [pk_commodityoption]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityoption]
   ADD CONSTRAINT [fk_commodityoption_commoditytrade]
      FOREIGN KEY ([id])
         REFERENCES [brady_trading].[commodityoption]([id])
            ON DELETE CASCADE
GO

GRANT INSERT ON [brady_trading].[commodityoption] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[commodityoption] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[commodityoption] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[commodityoption] TO [brady_trading_select]
GO

CREATE TABLE [brady_trading].[commoditytapo](
	[id] 				[bigint] NOT NULL,
	[averagedetailsid]  [bigint] NOT NULL,
	[optiondetailsid]   [bigint] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commoditytapo]
   ADD CONSTRAINT [pk_commoditytapo]
      PRIMARY KEY CLUSTERED ( [id] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commoditytapo]
   ADD CONSTRAINT [fk_commoditytapo_commoditytrade]
      FOREIGN KEY ([id])
         REFERENCES [brady_trading].[commoditytapo]([id])
            ON DELETE CASCADE
GO

GRANT INSERT ON [brady_trading].[commoditytapo] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[commoditytapo] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[commoditytapo] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[commoditytapo] TO [brady_trading_select]
GO

[brady_trading].[prc_updateversion] 2016, 2, 0, 1
GO

SET NOEXEC OFF