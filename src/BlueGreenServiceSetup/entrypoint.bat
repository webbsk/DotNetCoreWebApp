ECHO "Run BlueGreenService Setup"
CD BlueGreenServiceSetup
call setup.bat
CD ..
    
ECHO "Run BlueGreenFD"
CD BlueGreenFD 
call start.bat