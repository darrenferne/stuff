USE [limits-prototype]
GO

CREATE SCHEMA [limits]
GO

CREATE TABLE [limits].[schemaversion](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[version] [nvarchar](50) NOT NULL,
	[major] [int] NOT NULL,
	[minor] [int] NOT NULL,
	[patch] [int] NOT NULL,
	[compile] [int] NOT NULL,
	[timestamp] [datetime] NOT NULL,

    CONSTRAINT [pk_schemaversion] PRIMARY KEY CLUSTERED
    (
        [id] ASC
    )
)
GO

CREATE PROCEDURE limits.prc_checkversion
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
   
   IF NOT EXISTS (SELECT version FROM limits.schemaversion WHERE major = @OldMajor AND minor = @OldMinor AND patch = @OldPatch AND compile = @OldCompile)
   BEGIN 
      RAISERROR('Cannot upgrade to %d.%d.%d.%d before schema is upgraded to %d.%d.%d.%d', 16, 1, @NewMajor, @NewMinor, @NewPatch, @NewCompile, @OldMajor, @OldMinor, @OldPatch, @OldCompile)
   END 
   
   IF EXISTS (SELECT version FROM limits.schemaversion WHERE major = @NewMajor AND minor = @NewMinor AND patch = @NewPatch AND compile = @NewCompile)
   BEGIN
      RAISERROR('Cannot upgrade to %d.%d.%d.%d as schema is already at this version or greater', 16, 1, @NewMajor, @NewMinor, @NewPatch, @NewCompile)
   END 
END
GO

CREATE PROCEDURE limits.prc_updateversion
    ( @NewMajor    INT
    , @NewMinor    INT
    , @NewPatch    INT
    , @NewCompile  INT
   )
AS
BEGIN
   DECLARE @version NVARCHAR(43)
   SET @version = (CAST(@NewMajor AS NVARCHAR(10)) + '.' + CAST(@NewMinor AS NVARCHAR(10)) + '.' + CAST(@NewPatch AS NVARCHAR(10)) + '.' + CAST(@NewCompile AS NVARCHAR(10)))
   
   INSERT INTO limits.schemaversion (version, major, minor, patch, compile)
      VALUES (@version, @NewMajor, @NewMinor, @NewPatch, @NewCompile)
END
GO

CREATE TABLE [limits].[nexthigh](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nexthigh] [bigint] NOT NULL,
	[entityname] [nvarchar](30) NOT NULL,

    CONSTRAINT [pk_nexthigh] PRIMARY KEY CLUSTERED
    (
        [id] ASC
    ),
    CONSTRAINT [uk_nexthigh_entityname] UNIQUE NONCLUSTERED
    (
        [entityname] ASC
    )
)
GO

CREATE TABLE [limits].[provisionalcontract] (
   [id]              BIGINT NOT NULL,
   [contractid]      NVARCHAR(64) NOT NULL,
   [clientnumber]    NVARCHAR(64) NOT NULL,
   [clientname]      NVARCHAR(64) NOT NULL,
   [product]         NVARCHAR(64) NOT NULL,
   [quantity]        FLOAT NOT NULL,
   [quantityunit]    NVARCHAR(64) NOT NULL,
   [premium]         FLOAT NOT NULL

   CONSTRAINT [pk_provisionalcontract] PRIMARY KEY CLUSTERED
   (
      [id] ASC
   )
)
GO

INSERT INTO [limits].[nexthigh] ([nexthigh], [entityname])
     VALUES (0, 'ProvisionalContract')
GO

CREATE TABLE [limits].[a_provisionalcontract] (
	[bwf_auditId]             BIGINT            NOT NULL,
   [bwf_actionId]            BIGINT            NOT NULL,
   [bwf_auditSummary]        NVARCHAR(1024)    NOT NULL,
	[id]                      BIGINT            NOT NULL,
   [contractid]      NVARCHAR(64) NOT NULL,
   [clientnumber]    NVARCHAR(64) NOT NULL,
   [clientname]      NVARCHAR(64) NOT NULL,
   [product]         NVARCHAR(64) NOT NULL,
   [quantity]        FLOAT NOT NULL,
   [quantityunit]    NVARCHAR(64) NOT NULL,
   [premium]         FLOAT NOT NULL
	
    CONSTRAINT [pk_a_provisionalcontract] PRIMARY KEY CLUSTERED
    (
        [bwf_auditId] ASC
    )
)
GO

INSERT INTO [limits].[nexthigh] (nexthigh, entityname)
    VALUES (0, 'ProvisionalContractAudit');
GO	

CREATE TABLE [limits].bwfaudit (
    id          BIGINT              NOT NULL,
    action      NVARCHAR(50)        NOT NULL,
    timestamp   DATETIME2           NOT NULL,
    username    NVARCHAR(50)        NULL,
    
	CONSTRAINT pk_bwfaudit
        PRIMARY KEY CLUSTERED (id ASC)
)
GO

INSERT INTO [limits].nexthigh (nexthigh, entityname)
    VALUES (0, 'BwfAudit');
GO
	
CREATE TABLE limits.bwfservicelevelpermission (
    id          BIGINT          NOT NULL,
    roleid      BIGINT          NOT NULL,
    type        NVARCHAR(255)   NOT NULL,
    name        NVARCHAR(255)   NOT NULL,
    rolename    NVARCHAR(255)   NOT NULL,
    
	CONSTRAINT [pk_bwfservicelevelpermission] PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)
)
GO

INSERT INTO limits.nexthigh (nexthigh, entityname)
    VALUES (0, 'BwfServiceLevelPermission');
GO
	
brady_membership.prc_updateversion 0, 1, 0, 1
GO
