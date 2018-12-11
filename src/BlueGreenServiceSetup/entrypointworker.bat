ECHO "Run ReferenceFD Setup"
CD ReferenceServiceSetup
call setup.bat
CD ..
    
ECHO "Run Reference Worker"
CD ReferenceWorker
call start.bat