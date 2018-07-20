USE [dsd-latest]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE SCHEMA [dataservicedesigner]
GO

CREATE TABLE [dataservicedesigner].[nexthigh](
   [id]         [INT] IDENTITY(1,1) NOT NULL,
   [nexthigh]   [BIGINT]            NOT NULL,
   [entityname] [NVARCHAR](30)      NOT NULL
)

ALTER TABLE [dataservicedesigner].[nexthigh]
   ADD CONSTRAINT [pk_nexthigh] 
      PRIMARY KEY CLUSTERED ([id])
GO

CREATE TABLE [dataservicedesigner].[schemaversion](
   [id]        [BIGINT]       NOT NULL,
   [version]   [NVARCHAR](50) NOT NULL,
   [major]     [INT]          NOT NULL,
   [minor]     [INT]          NOT NULL,
   [patch]     [INT]          NOT NULL,
   [compile]   [INT]          NOT NULL,
   [timestamp] [DATETIME]     NOT NULL
)

ALTER TABLE [dataservicedesigner].[schemaversion]
   ADD CONSTRAINT [pk_schemaversion] 
      PRIMARY KEY CLUSTERED ([id]) 
GO

ALTER TABLE [dataservicedesigner].[schemaversion] 
   ADD CONSTRAINT [uk_schemaversion_version] UNIQUE NONCLUSTERED ([version]);
GO   

CREATE TABLE [dataservicedesigner].[connection](
   [id]                    [BIGINT]        NOT NULL,
   [name]                  [NVARCHAR](64)  NOT NULL,
   [databasetype]          [NVARCHAR](64)  NOT NULL,
   [datasource]            [NVARCHAR](64)  NOT NULL,
   [initialcatalog]        [NVARCHAR](64),
   [username]              [NVARCHAR](64),
   [password]              [NVARCHAR](64),
   [useintegratedsecurity] [BIT],
   [connectionstring]      [NVARCHAR](256) NOT NULL
)
GO

ALTER TABLE [dataservicedesigner].[connection] 
   ADD CONSTRAINT [pk_connection] 
      PRIMARY KEY CLUSTERED ([id])
GO

ALTER TABLE [dataservicedesigner].[connection] 
   ADD CONSTRAINT [uk_connection_name] UNIQUE NONCLUSTERED ([name]);
GO 

CREATE TABLE [dataservicedesigner].[dataservice](
   [id]               [BIGINT]        NOT NULL,
   [name]             [NVARCHAR](64)  NOT NULL,
   [connectionid]     [BIGINT],
   [defaultschema]    [NVARCHAR](30),
)
GO

ALTER TABLE [dataservicedesigner].[dataservice] 
   ADD CONSTRAINT [pk_dataservice] 
      PRIMARY KEY CLUSTERED ([id])
GO

ALTER TABLE [dataservicedesigner].[dataservice] 
   ADD CONSTRAINT [uk_dataservice_name] UNIQUE NONCLUSTERED ([name]);
GO 

ALTER TABLE [dataservicedesigner].[dataservice] 
   ADD CONSTRAINT [fk_dataservice1] 
      FOREIGN KEY([connectionid])
         REFERENCES [dataservicedesigner].[connection] ([id])
            ON DELETE SET NULL
GO

CREATE TABLE [dataservicedesigner].[domainobject](
   [id]                    [BIGINT]       NOT NULL,
   [dataserviceid]         [BIGINT]       NOT NULL,
   [dbschema]              [NVARCHAR](30) NOT NULL,
   [dbname]                [NVARCHAR](30) NOT NULL,
   [name]                  [NVARCHAR](64) NULL,
   [displayname]           [NVARCHAR](64) NULL,
   [pluraliseddisplayname] [NVARCHAR](64) NULL
)
GO
 
ALTER TABLE [dataservicedesigner].[domainobject]
   ADD CONSTRAINT [pk_domainobject] 
      PRIMARY KEY CLUSTERED ([id])
GO

ALTER TABLE [dataservicedesigner].[domainobject] 
   ADD CONSTRAINT [uk_domainobject_name] UNIQUE NONCLUSTERED ([name]);
GO 

ALTER TABLE [dataservicedesigner].[domainobject] 
   ADD CONSTRAINT [fk_domainobject1] 
      FOREIGN KEY([dataserviceid])
         REFERENCES [dataservicedesigner].[dataservice] ([id])
            ON DELETE CASCADE
GO

CREATE TABLE [dataservicedesigner].[domainobjectproperty](
   [id]                   [BIGINT]       NOT NULL,
   [objectid]             [BIGINT]       NOT NULL,
   [dbname]               [NVARCHAR](30) NOT NULL,
   [name]                 [NVARCHAR](64),
   [displayname]          [NVARCHAR](64),
   [ispartofkey]          [BIT],
   [includeindefaultview] [BIT]
)

ALTER TABLE [dataservicedesigner].[domainobjectproperty]
   ADD CONSTRAINT [pk_domainobjectproperty] 
      PRIMARY KEY CLUSTERED ([id])
GO

ALTER TABLE [dataservicedesigner].[domainobjectproperty] 
   ADD CONSTRAINT [fk_domainobjectproperty1] 
      FOREIGN KEY([objectid])
         REFERENCES [dataservicedesigner].[domainobject] ([id])
            ON DELETE CASCADE
GO

ALTER TABLE [dataservicedesigner].[domainobjectproperty] 
   ADD CONSTRAINT [uk_domainobjectproperty_name] UNIQUE NONCLUSTERED ([name]);
GO 

SET IDENTITY_INSERT [dataservicedesigner].[nexthigh] ON
INSERT [dataservicedesigner].[nexthigh] ([id], [nexthigh], [entityname]) VALUES (1, 1, N'designerdataservice')
INSERT [dataservicedesigner].[nexthigh] ([id], [nexthigh], [entityname]) VALUES (2, 1, N'designerconnection')
INSERT [dataservicedesigner].[nexthigh] ([id], [nexthigh], [entityname]) VALUES (3, 1, N'designerdomainobject')
INSERT [dataservicedesigner].[nexthigh] ([id], [nexthigh], [entityname]) VALUES (4, 1, N'designerdomainobjectproperty')
SET IDENTITY_INSERT [dataservicedesigner].[nexthigh] OFF
GO
