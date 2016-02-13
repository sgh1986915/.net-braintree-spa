@echo off
SET dbfolder="c:\mongodb\data"
SET logfolder="c:\mongodb\log"
SET cfgfile="c:\mongodb\mongod.cfg"

if exist %dbfolder% goto FolderExists
echo == Creating data folder: %dbfolder%
mkdir "%dbfolder%"
:FolderExists 
echo

if exist %logfolder% goto LogFolderExists
echo == Creating log folder: %logfolder%
mkdir %logfolder%
:LogFolderExists

echo == Creating mongodb.cfg file: mongod.cfg
echo logpath=c:\mongodb\log\mongod.log > %cfgfile%
echo dbpath=c:\mongodb\data >> %cfgfile%

echo == running command: "mongod --install" (install mongodb as windows service)
@echo on
"C:\Program Files\MongoDB\Server\3.0\bin\mongod.exe" --config %cfgfile% --install
@echo off
echo
echo

echo == Starting MongoDB Windows service
timeout 2
net start MongoDB
@echo off

echo == DONE
