ECHO Restoring NPM packages for the Explorer Solution
CALL NPM install

IF [%1] NEQ [ro] (
	ECHO Updating NPM packages for the Explorer Solution
	CALL NPM update @brady/bwf
)