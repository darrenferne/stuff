/* DROP THE DYNAMITE SCHEMA */
DROP USER DYN_COMMON CASCADE;
DROP USER DYN_STATICDATA CASCADE;
DROP USER DYN_MARKETDATA CASCADE;
DROP USER DYN_MARKETDATAMAPPING CASCADE;
DROP USER DYN_DATASERVICE CASCADE;
DROP USER DYN_VALUATION CASCADE;
DROP USER EXP_VIEW CASCADE;
DROP USER EXP_CREDENTIAL CASCADE;
DROP USER EXP_COMMON CASCADE;
DROP USER BRADY_COMMON CASCADE;
DROP USER BRADY_LOGGING_USER CASCADE;
DROP USER TRINITY_TRADE_MODEL CASCADE;
DROP USER TRADE_MODEL_USER CASCADE;
DROP USER TRINITY_BEE CASCADE;
DROP USER TRINITY_DATASERVICE CASCADE;
DROP USER TRINITY_VAL_EVENT_SOURCE CASCADE;
DROP USER TRINITY_VAL_EVENT_SOURCE_USER CASCADE;
DROP USER BRADY_MEMBERSHIP CASCADE;
DROP USER MEMBERSHIP_SERVICE_USER CASCADE;
DROP USER DYN_ACCOUNTS CASCADE;
DROP USER DYN_COLLATERAL CASCADE;
DROP USER DYN_LIMITS CASCADE;
DROP USER DYN_SPAN CASCADE;
DROP USER DYN_MARGIN CASCADE;
DROP USER BROKER_TRADE_FEED CASCADE;
DROP USER ASP CASCADE;

DROP ROLE BRADY_SELECT;
DROP ROLE BRADY_DELETE;
DROP ROLE BRADY_EXECUTE;
DROP ROLE BRADY_INSERT;
DROP ROLE BRADY_UPDATE;
DROP ROLE BRADY_USER;
DROP ROLE DYNAMITE_DELETE;
DROP ROLE DYNAMITE_INSERT;
DROP ROLE DYNAMITE_SELECT;
DROP ROLE DYNAMITE_UPDATE;
DROP ROLE DYNAMITE_USER;
DROP ROLE DYN_MARGIN_DELETE;
DROP ROLE DYN_MARGIN_EXECUTE;
DROP ROLE DYN_MARGIN_INSERT;
DROP ROLE DYN_MARGIN_SELECT;
DROP ROLE DYN_MARGIN_UPDATE;
DROP ROLE DYN_MARGIN_USER;
DROP ROLE DYN_SPAN_USER;
DROP ROLE DYN_STATICDATA_DELETE;
DROP ROLE DYN_STATICDATA_INSERT;
DROP ROLE DYN_STATICDATA_SELECT;
DROP ROLE DYN_STATICDATA_UPDATE;
DROP ROLE DYN_STATICDATA_USER;
DROP ROLE EXPLORER_DELETE;
DROP ROLE EXPLORER_INSERT;
DROP ROLE EXPLORER_SELECT;
DROP ROLE EXPLORER_UPDATE;
DROP ROLE EXPLORER_USER;
DROP ROLE TRINITY_TRADE_INTERFACE;
DROP ROLE TRINITY_VAL_EVENT_SOURCE_ROLE;
DROP ROLE MEMBERSHIP_ADMIN;
DROP ROLE MEMBERSHIP_READ;
DROP ROLE ORA_ASPNET_MEM_BASICACCESS;
DROP ROLE ORA_ASPNET_MEM_FULLACCESS;
DROP ROLE ORA_ASPNET_MEM_REPORTACCESS;
DROP ROLE ORA_ASPNET_PROF_BASICACCESS;
DROP ROLE ORA_ASPNET_PROF_FULLACCESS;
DROP ROLE ORA_ASPNET_PROF_REPORTACCESS;
DROP ROLE ORA_ASPNET_ROLES_BASICACCESS;
DROP ROLE ORA_ASPNET_ROLES_FULLACCESS;
DROP ROLE ORA_ASPNET_ROLES_REPORTACCESS;