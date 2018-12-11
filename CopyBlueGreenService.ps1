#----------------------------------------------------------------
# <copyright company="Microsoft Corporation">
#     Copyright (c) Microsoft Corporation.  All rights reserved.
# </copyright>
#----------------------------------------------------------------

<#
    .SYNOPSIS
    Copies reference service.

    .DESCRIPTION
    Creates a copy of reference service directory content at the designated target dir.
    Renames files and directories to have new service name instead of "Reference" prefix.
    Also renames content within the files to reflect new service name and reassigns project guids.

    .PARAMETER NewServiceName
    New service name.

    .PARAMETER ReferenceServiceDir
    Absolute path to the reference service folder without trailing directory separator.

    .PARAMETER TargetServiceDir
    Absolute path to the target service folder without trailing directory separator.

    .EXAMPLE
    .\CopyReferenceService.ps1 -NewServiceName "CatsAndDogs" -ReferenceServiceDir "C:\Proj\Git\ReferenceService" -TargetServiceDir "c:\Proj\Git\Demo.Service.CatsAndDogs"

    Note, that you don't need to create target service directory, script will do it automatically.
    Also note, that script does not attempt to initialize git in the target dir, so you can do it yourself later by either doing git init in the
    target folder or by cloning empty master repository or by using non-git VCS for your newly created service.
#>

param(
    [Parameter(Mandatory = $true)]
    [string]
    $NewServiceName,

    [Parameter(Mandatory = $true)]
    [string]
    $ReferenceServiceDir,

    [Parameter(Mandatory = $true)]
    [string]
    $TargetServiceDir
)

function ReplaceInAllFiles([string]$oldPattern, [string]$newPattern)
{
    $matchingFiles = Get-ChildItem $TargetServiceDir -Recurse | ? { $_ | Select-String -Pattern $oldPattern }
    foreach ($matchingFile in $matchingFiles)
    {
        (Get-Content $matchingFile.FullName) | % { $_ -replace [regex]::escape($oldPattern), $newPattern } | Set-Content $matchingFile.FullName
    }
}

# Copy files and implicitly create folders
Write-Host "Renaming 'Reference' files and directories"
$referenceFiles = Get-ChildItem $ReferenceServiceDir -Recurse -Exclude ".git", "*.docx"  | where { ! $_.PSIsContainer }
$targetFiles = $referenceFiles | %{
    $_.FullName -replace [regex]::escape($ReferenceServiceDir), $TargetServiceDir -replace "Reference", $NewServiceName
}

for ($i = 0; $i -lt $referenceFiles.Length; $i++)
{
    $source = $referenceFiles[$i]
    $target = $targetFiles[$i]
    # Copy files only and rely on Copy-Items to create directory structure
    New-Item -Force $target
    Copy-Item $source.FullName $target -Recurse -Force 
}

# Rename Reference content in files
Write-Host "Renaming 'Reference' in files"
$patterns = @(
    "{0}Service",
    "{0}FD",
    "{0}WD",
    "{0}Setup",
    "{0}Client",
    "{0}Common",
    "{0}Contracts",
    "{0} Service",
    "{0} FD",
    "{0}Check",
    "{0} Front",
    "{0} Client",
    "{0}Common",
    "{0} Contracts",
    "{0}Worker"
)
foreach ($pattern in $patterns)
{
    $oldPattern = $pattern -f "Reference"
    $newPattern = $pattern -f $NewServiceName
    
    ReplaceInAllFiles $oldPattern $newPattern
}

# Reissue project GUIDs
Write-Host "Regenerating project GUIDs"
$projectFiles = Get-ChildItem $TargetServiceDir -Recurse *.csproj
$projectGuids = @{}
foreach ($projectFile in $projectFiles)
{
    (Get-Content $projectFile.FullName) | ? { $_ -match "ProjectGuid\>([^\<]+)\<" }
    if ($Matches -and $Matches.Count -eq 2)
    {
        $oldGuid = $Matches[1].Substring(1, $Matches[1].Length - 2)
        $newGuid = "{0}" -f ([Guid]::NewGuid())
        
        ReplaceInAllFiles $oldGuid $newGuid
    }
}

Write-Host "Clone is complete"
