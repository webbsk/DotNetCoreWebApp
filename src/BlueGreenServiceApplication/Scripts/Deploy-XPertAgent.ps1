# You must fill in the following configuration settings with YOUR values.

#XPert docs - https://docs.xpert.microsoft.com/documentation/onboard/servicefabric/

# Your Service Name from your XpertModel.xml file.  i.e. "AbbyAwesome-NonPROD-ServiceFabric"
$XpertAgent_Environment = "RefSvc-NonPROD-WestUS"

# Your Service Key from your XpertModel.xml file.
$XpertAgent_ServiceKey = "B4275CA49C8A9E6CA03E54D6212C1FBABA4D07244B9BAD64EE2198636858D398129B7D2272BBB04C3A8D51CBEE80BCCD2ABADF4D92C88C99A44BF702F96DA0A8"

# Role Name for Xpert Agents.  i.e. "ServiceFabric"
$XpertAgent_RoleName = "Shared.ReferenceService"

# Environment for Xpert Agents.  i.e. "osg", "xbox", "xboxnonprod"
$XpertAgent_AcquisitionEnvironment = "osg"

# Your Cluster's Client connection endpoint.  You get this from Azure Portal.  i.e. "vil-cluster.westus.cloudapp.azure.com:19000"
$ConnectionEndpoint = "mp-refsvc-int-wusf.westus.cloudapp.azure.com:19000"

# Specify your Cluster Connection Type.  i.e. "Secure", "Azure".
$ClusterType = "Secure"

# If $ClusterType is "Secure", provide your Server common name of the server certificate installed on the cluster nodes.  i.e. "westus.cloudapp.azure.com"
$ServerCommonName = "refsvc.primary.local"

# If $ClusterType is "Secure", provide your X509 Certificate Thumbprint of the admin client certificate installed on the cluster nodes.
$CertificateThumbprint = "231A15D356973FF8AEBF55D88722BA7268B481AB"

# ======= Do not edit below this line =======
# ======= Do not edit below this line =======
# ======= Do not edit below this line =======

if ($ClusterType -eq "Secure")
{
	if ($CertificateThumbprint -eq "" -or $ServerCommonName -eq "")
	{
		Write-Error -message "Required parameters/settings are missing."
		exit
	}

	$ConnectParams = @{
		ConnectionEndpoint = $ConnectionEndpoint;
		X509Credential = $True;
		StoreLocation = 'LocalMachine';
		StoreName = "MY";
		ServerCommonName = $ServerCommonName;
		FindType = "FindByThumbprint";
		FindValue = $CertificateThumbprint;
		ServerCertThumbprint = $CertificateThumbprint;
	}
}
elseif ($ClusterType -eq "Azure")
{
	$ConnectParams = @{
		ConnectionEndpoint = $ConnectionEndpoint;
		AzureActiveDirectory  = $True;
	}
}
else
{
	$ConnectParams = @{
		ConnectionEndpoint = $ConnectionEndpoint;
	}
}

$RegKey = "HKLM:\SOFTWARE\Microsoft\Service Fabric SDK"
$ModuleFolderPath = (Get-ItemProperty -Path $RegKey -Name FabricSDKPSModulePath).FabricSDKPSModulePath
Import-Module "$ModuleFolderPath\ServiceFabricSDK.psm1"

Connect-ServiceFabricCluster @ConnectParams
if (-not $?)
{
	Write-Error -message "Connect failed.  Check your Connect Parameters."
	exit
}
$global:clusterConnection = $clusterConnection

Test-ServiceFabricClusterConnection
if (-not $?)
{
	Write-Error -message "Connect failed.  Check your Connect Parameters."
	exit
}

if ($XpertAgent_AcquisitionEnvironment -eq "" -or $XpertAgent_Environment -eq "" -or $XpertAgent_RoleName -eq "" -or $XpertAgent_ServiceKey -eq "")
{
	Write-Error -message "Required parameters/settings are missing."
	exit
}

function Update-ApplicationManifest
{
	ren .\XpertAgentProxyApp.sfpkg .\XpertAgentProxyApp.zip

	$zipfile = Resolve-Path .\XpertAgentProxyApp.zip

	$shell = New-Object -Com Shell.Application
	$file = $shell.NameSpace($zipfile.Path).Items() | where Name -eq "ApplicationManifest.xml"
	if (!$file)
	{
		$file = $shell.NameSpace($zipfile.Path).Items() | where Name -eq "ApplicationManifest"
	}
	$shell.NameSpace($PSScriptRoot).copyhere($file)

	$xmlfile = Resolve-Path .\ApplicationManifest.xml

	[xml]$XmlDocument = Get-Content -Path $xmlfile

	$node = $XmlDocument.ApplicationManifest.Parameters.Parameter | where {$_.Name -eq 'XpertAgent_AcquisitionEnvironment'}
	$node.DefaultValue = $XpertAgent_AcquisitionEnvironment

	$node = $XmlDocument.ApplicationManifest.Parameters.Parameter | where {$_.Name -eq 'XpertAgent_Environment'}
	$node.DefaultValue = $XpertAgent_Environment

	$node = $XmlDocument.ApplicationManifest.Parameters.Parameter | where {$_.Name -eq 'XpertAgent_RoleName'}
	$node.DefaultValue = $XpertAgent_RoleName

	$node = $XmlDocument.ApplicationManifest.Parameters.Parameter | where {$_.Name -eq 'XpertAgent_ServiceKey'}
	$node.DefaultValue = $XpertAgent_ServiceKey

	$node = $XmlDocument.ApplicationManifest.Parameters.Parameter | where {$_.Name -eq 'CertificateThumbprint'}
	$node.DefaultValue = $CertificateThumbprint

	$XmlDocument.Save($xmlfile)

	Compress-Archive -Update -Path $xmlfile -DestinationPath $zipfile.Path

	del $xmlfile
	ren .\XpertAgentProxyApp.zip .\XpertAgentProxyApp.sfpkg
}

Update-ApplicationManifest

$ApplicationName = "fabric:/XpertAgentProxyApp"
$pkgpath = Resolve-Path .\XpertAgentProxyApp.sfpkg
$Params = @{
	XpertAgent_AcquisitionEnvironment = $XpertAgent_AcquisitionEnvironment;
	XpertAgent_Environment = $XpertAgent_Environment;
	XpertAgent_RoleName = $XpertAgent_RoleName;
	XpertAgent_ServiceKey = $XpertAgent_ServiceKey;
	CertificateThumbprint = $CertificateThumbprint;
}

Publish-NewServiceFabricApplication -ApplicationPackagePath $pkgpath -ApplicationName $ApplicationName -ApplicationParameter $Params -OverwriteBehavior Always
