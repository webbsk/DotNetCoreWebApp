$source = "E:\CCP.PTT.Nautilus.zBlueGreenReferenceSvc\src\BlueGreenServiceApplication\pkg\Debug"

$destination = "E:\CCP.PTT.Nautilus.zBlueGreenReferenceSvc\src\BlueGreenServiceApplication\pkg\bgapp.sfpkg"

 If(Test-path $destination) {Remove-item $destination}

Add-Type -assembly "system.io.compression.filesystem"

[io.compression.zipfile]::CreateFromDirectory($Source, $destination) 