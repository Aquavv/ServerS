[Setup]
AppId={{8B0A82A0-A39B-4B7C-80E2-93D0F6F5E300}
AppName=ServerS
AppVersion=1.0.0
DefaultDirName={autopf}\ServerS
DefaultGroupName=ServerS
UninstallDisplayIcon={app}\ServerS.exe
Compression=lzma2
SolidCompression=yes
OutputDir=.\Installer
OutputBaseFilename=serverS
SetupIconFile=Assets\favicon.ico
PrivilegesRequired=admin

[Files]
Source: "bin\Release\net10.0\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\ServerS"; Filename: "{app}\ServerS.exe"
Name: "{autodesktop}\ServerS"; Filename: "{app}\ServerS.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"; Flags: checkedonce

[Run]
Filename: "{app}\ServerS.exe"; Description: "Launch ServerS"; Flags: nowait postinstall skipifsilent
