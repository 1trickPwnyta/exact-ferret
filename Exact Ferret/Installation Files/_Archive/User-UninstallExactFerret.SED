[Version]
Class=IEXPRESS
SEDVersion=3
[Options]
PackagePurpose=InstallApp
ShowInstallProgramWindow=0
HideExtractAnimation=1
UseLongFileName=1
InsideCompressed=0
CAB_FixedSize=0
CAB_ResvCodeSigning=0
RebootMode=N
InstallPrompt=%InstallPrompt%
DisplayLicense=%DisplayLicense%
FinishMessage=%FinishMessage%
TargetName=%TargetName%
FriendlyName=%FriendlyName%
AppLaunched=%AppLaunched%
PostInstallCmd=%PostInstallCmd%
AdminQuietInstCmd=%AdminQuietInstCmd%
UserQuietInstCmd=%UserQuietInstCmd%
SourceFiles=SourceFiles
[Strings]
InstallPrompt=Do you want to uninstall Exact Ferret?
DisplayLicense=
FinishMessage=Exact Ferret was uninstalled.
TargetName=C:\Users\jigglypuff\Documents\Visual Studio 2013\Projects\Exact Ferret\Exact Ferret\Installation Files\_Archive\Uninstall300.exe
FriendlyName=Exact Ferret
AppLaunched=cmd /c user-uninstall.bat
PostInstallCmd=<None>
AdminQuietInstCmd=
UserQuietInstCmd=
FILE0="user-uninstall.bat"
FILE1="user-uninstall-helper.ps1"
FILE2="Admin-UninstallExactFerret.exe"
[SourceFiles]
SourceFiles0=C:\Users\jigglypuff\Documents\Visual Studio 2013\Projects\Exact Ferret\Exact Ferret\Installation Files\_Archive\
SourceFiles1=C:\Users\jigglypuff\Documents\Visual Studio 2013\Projects\Exact Ferret\Exact Ferret\Installation Files\_Archive\
Admin Section\
[SourceFiles0]
%FILE0%=
%FILE1%=
[SourceFiles1]
%FILE2%=
