SETLOCAL
ECHO Running package restore for the Valuation Solution

IF [%1] NEQ [] (
   SET npm.registry=%1
) ELSE (
   SET npm.registry=bradyplc.myget.org/F/development/npm/
)

IF [%2] NEQ [] (
   SET npm.auth_token=%2
) ELSE (
   SET npm.auth_token=82adbd0a-719f-4683-893b-178c60b387d2
)

ECHO Setting registry
ECHO npm.registry: %npm.registry%
ECHO npm.auth_token: %npm.auth_token%
CALL NPM config set "@brady:registry" "https://%npm.registry%"
CALL NPM config set "//%npm.registry%:_authToken" "%npm.auth_token%" 
CALL NPM config set "//%npm.registry%:always-auth" "true"

ECHO Running npm install for the Notifications Solution
CD .\Notifications
CALL npmInstallForBuild.bat %3
ECHO.

ECHO Running npm install for the Explorer Solution
CD ..\Explorer
CALL npmInstallForBuild.bat %3
ECHO.

ECHO Running npm install for the Valuation Solution
CD ..\Valuation\Brady.Valuation
CALL npmInstallForBuild.bat %3
ECHO.

ECHO Running npm install for the Market Data Solution
CD ..\..\MarketData
CALL npmInstallForBuild.bat %3
ECHO.

ECHO Running npm install for the Curves Solution
CD ..\Curves\Brady.Curves
CALL npmInstallForBuild.bat %3
ECHO.

ECHO Running npm install for the Market Data Solution
CD ..\..\SPAN\Brady.SPAN
CALL npmInstallForBuild.bat %3
ECHO.

CD ..\..

ECHO Deleting config keys
CALL NPM config delete "//%npm.registry%:_authToken"
CALL NPM config delete "//%npm.registry%:always-auth"
CALL NPM config delete "@brady:registry"
ENDLOCAL
