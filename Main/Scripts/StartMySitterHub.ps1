########################################################################################
# Start MySitterHub Console
########################################################################################

cd $PSScriptRoot

$web = "..\src\Sitter.Host.Console\bin\Debug"

#cd $web

#invoke-item "MySitterHub.Host.Console.exe"


#split-path $web -Parent | cd
#Get-Location

#$webf = split-path $web -Leaf

#Invoke-Item $web


<#
$appsvc = "..\src\Sitter.AppService.Console\bin\Debug\MySitterHub.AppService.exe"
split-path $appsvc -parent | Set-Location

$toobox = "..\src\Sitter.ToolBox\bin\Debug\Sitter.ToolBox.exe"



$webf = resolve-path $web
$webdir = split-path $web
write-host $webdir

Set-Location 
#Get-Location


#>