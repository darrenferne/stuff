USE [trading]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[commoditycarry]') AND type in (N'U'))
   DROP TABLE [brady_trading].[commoditycarry]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[commoditytapo]') AND type in (N'U'))
   DROP TABLE [brady_trading].[commoditytapo]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[commodityoption]') AND type in (N'U'))
   DROP TABLE [brady_trading].[commodityoption]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[commodityaverageswap]') AND type in (N'U'))
   DROP TABLE [brady_trading].[commodityaverageswap]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[commodityaverage]') AND type in (N'U'))
   DROP TABLE [brady_trading].[commodityaverage]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[commodityforward]') AND type in (N'U'))
   DROP TABLE [brady_trading].[commodityforward]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[commodityfuture]') AND type in (N'U'))
   DROP TABLE [brady_trading].[commodityfuture]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[commoditytrade]') AND type in (N'U'))
   DROP TABLE [brady_trading].[commoditytrade]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[trade]') AND type in (N'U'))
   DROP TABLE [brady_trading].[trade]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[nexthigh]') AND type in (N'U'))
   DROP TABLE [brady_trading].[nexthigh]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[vanillaaveragedetails]') AND type in (N'U'))
   DROP TABLE [brady_trading].[vanillaaveragedetails]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[averagedetails]') AND type in (N'U'))
   DROP TABLE [brady_trading].[averagedetails]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[vanillaoptiondetails]') AND type in (N'U'))
   DROP TABLE [brady_trading].[vanillaoptiondetails]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[brady_trading].[optiondetails]') AND type in (N'U'))
   DROP TABLE [brady_trading].[optiondetails]
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

DECLARE @RoleName sysname
set @RoleName = N'brady_trading_insert'
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = @RoleName AND type = 'R')
Begin
	DECLARE @RoleMemberName sysname
	DECLARE Member_Cursor CURSOR FOR
	select [name]
	from sys.database_principals 
	where principal_id in ( 
		select member_principal_id 
		from sys.database_role_members 
		where role_principal_id in (
			select principal_id
			FROM sys.database_principals where [name] = @RoleName  AND type = 'R' ))

	OPEN Member_Cursor;

	FETCH NEXT FROM Member_Cursor
	into @RoleMemberName

	WHILE @@FETCH_STATUS = 0
	BEGIN

		exec sp_droprolemember @rolename=@RoleName, @membername= @RoleMemberName

		FETCH NEXT FROM Member_Cursor
		into @RoleMemberName
	END;

	CLOSE Member_Cursor;
	DEALLOCATE Member_Cursor;
End

GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'brady_trading_insert' AND type = 'R')
	DROP ROLE [brady_trading_insert]
GO

CREATE ROLE [brady_trading_insert] AUTHORIZATION [dbo]
GO

DECLARE @RoleName sysname
set @RoleName = N'brady_trading_update'
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = @RoleName AND type = 'R')
Begin
	DECLARE @RoleMemberName sysname
	DECLARE Member_Cursor CURSOR FOR
	select [name]
	from sys.database_principals 
	where principal_id in ( 
		select member_principal_id 
		from sys.database_role_members 
		where role_principal_id in (
			select principal_id
			FROM sys.database_principals where [name] = @RoleName  AND type = 'R' ))

	OPEN Member_Cursor;

	FETCH NEXT FROM Member_Cursor
	into @RoleMemberName

	WHILE @@FETCH_STATUS = 0
	BEGIN

		exec sp_droprolemember @rolename=@RoleName, @membername= @RoleMemberName

		FETCH NEXT FROM Member_Cursor
		into @RoleMemberName
	END;

	CLOSE Member_Cursor;
	DEALLOCATE Member_Cursor;
End

GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'brady_trading_update' AND type = 'R')
	DROP ROLE [brady_trading_update]
GO

CREATE ROLE [brady_trading_update] AUTHORIZATION [dbo]
GO

DECLARE @RoleName sysname
set @RoleName = N'brady_trading_delete'
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = @RoleName AND type = 'R')
Begin
	DECLARE @RoleMemberName sysname
	DECLARE Member_Cursor CURSOR FOR
	select [name]
	from sys.database_principals 
	where principal_id in ( 
		select member_principal_id 
		from sys.database_role_members 
		where role_principal_id in (
			select principal_id
			FROM sys.database_principals where [name] = @RoleName  AND type = 'R' ))

	OPEN Member_Cursor;

	FETCH NEXT FROM Member_Cursor
	into @RoleMemberName

	WHILE @@FETCH_STATUS = 0
	BEGIN

		exec sp_droprolemember @rolename=@RoleName, @membername= @RoleMemberName

		FETCH NEXT FROM Member_Cursor
		into @RoleMemberName
	END;

	CLOSE Member_Cursor;
	DEALLOCATE Member_Cursor;
End

GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'brady_trading_delete' AND type = 'R')
	DROP ROLE [brady_trading_delete]
GO

CREATE ROLE [brady_trading_delete] AUTHORIZATION [dbo]
GO

DECLARE @RoleName sysname
set @RoleName = N'brady_trading_select'
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = @RoleName AND type = 'R')
Begin
	DECLARE @RoleMemberName sysname
	DECLARE Member_Cursor CURSOR FOR
	select [name]
	from sys.database_principals 
	where principal_id in ( 
		select member_principal_id 
		from sys.database_role_members 
		where role_principal_id in (
			select principal_id
			FROM sys.database_principals where [name] = @RoleName  AND type = 'R' ))

	OPEN Member_Cursor;

	FETCH NEXT FROM Member_Cursor
	into @RoleMemberName

	WHILE @@FETCH_STATUS = 0
	BEGIN

		exec sp_droprolemember @rolename=@RoleName, @membername= @RoleMemberName

		FETCH NEXT FROM Member_Cursor
		into @RoleMemberName
	END;

	CLOSE Member_Cursor;
	DEALLOCATE Member_Cursor;
End

GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'brady_trading_select' AND type = 'R')
	DROP ROLE [brady_trading_select]
GO

CREATE ROLE [brady_trading_select] AUTHORIZATION [dbo]
GO

DECLARE @RoleName sysname
set @RoleName = N'brady_trading_execute'
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = @RoleName AND type = 'R')
Begin
	DECLARE @RoleMemberName sysname
	DECLARE Member_Cursor CURSOR FOR
	select [name]
	from sys.database_principals 
	where principal_id in ( 
		select member_principal_id 
		from sys.database_role_members 
		where role_principal_id in (
			select principal_id
			FROM sys.database_principals where [name] = @RoleName  AND type = 'R' ))

	OPEN Member_Cursor;

	FETCH NEXT FROM Member_Cursor
	into @RoleMemberName

	WHILE @@FETCH_STATUS = 0
	BEGIN

		exec sp_droprolemember @rolename=@RoleName, @membername= @RoleMemberName

		FETCH NEXT FROM Member_Cursor
		into @RoleMemberName
	END;

	CLOSE Member_Cursor;
	DEALLOCATE Member_Cursor;
End

GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'brady_trading_execute' AND type = 'R')
	DROP ROLE [brady_trading_execute]
GO

CREATE ROLE [brady_trading_execute] AUTHORIZATION [dbo]
GO

DECLARE @RoleName sysname
set @RoleName = N'brady_trading_user'
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = @RoleName AND type = 'R')
Begin
	DECLARE @RoleMemberName sysname
	DECLARE Member_Cursor CURSOR FOR
	select [name]
	from sys.database_principals 
	where principal_id in ( 
		select member_principal_id 
		from sys.database_role_members 
		where role_principal_id in (
			select principal_id
			FROM sys.database_principals where [name] = @RoleName  AND type = 'R' ))

	OPEN Member_Cursor;

	FETCH NEXT FROM Member_Cursor
	into @RoleMemberName

	WHILE @@FETCH_STATUS = 0
	BEGIN

		exec sp_droprolemember @rolename=@RoleName, @membername= @RoleMemberName

		FETCH NEXT FROM Member_Cursor
		into @RoleMemberName
	END;

	CLOSE Member_Cursor;
	DEALLOCATE Member_Cursor;
End

GO

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'brady_trading_user' AND type = 'R')
	DROP ROLE [brady_trading_user]
GO

CREATE ROLE [brady_trading_user] AUTHORIZATION [dbo]
GO

EXEC sp_addrolemember brady_trading_insert, brady_trading_user
GO

EXEC sp_addrolemember brady_trading_insert, brady_trading_user
GO

EXEC sp_addrolemember brady_trading_insert, brady_trading_user
GO

EXEC sp_addrolemember brady_trading_insert, brady_trading_user
GO

EXEC sp_addrolemember brady_trading_insert, brady_trading_user
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

GRANT SELECT ON [brady_trading].[schemaversion] TO [brady_trading_select]
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

GRANT EXECUTE ON [brady_trading].[prc_checkversion] TO [brady_trading_execute]
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

GRANT EXECUTE ON [brady_trading].[prc_updateversion] TO [brady_trading_execute]
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

GRANT SELECT ON [brady_trading].[nexthigh] TO [brady_trading_select]
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
	[tradeid] 		    [bigint] 		NOT NULL,
	[bs]				[nvarchar](4),
	[term]				[nvarchar](64),
	[deliverymonth]		[datetime],
	[deliverydate]		[datetime],
	[lots]				[float],
	[commodityamount]   [float],
	[commodityunits]	[nvarchar](64),
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commoditytrade]
   ADD CONSTRAINT [pk_commoditytrade]
      PRIMARY KEY CLUSTERED ( [tradeid] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commoditytrade]
   ADD CONSTRAINT [fk_commoditytrade_trade]
      FOREIGN KEY ([tradeid])
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
	[tradeid] 			[bigint] 		NOT NULL,
	[currencyamount]    [float],
	[baseprice]   	    [float],
	[price]   			[float],
	[spread]			[float],
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityfuture]
   ADD CONSTRAINT [pk_commodityfuture]
      PRIMARY KEY CLUSTERED ( [tradeid] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityfuture]
   ADD CONSTRAINT [fk_commodityfuture_commoditytrade]
      FOREIGN KEY ([tradeid])
         REFERENCES [brady_trading].[commoditytrade]([tradeid])
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
	[tradeid] 			[bigint] 		NOT NULL,
	[currencyamount]    [float],
	[baseprice]   	    [float],
	[price]   			[float],
	[spread]			[float],
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityforward]
   ADD CONSTRAINT [pk_commodityforward]
      PRIMARY KEY CLUSTERED ( [tradeid] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityforward]
   ADD CONSTRAINT [fk_commodityforward_commoditytrade]
      FOREIGN KEY ([tradeid])
         REFERENCES [brady_trading].[commoditytrade]([tradeid])
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

CREATE TABLE [brady_trading].[vanillaaveragedetails](
	[averagedetailsid] 		[bigint] 		NOT NULL,
	[fixingindex]	    	[nvarchar](64),
	[startdate]		    	[datetime],
	[enddate]		    	[datetime],
	[additivepremium]      	[float],
	[additivepremiumunits]	[nvarchar](64),
	[percentagepremium] 	[float]
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[vanillaaveragedetails]
   ADD CONSTRAINT [pk_vanillaaveragedetails]
      PRIMARY KEY CLUSTERED ( [averagedetailsid] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[vanillaaveragedetails]
   ADD CONSTRAINT [fk_vanillaaveragedetails_averagedetails]
      FOREIGN KEY ([averagedetailsid])
         REFERENCES [brady_trading].[averagedetails]([id])
            ON DELETE CASCADE
GO

GRANT INSERT ON [brady_trading].[vanillaaveragedetails] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[vanillaaveragedetails] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[vanillaaveragedetails] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[vanillaaveragedetails] TO [brady_trading_select]
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

CREATE TABLE [brady_trading].[vanillaoptiondetails](
	[optiondetailsid] 		[bigint] 		NOT NULL,
	[cp]	    			[nvarchar](4),
	[currencyamount]		[float],
	[model]	    			[nvarchar](64),
	[strikeprice]			[float],
	[expirymonth]		    [datetime],
	[expirydate]		    [datetime],
	[premiumdate]		    [datetime],
	[premiumcurrency]	   	[nvarchar](64),
	[premiumrate]			[float],
	[premiumamount] 		[float]
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[vanillaoptiondetails]
   ADD CONSTRAINT [pk_vanillaoptiondetails]
      PRIMARY KEY CLUSTERED ( [optiondetailsid] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[vanillaoptiondetails]
   ADD CONSTRAINT [fk_vanillaoptiondetails_optiondetails]
      FOREIGN KEY ([optiondetailsid])
         REFERENCES [brady_trading].[optiondetails]([id])
            ON DELETE CASCADE
GO

GRANT INSERT ON [brady_trading].[vanillaoptiondetails] TO [brady_trading_insert]
GO

GRANT UPDATE ON [brady_trading].[vanillaoptiondetails] TO [brady_trading_update]
GO

GRANT DELETE ON [brady_trading].[vanillaoptiondetails] TO [brady_trading_delete]
GO

GRANT SELECT ON [brady_trading].[vanillaoptiondetails] TO [brady_trading_select]
GO

CREATE TABLE [brady_trading].[commodityaverage](
	[tradeid] 			[bigint] NOT NULL,
	[averagedetailsid]  [bigint] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityaverage]
   ADD CONSTRAINT [pk_commodityaverage]
      PRIMARY KEY CLUSTERED ( [tradeid] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityaverage]
   ADD CONSTRAINT [fk_commodityaverage_commoditytrade]
      FOREIGN KEY ([tradeid])
         REFERENCES [brady_trading].[commoditytrade]([tradeid])
            ON DELETE CASCADE
GO

ALTER TABLE [brady_trading].[commodityaverage]
   ADD CONSTRAINT [fk_commodityaverage_averagedetails]
      FOREIGN KEY ([averagedetailsid])
         REFERENCES [brady_trading].[averagedetails]([id])
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
	[tradeid] 				[bigint] NOT NULL,
	[averagedetailsid]  	[bigint] NOT NULL,
	[strikeprice]			[float],
	[strikecurrencyamount] 	[float]
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityaverageswap]
   ADD CONSTRAINT [pk_commodityaverageswap]
      PRIMARY KEY CLUSTERED ( [tradeid] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityaverageswap]
   ADD CONSTRAINT [fk_commodityaverageswap_commoditytrade]
      FOREIGN KEY ([tradeid])
         REFERENCES [brady_trading].[commoditytrade]([tradeid])
            ON DELETE CASCADE
GO

ALTER TABLE [brady_trading].[commodityaverageswap]
   ADD CONSTRAINT [fk_commodityaverageswap_averagedetails]
      FOREIGN KEY ([averagedetailsid])
         REFERENCES [brady_trading].[averagedetails]([id])
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
	[tradeid]			[bigint] NOT NULL,
	[optiondetailsid]   [bigint] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityoption]
   ADD CONSTRAINT [pk_commodityoption]
      PRIMARY KEY CLUSTERED ( [tradeid] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commodityoption]
   ADD CONSTRAINT [fk_commodityoption_commoditytrade]
      FOREIGN KEY ([tradeid])
         REFERENCES [brady_trading].[commoditytrade]([tradeid])
            ON DELETE CASCADE
GO

ALTER TABLE [brady_trading].[commodityoption]
   ADD CONSTRAINT [fk_commodityoption_optiondetails]
      FOREIGN KEY ([optiondetailsid])
         REFERENCES [brady_trading].[optiondetails]([id])
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
	[tradeid] 			[bigint] NOT NULL,
	[averagedetailsid]  [bigint] NOT NULL,
	[optiondetailsid]   [bigint] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commoditytapo]
   ADD CONSTRAINT [pk_commoditytapo]
      PRIMARY KEY CLUSTERED ( [tradeid] ) 
         ON [PRIMARY]
GO

ALTER TABLE [brady_trading].[commoditytapo]
   ADD CONSTRAINT [fk_commoditytapo_commoditytrade]
      FOREIGN KEY ([tradeid])
         REFERENCES [brady_trading].[commoditytrade]([tradeid])
            ON DELETE CASCADE
GO

ALTER TABLE [brady_trading].[commoditytapo]
   ADD CONSTRAINT [fk_commoditytapo_averagedetails]
      FOREIGN KEY ([averagedetailsid])
         REFERENCES [brady_trading].[averagedetails]([id])
            ON DELETE CASCADE
GO

ALTER TABLE [brady_trading].[commoditytapo]
   ADD CONSTRAINT [fk_commoditytapo_optiondetails]
      FOREIGN KEY ([optiondetailsid])
         REFERENCES [brady_trading].[optiondetails]([id])
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

INSERT INTO [brady_trading].[nexthigh] ([NextHigh], [EntityName])
   VALUES (0, 'trade')
GO

INSERT INTO [brady_trading].[nexthigh] ([NextHigh], [EntityName])
   VALUES (0, 'averagedetails')
GO

INSERT INTO [brady_trading].[nexthigh] ([NextHigh], [EntityName])
   VALUES (0, 'optiondetails')
GO

[brady_trading].[prc_updateversion] 2016, 2, 0, 1
GO

SET NOEXEC OFF