# escape=`

# pull down the build container
FROM ustplatformcontainerregistry.azurecr.io/microsoft.universalstore.containers:windowsserverbase-latest
 
 SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

 # Install .NET Core
  ENV DOTNET_VERSION 2.1.2
 ENV DOTNET_DOWNLOAD_URL https://dotnetcli.blob.core.windows.net/dotnet/Runtime/$DOTNET_VERSION/dotnet-runtime-$DOTNET_VERSION-win-x64.zip
 ENV DOTNET_DOWNLOAD_SHA 9e67c62feb34aeb52a45d0e623d1ad098637a322545956eb5e6de2287ddad2412b766c492ae5a7dddc123a4cb47cfc51d9bb10d0e30c007ec3fc90666f9733c8

 RUN Invoke-WebRequest $Env:DOTNET_DOWNLOAD_URL -OutFile dotnet.zip; `
    if ((Get-FileHash dotnet.zip -Algorithm sha512).Hash -ne $Env:DOTNET_DOWNLOAD_SHA) { `
        Write-Host 'CHECKSUM VERIFICATION FAILED!'; `
        exit 1; `
    }; `
    `
    Expand-Archive dotnet.zip -DestinationPath $Env:ProgramFiles\dotnet; `
    Remove-Item -Force dotnet.zip

 RUN setx /M PATH $($Env:PATH + ';' + $Env:ProgramFiles + '\dotnet')

  # Enable detection of running in a container
  ENV DOTNET_RUNNING_IN_CONTAINER=true

  SHELL ["cmd"]
  # install the Geneva docker shim
  WORKDIR /Geneva
  COPY .\drop\Release\Product\ReferenceServiceSetup\Monitoring\DockerShim .

  RUN ["c:\\Geneva\\MonAgentDockerShimLauncher.exe -install"]

  # copy setup output
  WORKDIR /app
  COPY .\drop\Release\Product\ReferenceServiceSetup ReferenceServiceSetup

  # copy build output
  COPY .\drop\Release\Product\ReferenceFD ReferenceFD
  COPY .\drop\Release\Product\ReferenceServiceSetup\entrypoint.bat entrypoint.bat

  # expose ports
  EXPOSE 80
  EXPOSE 443

  # health probe
  HEALTHCHECK --interval=5m --timeout=60s --start-period=30s --retries=4 CMD ["powershell","Invoke-WebRequest -UseBasicParsing 'http://localhost/health/keepalive'"]

  # start the service
  ENTRYPOINT ["cmd.exe", "/k", "entrypoint.bat"]