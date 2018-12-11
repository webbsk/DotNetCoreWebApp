ECHO "Run ReferenceFD Setup"
CD ReferenceServiceSetup
call setup.bat
CD ..
    
ECHO "Run Reference FD"
CD ReferenceFD 
call start.bat