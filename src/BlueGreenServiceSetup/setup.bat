REM Enable MSI access
call EnableManagedServiceIdentity.bat

ECHO "Run ReferenceFD Setup"
dotnet ReferenceServiceSetup.dll -- -RunContainer
