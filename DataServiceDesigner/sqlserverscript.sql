USE [dsd-latest]
GO
DROP TRIGGER [dataservicedesigner].[trg_domainobjectproperty_delete]
GO
DROP TABLE [dataservicedesigner].[domainobjectreferenceproperty]
GO
DROP TABLE [dataservicedesigner].[domainobjectreference]
GO
DROP TABLE [dataservicedesigner].[domainobjectproperty]
GO
DROP TABLE [dataservicedesigner].[domainobject]
GO
DROP TABLE [dataservicedesigner].[domainschema]
GO
DROP TABLE [dataservicedesigner].[domaindataservice]
GO
DROP TABLE [dataservicedesigner].[connection]
GO
DROP TABLE [dataservicedesigner].[schemaversion]
GO
DROP TABLE [dataservicedesigner].[nexthigh]
GO
DROP SCHEMA [dataservicedesigner]
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

CREATE TABLE [dataservicedesigner].[domaindataservice](
   [id]               [BIGINT]        NOT NULL,
   [name]             [NVARCHAR](64)  NOT NULL,
   [connectionid]     [BIGINT]
)
GO

ALTER TABLE [dataservicedesigner].[domaindataservice] 
   ADD CONSTRAINT [pk_domaindataservice] 
      PRIMARY KEY CLUSTERED ([id])
GO

ALTER TABLE [dataservicedesigner].[domaindataservice] 
   ADD CONSTRAINT [uk_domaindataservice_name] 
      UNIQUE NONCLUSTERED ([name]);
GO 

ALTER TABLE [dataservicedesigner].[domaindataservice] 
   ADD CONSTRAINT [fk_domaindataservice1] 
      FOREIGN KEY([connectionid])
         REFERENCES [dataservicedesigner].[connection] ([id])
            ON DELETE SET NULL
GO

CREATE TABLE [dataservicedesigner].[domainschema](
   [id]              [BIGINT]        NOT NULL,
   [dataserviceid]   [BIGINT]        NOT NULL,
   [schemaname]      [NVARCHAR](64)  NOT NULL,
   [isdefault]       [BIT]
)
GO

ALTER TABLE [dataservicedesigner].[domainschema] 
   ADD CONSTRAINT [pk_domainschema] 
      PRIMARY KEY CLUSTERED ([id])
GO

ALTER TABLE [dataservicedesigner].[domainschema] 
   ADD CONSTRAINT [uk_domainschema_name] 
      UNIQUE NONCLUSTERED ([dataserviceid],[schemaname]);
GO 

ALTER TABLE [dataservicedesigner].[domainschema] 
   ADD CONSTRAINT [fk_domainschema1] 
      FOREIGN KEY([dataserviceid])
         REFERENCES [dataservicedesigner].[domaindataservice] ([id])
            ON DELETE CASCADE
GO

CREATE TABLE [dataservicedesigner].[domainobject](
   [id]                    [BIGINT]       NOT NULL,
   [schemaid]              [BIGINT]       NOT NULL,
   [tablename]             [NVARCHAR](30) NOT NULL,
   [objectname]            [NVARCHAR](64) NULL,
   [displayname]           [NVARCHAR](64) NULL,
   [pluraliseddisplayname] [NVARCHAR](64) NULL
)
GO
 
ALTER TABLE [dataservicedesigner].[domainobject]
   ADD CONSTRAINT [pk_domainobject] 
      PRIMARY KEY CLUSTERED ([id])
GO

ALTER TABLE [dataservicedesigner].[domainobject] 
   ADD CONSTRAINT [uk_domainobject_dbname] 
      UNIQUE NONCLUSTERED ([schemaid],[objectname]);
GO 

ALTER TABLE [dataservicedesigner].[domainobject] 
   ADD CONSTRAINT [fk_domainobject1] 
      FOREIGN KEY([schemaid])
         REFERENCES [dataservicedesigner].[domainschema] ([id])
            ON DELETE CASCADE
GO

CREATE TABLE [dataservicedesigner].[domainobjectproperty](
   [id]                   [BIGINT]       NOT NULL,
   [objectid]             [BIGINT]       NOT NULL,
   [columnname]           [NVARCHAR](30) NOT NULL,
   [propertyname]         [NVARCHAR](64),
   [displayname]          [NVARCHAR](64),
   [propertytype]         [NVARCHAR](64) NOT NULL,
   [length]	              [INT],
   [isnullable]           [BIT],
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
   ADD CONSTRAINT [uk_domainobjectproperty_columnname] 
      UNIQUE NONCLUSTERED ([objectid],[columnname]);
GO 

ALTER TABLE [dataservicedesigner].[domainobjectproperty] 
   ADD CONSTRAINT [uk_domainobjectproperty_propertyname] 
      UNIQUE NONCLUSTERED ([objectid],[propertyname]);
GO 

CREATE TABLE dataservicedesigner.domainobjectreference (
   [id]               [BIGINT]        NOT NULL,
   [parentobjectid]   [BIGINT]        NOT NULL,
   [childobjectid]    [BIGINT]        NOT NULL,
   [referencename]    [NVARCHAR](64)  NOT NULL,
   [constraintname]   [NVARCHAR](30)  NOT NULL,
   [referencetype]    [NVARCHAR](10)  NOT NULL
)
GO

ALTER TABLE [dataservicedesigner].[domainobjectreference]
   ADD CONSTRAINT [pk_domainobjectreference] 
      PRIMARY KEY CLUSTERED ([id])
GO

ALTER TABLE [dataservicedesigner].[domainobjectreference] 
   ADD CONSTRAINT [uk_domainobjectreference_referencename] 
      UNIQUE NONCLUSTERED ([referencename]);
GO

ALTER TABLE [dataservicedesigner].[domainobjectreference] 
   ADD CONSTRAINT [uk_domainobjectreference_constraintname] 
      UNIQUE NONCLUSTERED ([constraintname]);
GO

ALTER TABLE [dataservicedesigner].[domainobjectreference] 
   ADD CONSTRAINT [fk_domainobjectreference1] 
      FOREIGN KEY([parentobjectid])
         REFERENCES [dataservicedesigner].[domainobject] ([id])
            ON DELETE NO ACTION
GO

ALTER TABLE [dataservicedesigner].[domainobjectreference] 
   ADD CONSTRAINT [fk_domainobjectreference2] 
      FOREIGN KEY([childobjectid])
         REFERENCES [dataservicedesigner].[domainobject] ([id])
            ON DELETE NO ACTION
GO

CREATE TABLE [dataservicedesigner].[domainobjectreferenceproperty] (
   [id]                 [BIGINT]   NOT NULL,	
   [referenceid]        [BIGINT]   NOT NULL,	
   [parentpropertyid]   [BIGINT]   NOT NULL,
   [childpropertyid]    [BIGINT]   NOT NULL
)
GO

ALTER TABLE [dataservicedesigner].[domainobjectreferenceproperty]
   ADD CONSTRAINT [pk_domainobjectreferenceproperty] 
      PRIMARY KEY CLUSTERED ([referenceid],[parentpropertyid],[childpropertyid])
GO

ALTER TABLE [dataservicedesigner].[domainobjectreferenceproperty]
   ADD CONSTRAINT [ck_domainobjectreferenceproperty1] 
      CHECK(parentpropertyid != childpropertyid)
GO

ALTER TABLE [dataservicedesigner].[domainobjectreferenceproperty] 
   ADD CONSTRAINT [fk_domainobjectreferenceproperty1] 
      FOREIGN KEY([parentpropertyid])
         REFERENCES [dataservicedesigner].[domainobjectproperty] ([id])
            ON DELETE NO ACTION
GO

ALTER TABLE [dataservicedesigner].[domainobjectreferenceproperty] 
   ADD CONSTRAINT [fk_domainobjectreferenceproperty2] 
      FOREIGN KEY([childpropertyid])
         REFERENCES [dataservicedesigner].[domainobjectproperty] ([id])
            ON DELETE NO ACTION
GO

CREATE TRIGGER [dataservicedesigner].[trg_domainobject_delete]
   ON [dataservicedesigner].[domainobject]
      FOR DELETE
AS
	DELETE FROM [dataservicedesigner].[domainobjectreference]
	   WHERE 
	      [parentobjectid] IN (SELECT deleted.id FROM deleted) OR
		   [childobjectid] IN (SELECT deleted.id FROM deleted) 
GO

CREATE TRIGGER [dataservicedesigner].[trg_domainobjectproperty_delete]
   ON [dataservicedesigner].[domainobjectproperty]
      FOR DELETE
AS
	DELETE FROM [dataservicedesigner].[domainobjectreferenceproperty]
	   WHERE 
	      [parentpropertyid] IN (SELECT deleted.id FROM deleted) OR
		  [childpropertyid] IN (SELECT deleted.id FROM deleted) 
GO

SET IDENTITY_INSERT [dataservicedesigner].[nexthigh] ON
INSERT [dataservicedesigner].[nexthigh] ([id], [nexthigh], [entityname]) VALUES (1, 1, N'dataserviceconnection')
INSERT [dataservicedesigner].[nexthigh] ([id], [nexthigh], [entityname]) VALUES (2, 1, N'domaindataservice')
INSERT [dataservicedesigner].[nexthigh] ([id], [nexthigh], [entityname]) VALUES (3, 1, N'domainschema')
INSERT [dataservicedesigner].[nexthigh] ([id], [nexthigh], [entityname]) VALUES (4, 1, N'domainobject')
INSERT [dataservicedesigner].[nexthigh] ([id], [nexthigh], [entityname]) VALUES (5, 1, N'domainobjectproperty')
INSERT [dataservicedesigner].[nexthigh] ([id], [nexthigh], [entityname]) VALUES (6, 1, N'domainobjectreference')
INSERT [dataservicedesigner].[nexthigh] ([id], [nexthigh], [entityname]) VALUES (7, 1, N'domainobjectreferenceproperty')
SET IDENTITY_INSERT [dataservicedesigner].[nexthigh] OFF
GO
