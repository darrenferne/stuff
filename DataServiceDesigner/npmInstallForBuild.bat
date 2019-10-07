SETLOCAL
ECHO Running package restore for the Explorer Host Solution

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

ECHO Restoring NPM packages for the Explorer Host Solution
CALL NPM install

ECHO Updating NPM packages for the Host Solution
CALL NPM install @brady/bwf@next

ECHO Deleting config keys
CALL NPM config delete "//%npm.registry%:_authToken"
CALL NPM config delete "//%npm.registry%:always-auth"
CALL NPM config delete "@brady:registry"
ENDLOCAL