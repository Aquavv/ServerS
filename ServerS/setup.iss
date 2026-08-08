[Setup]
AppName=ServerPickerX
AppVersion=1.0.0
DefaultDirName={autopf}\ServerPickerX
DefaultGroupName=ServerPickerX
UninstallDisplayIcon={app}\ServerPickerX.exe
Compression=lzma2
SolidCompression=yes
OutputDir=.\Installer
OutputBaseFilename=serverS
PrivilegesRequired=admin

[Files]
Source: "bin\Release\net10.0\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\ServerPickerX"; Filename: "{app}\ServerPickerX.exe"
Name: "{autodesktop}\ServerPickerX"; Filename: "{app}\ServerPickerX.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"; Flags: checkedonce

[Run]
Filename: "{app}\ServerPickerX.exe"; Description: "Launch ServerPickerX"; Flags: nowait postinstall skipifsilent
