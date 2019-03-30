SET nuget="C:\Program Files\NuGet\NuGet-3-4-4.exe"
SET sources="https://bradyplc.myget.org/F/development/auth/a90627e2-2277-4832-a591-c59e5464064b/api/v3/index.json;http://camvs-nuget12-2.bradyplc.com/api/v2;https://www.nuget.org/api/v2"

ECHO Restoring Limits Solution Packages
%nuget% restore Limits.sln -source %sources% -noninteractive -nocache

IF [%1] NEQ [ro] (
	ECHO Updating Limits Solution Packages
	%nuget% update Brady.Limits.ExplorerHost\packages.config -source %sources% -noninteractive -prerelease -safe -fileconflictaction overwrite -id "combined-bwf-client" -id "combined-bwf-client-repository" -id "combined-bwf-database-dataservice" -id "combined-bwf-dataservice-host" -id "combined-bwf-domain" -id "combined-bwf-explorer-host" -id "combined-bwf-inmemory-dataservice" -id "combined-bwf-nancy-dataservice"
  
)
