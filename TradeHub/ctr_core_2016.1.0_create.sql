:SetVar ScriptPath "C:\Git\dynamite\DatabaseScripts\MasterScripts\SQLServer\2016.1\CTR Core"
:SetVar ExplorerDb trading
:SetVar MembershipDb trading

:on error exit
:out $(ScriptPath)\ctr_2016.1.0_create.log

ALTER DATABASE [$(ExplorerDb)] SET SINGLE_USER WITH NO_WAIT
GO

ALTER DATABASE [$(MembershipDb)] SET SINGLE_USER WITH NO_WAIT
GO

USE [$(ExplorerDb)]
GO
:r $(ScriptPath)\explorer_2012_1_0_1.sql
:r $(ScriptPath)\explorer_2012_3_0_1.sql
:r $(ScriptPath)\explorer_2013_2_0_1.sql
:r $(ScriptPath)\explorer_2013_2_0_2.sql

USE [$(ExplorerDb)]
GO
:r $(ScriptPath)\explorer_2013_4_0_1.sql
:r $(ScriptPath)\explorer_2013_4_0_2.sql
:r $(ScriptPath)\explorer_2013_4_0_3.sql
:r $(ScriptPath)\explorer_2013_4_0_4.sql
:r $(ScriptPath)\explorer_2014_1_0_1.sql
:r $(ScriptPath)\explorer_2014_1_0_2.sql
:r $(ScriptPath)\explorer_2014_1_0_3.sql
:r $(ScriptPath)\explorer_2014_1_0_4.sql

USE [$(MembershipDb)]
GO
:r $(ScriptPath)\membership_2013_1_0_1.sql
:r $(ScriptPath)\membership_2013_1_0_2.sql
:r $(ScriptPath)\membership_2013_1_0_2_explorer_1.sql
:r $(ScriptPath)\membership_2013_3_0_1.sql
:r $(ScriptPath)\membership_2013_3_0_2.sql
:r $(ScriptPath)\membership_2014_1_0_1.sql
:r $(ScriptPath)\membership_2014_1_0_2.sql
:r $(ScriptPath)\membership_2014_1_0_3.sql
:r $(ScriptPath)\membership_2014_1_0_3_marketdata_1.sql

USE [$(ExplorerDb)]
GO
:r $(ScriptPath)\explorer_2014_2_0_1.sql
:r $(ScriptPath)\explorer_2014_3_0_1.sql
:r $(ScriptPath)\explorer_2015_1_0_1.sql
:r $(ScriptPath)\explorer_2015_1_0_2.sql
:r $(ScriptPath)\explorer_2015_1_0_3.sql

USE [$(MembershipDb)]
GO
:r $(ScriptPath)\membership_2014_3_0_1.sql

USE [$(ExplorerDb)]
GO
:r $(ScriptPath)\explorer_2015_2_0_1.sql

USE [$(MembershipDb)]
GO

USE [$(ExplorerDb)]
GO
:r $(ScriptPath)\explorer_2015_3_0_1.sql
:r $(ScriptPath)\explorer_2015_3_0_2.sql

USE [$(MembershipDb)]
GO
:r $(ScriptPath)\membership_2015_3_0_1.sql
:r $(ScriptPath)\membership_2015_3_0_2.sql
:r $(ScriptPath)\membership_2015_3_0_3.sql

USE [$(ExplorerDb)]
GO
:r $(ScriptPath)\explorer_2016_1_0_1.sql
:r $(ScriptPath)\explorer_2016_1_0_2.sql
:r $(ScriptPath)\explorer_2016_1_0_3.sql
:r $(ScriptPath)\explorer_2016_1_0_4.sql

ALTER DATABASE [$(ExplorerDb)] SET MULTI_USER
GO

ALTER DATABASE [$(MembershipDb)] SET MULTI_USER
GO
