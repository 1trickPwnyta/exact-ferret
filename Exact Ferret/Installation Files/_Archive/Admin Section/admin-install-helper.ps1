function Create-Shortcut ( [string]$SourceExe, [string]$DestinationPath , [string]$Arguments = "") {
	$WshShell = New-Object -comObject WScript.Shell
	$Shortcut = $WshShell.CreateShortcut($DestinationPath)
	$Shortcut.TargetPath = $SourceExe
	if ($Arguments -ne "") {
		$Shortcut.Arguments = $Arguments
	}
	$Shortcut.Save()
}

New-PSDrive -Name HKU -PSProvider Registry -Root Registry::HKEY_USERS | Out-Null
$tempDir = $env:PUBLIC + "\Temp"
mkdir $tempDir -ErrorAction SilentlyContinue | Out-Null

"Checking system requirements..."

#Check the Windows version
$WindowsVersion = [float]([string][System.Environment]::OSVersion.Version.Major + "." + [string][System.Environment]::OSVersion.Version.Minor)
if ($WindowsVersion -lt 6.09) {
	"ERROR: Exact Ferret requires Windows 7 or higher."
	cmd /c pause
	mkdir "$tempDir\Exact Ferret" -ErrorAction SilentlyContinue | Out-Null
	cmd /c "echo quit> `"$tempDir\Exact Ferret\admin-result.txt`""
	exit
}

#Check the operating system bitness
if ([System.IntPtr]::Size -eq 4) {
	"ERROR: Exact Ferret requires a 64-bit operating system."
	cmd /c pause
	mkdir "$tempDir\Exact Ferret" -ErrorAction SilentlyContinue | Out-Null
	cmd /c "echo quit> `"$tempDir\Exact Ferret\admin-result.txt`""
	exit
}

#Check the .NET version
if ((Test-Path "HKLM:\Software\Microsoft\NET Framework Setup\NDP\v4") -eq $false) {
	"ERROR: Exact Ferret requires Microsoft .NET Framework v4.0.30319 or higher."
	cmd /c pause
	mkdir "$tempDir\Exact Ferret" -ErrorAction SilentlyContinue | Out-Null
	cmd /c "echo quit> `"$tempDir\Exact Ferret\admin-result.txt`""
	exit
}

if ($WindowsVersion -lt 6.19) {
	#Check that Image Magick is installed
	if ((cmd /c convert 2>nil) -eq $null) {
		"ERROR: You must have Image Magick installed. You can download Image Magick from http://www.imagemagick.org/script/binary-releases.php#windows"
		cmd /c pause
		mkdir "$tempDir\Exact Ferret" -ErrorAction SilentlyContinue | Out-Null
		cmd /c "echo quit> `"$tempDir\Exact Ferret\admin-result.txt`""
		exit
	}
}

"The system meets all requirements."

#Remove old versions of Exact Ferret, if any
"Checking for old versions of Exact Ferret..."
$oldDone = $false
Get-ChildItem HKU:\ | %{
	if ($oldDone -eq $false) {
		$regKey = ($_.Name -Replace "HKEY_USERS", "HKU:") + "\Software\Exact Ferret"
		$installDir = (Get-ItemProperty -path $regKey -name "installation directory" -ErrorAction SilentlyContinue)."installation directory"
		if ($installDir) {
			#Ask if the user wants to uninstall
			$ok = Read-Host "Do you want to uninstall the previous version of Exact Ferret? (y/n)"
			if ($ok -ne 'y') {
				mkdir "$tempDir\Exact Ferret" -ErrorAction SilentlyContinue | Out-Null
				cmd /c "echo quit> `"$tempDir\Exact Ferret\admin-result.txt`""
				exit
			}
			
			#Stop the Exact Ferret service
			$process = (Get-Process ExactFerret -ErrorAction SilentlyContinue)
			if ($process) {
				Stop-Process $process.Id -Force -ErrorAction SilentlyContinue
			}
			$process = (Get-Process QuickRun -ErrorAction SilentlyContinue)
			if ($process) {
				Stop-Process $process.Id -Force -ErrorAction SilentlyContinue
			}
			
			"Removing configurations from the registry..."
			#Remove the configuration options from the registry
			Get-ChildItem HKU:\ | %{
				$regKey = ($_.Name -Replace "HKEY_USERS", "HKU:") + "\Software\Exact Ferret"
				Remove-Item $regKey -ErrorAction SilentlyContinue
			}

			"Removing installed files..."
			#Remove installed files
			Remove-Item "$installDir\ExactFerretSettings.exe"
			Remove-Item "$installDir\QuickRun.exe"
			Remove-Item "$installDir\change.ps1"
			Remove-Item "$installDir\dictionary.txt"
			Remove-Item "$installDir\ExactFerret.exe"
			Remove-Item "$installDir\ExactFerretHelp.html"
			Remove-Item "$installDir\ef.version" -ErrorAction SilentlyContinue
			Remove-Item "$installDir\Uninstall.exe"
			
			$WindowsVersion = [float]([string][System.Environment]::OSVersion.Version.Major + "." + [string][System.Environment]::OSVersion.Version.Minor)
			if ($WindowsVersion -lt 6.19) {
				#Unprepare Windows 7 for login screen changes - not needed
			} else {
				#Unprepare Windows 8 for login screen changes
				Remove-Item "$installDir\Windows8.dll"
			}
			
			if ((dir "$installDir") -eq $null) {
				Remove-Item "$installDir" -recurse
			}
			
			"Removing shortcuts..."
			#Remove shortcuts
			Get-ChildItem "C:\Users" | %{
				$shortcutFolder = "C:\Users\" + $_.Name + "\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Exact Ferret"
				Remove-Item $shortcutFolder -recurse -ErrorAction SilentlyContinue
			}
			
			"Unconfiguring Exact Ferret from running at startup..."
			#Unset Exact Ferret to run on startup
			Get-ChildItem HKU:\ | %{
				$regKey = ($_.Name -Replace "HKEY_USERS", "HKU:") + "\Software\Microsoft\Windows\CurrentVersion\Run"
				Remove-ItemProperty -path $regKey -name "ExactFerret" -ErrorAction SilentlyContinue | Out-Null
			}
			
			"Exact Ferret was uninstalled."
			
			$oldDone = $true
		}
	}
}

#Get the installation directory
$installDir = $env:EXACTFERRET
if ($installDir -eq $null) {
	$programFiles = (cmd /c "echo %PROGRAMFILES%")
	$installDir = "$programFiles\Exact Ferret"
}
$ok = Read-Host "Exact Ferret will be installed to $installDir. Is this okay? (y/n)"
if ($ok -ne 'y') {
	$installDir = Read-Host "Then where do you want it?"
	New-Item -Type Directory $installDir -ErrorAction SilentlyContinue | Out-Null
	while ((Test-Path $installDir) -eq $false) {
		$installDir = Read-Host "That location doesn't exist. Where do you want it?"
		New-Item -Type Directory $installDir -ErrorAction SilentlyContinue | Out-Null
	}
}

if (Get-Process "Exact Ferret" -ErrorAction SilentlyContinue) {
	"Stopping Exact Ferret..."

	#Stop Exact Ferret
	try {
		Get-Process "Exact Ferret" | %{Stop-Process $_.Id -Force}
	} catch {
		"ERROR: Exact Ferret is unstoppable. Installation cannot continue."
		cmd /c pause
		mkdir "$tempDir\Exact Ferret" -ErrorAction SilentlyContinue | Out-Null
		cmd /c "echo quit> `"$tempDir\Exact Ferret\admin-result.txt`""
		exit
	}
	
	Start-Sleep -s 1
	
	#Wait for Exact Ferret to really be stopped
	$sleepCounter = 0
	while (Get-Process "Exact Ferret" -ErrorAction SilentlyContinue) {
		"Waiting for Exact Ferret to stop..."
		Start-Sleep -s 1
		$sleepCounter++
		if ($sleepCounter -gt 10) {
			"ERROR: Exact Ferret won't stop. Installation cannot continue."
			cmd /c pause
			mkdir "$tempDir\Exact Ferret" -ErrorAction SilentlyContinue | Out-Null
			cmd /c "echo quit> `"$tempDir\Exact Ferret\admin-result.txt`""
			exit
		}
	}
}

#Remove old files
$oldInstallDir = $env:EXACTFERRET
if ($oldInstallDir -ne $null) {
	Remove-Item "$oldInstallDir\Exact Ferret.exe"
	Remove-Item "$oldInstallDir\Exact Ferret.exe.config"
	Remove-Item "$oldInstallDir\Exact Ferret.pdb"
	Remove-Item "$oldInstallDir\dictionary.txt"
	Remove-Item "$oldInstallDir\ExactFerretHelp.pdf"
	Remove-Item "$oldInstallDir\UninstallExactFerret.exe"
	if ((dir "$oldInstallDir") -eq $null) {
		Remove-Item "$oldInstallDir" -recurse
	}
}

#Set the location of install dir in environment variable and in temp dir
[Environment]::SetEnvironmentVariable("EXACTFERRET", $installDir, "Machine")
mkdir "$tempDir\Exact Ferret" -ErrorAction SilentlyContinue | Out-Null
Set-Content "$tempDir\Exact Ferret\installation_directory.txt" $installDir

"Creating the installation directory..."

#Create the installation directory
if ((Test-Path $installDir) -eq $false) {
	New-Item -Type Directory $installDir | Out-Null
}

"Installing files..."

#Install the files
cp ".\Exact Ferret.exe" $installDir
cp ".\Exact Ferret.exe.config" $installDir
cp ".\Exact Ferret.pdb" $installDir
cp .\UninstallExactFerret.exe $installDir
if ((Test-Path "$installDir\dictionary.txt") -eq $false) {
	cp .\dictionary.txt $installDir
}
cp .\ExactFerretHelp.pdf $installDir

if ($WindowsVersion -lt 6.19) {
	#Prepare Windows 7 for login screen changes
	"Setting up Login Screen changing..."

	$dir = "C:\Windows\system32\oobe\info\backgrounds"
	if ((Test-Path $dir) -eq $false) {
		mkdir $dir | Out-Null
	}

	icacls $dir /grant 'Users:(OI)(CI)F' | Out-Null

	cmd /c "NET SHARE efw7=$dir /GRANT:Users,CHANGE" 2>&1 | Out-Null

	Set-ItemProperty "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\Background" -name "OEMBackground" -value 1 | Out-Null
}

#Create entry in Control Panel > Programs and Features
$regKey = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Exact Ferret"
Remove-Item -path $regKey -ErrorAction SilentlyContinue | Out-Null
New-Item -path $regKey | Out-Null
New-ItemProperty -path $regKey -name "DisplayName" -PropertyType String -value "Exact Ferret" | Out-Null
New-ItemProperty -path $regKey -name "UninstallString" -PropertyType String -value "`"$installDir\UninstallExactFerret.exe`"" | Out-Null
New-ItemProperty -path $regKey -name "DisplayIcon" -PropertyType String -value "`"$installDir\Exact Ferret.exe`"" | Out-Null
New-ItemProperty -path $regKey -name "Publisher" -PropertyType String -value "Kangaroo Standard" | Out-Null
$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$installDir\Exact Ferret.exe").FileVersion
New-ItemProperty -path $regKey -name "DisplayVersion" -PropertyType String -value $version | Out-Null
New-ItemProperty -path $regKey -name "NoModify" -PropertyType DWord -value 1 | Out-Null
New-ItemProperty -path $regKey -name "EstimatedSize" -PropertyType DWord -value 1439 | Out-Null

"Creating shortcuts..."

#Create shortcuts
$shortcutFolder = "C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Exact Ferret"
if (Test-Path $shortcutFolder) {
	Remove-Item $shortcutFolder -recurse
}
New-Item -Type Directory $shortcutFolder | Out-Null
Create-Shortcut "$installDir\Exact Ferret.exe" "$shortcutFolder\Exact Ferret.lnk" "-settings"
Create-Shortcut "$installDir\Exact Ferret.exe" "$shortcutFolder\Exact Ferret - Quick Run.lnk" "-quick"
Create-Shortcut "$installDir\ExactFerretHelp.pdf" "$shortcutFolder\Exact Ferret Help.lnk"
Create-Shortcut "$installDir\UninstallExactFerret.exe" "$shortcutFolder\Uninstall Exact Ferret.lnk"

"Exact Ferret has been installed."

cmd /c pause

mkdir "$tempDir\Exact Ferret" -ErrorAction SilentlyContinue | Out-Null
cmd /c "echo finished> `"$tempDir\Exact Ferret\admin-result.txt`""
