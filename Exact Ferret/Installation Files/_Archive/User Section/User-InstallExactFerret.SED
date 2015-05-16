[Version]
Class=IEXPRESS
SEDVersion=3
[Options]
PackagePurpose=InstallApp
ShowInstallProgramWindow=0
HideExtractAnimation=0
UseLongFileName=1
InsideCompressed=0
CAB_FixedSize=0
CAB_ResvCodeSigning=0
RebootMode=I
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
InstallPrompt=Do you want to install Exact Ferret?
DisplayLicense=
FinishMessage=
TargetName=C:\Users\jigglypuff\Documents\Visual Studio 2013\Projects\Exact Ferret\Exact Ferret\Installation Files\Dist\InstallExactFerret.exe
FriendlyName=Exact Ferret
AppLaunched=cmd /c user-install.bat
PostInstallCmd=<None>
AdminQuietInstCmd=
UserQuietInstCmd=
FILE0="user-install.bat"
FILE1="user-install-helper.ps1"
FILE2="Admin-InstallExactFerret.exe"
[SourceFiles]
SourceFiles0=C:\Users\jigglypuff\Documents\Visual Studio 2013\Projects\Exact Ferret\Exact Ferret\Installation Files\User Section\
SourceFiles1=C:\Users\jigglypuff\Documents\Visual Studio 2013\Projects\Exact Ferret\Exact Ferret\Installation Files\Admin Section\
[SourceFiles0]
%FILE0%=
%FILE1%=
[SourceFiles1]
%FILE2%=
