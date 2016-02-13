#------------------  ON MySitterHub AppServer, finish deployment ----------------

# Change present working directory to script location
cd $PSScriptRoot

# STEP - Stop web and app services


# STEP - Unzip to C:\MySitterHubApp


# STEP - rename 'appconfig.prod.json' to 'appconfig.json'
Remove-Item "C:\MySitterHubApp\Sitter.AppService.Console\bin\Debug\appconfig.prod.json"
rename-item "C:\MySitterHubApp\Sitter.AppService.Console\bin\Debug\appconfig.prod.json" "appconfig.json"

# STEP - start web and app services
$websvc= "C:\MySitterHubApp\Sitter.Host.Console\bin\Debug\MySitterHub.Host.Console.exe"
invoke-item $websvc
$appsvc = "C:\MySitterHubApp\Sitter.AppService.Console\bin\Debug\MySitterHub.AppService.exe"
invoke-item $appsvc 







