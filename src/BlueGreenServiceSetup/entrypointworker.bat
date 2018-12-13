ECHO "Run BlueGreenService Setup"
CD BlueGreenServiceSetup
call setup.bat
CD ..
    
ECHO "Run BlueGreen Worker"
CD BlueGreenWorker
call start.bat