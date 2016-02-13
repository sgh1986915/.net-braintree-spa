#------------------  Create MySitterHub Deployment Package zip ----------------

# STEP -- Change present working directory to script location
cd $PSScriptRoot

# STEP --  Set appDeployDate
$appDeployDate = Get-Date -format "yyyyMMdd_HHmmss"
write-host "STEP - Copying folders to AppDeploy"
write-host "appDeployDate:$appDeployDate"

$dest = ".\AppDeploy\$appDeployDate"

# STEP -- Copy just the needed directories
Copy-Item ..\src\Sitter.Host.Console\bin\Debug $dest\Sitter.Host.Console\bin\Debug -Recurse
Copy-Item ..\src\Sitter.AppService.Console\bin\Debug $dest\Sitter.AppService.Console\bin\Debug -Recurse
Copy-Item ..\src\Sitter.Client\src\build $dest\Sitter.Client\src\build -recurse
Copy-Item ..\src\Sitter.ToolBox\bin\Debug $dest\Sitter.ToolBox\bin\Debug -Recurse
Remove-Item $dest\Sitter.Toolbox\bin\Debug\AWSSDK.pdb
Remove-Item $dest\Sitter.AppService.Console\bin\Debug\AWSSDK.pdb

#New-Item $dest\Scripts -ItemType Directory
#Copy-Item .\RemoteDeploy.ps1 -destination $dest\Scripts

write-zip $dest $dest\mysitterhub_$appDeployDate.zip 

invoke-item $dest


