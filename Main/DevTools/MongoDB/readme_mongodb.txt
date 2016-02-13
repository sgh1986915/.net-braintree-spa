1) Install MongoDB Windows 64bit
  (chose complete install)
  http://www.mongodb.org/downloads

2) Run script to create mongo service:
  install_mongo_win_service.cmd
   
Details of what script does:
- This script will create database folder "c:\mongodb\data" and log folder "c:\mongodb\log"
- Create Mongo configuration file here: "c:\mongodb\mongod.cfg"
  which specifies where the log file and data will be stored.
- Call the mongod.exe with the --install switch to set up mongo as a windows service  "mongod.exe --config %cfgfile% --install"
- start the mongo service


3) Install IDE at Robomongo:
This is a great little IDE for connecting to Mongo and looking at data and updating data in MongoDB.
  http://robomongo.org/


  
  