New-PSDrive -Name HKU -PSProvider Registry -Root Registry::HKEY_USERS | Out-Null

if (Get-Process "Exact Ferret" -ErrorAction SilentlyContinue) {
	"Stopping Exact Ferret..."

	#Stop Exact Ferret
	try {
		Get-Process "Exact Ferret" | %{Stop-Process $_.Id -Force}
	} catch {
		"ERROR: Exact Ferret is unstoppable. Uninstallation cannot continue."
		cmd /c pause
		exit
	}
}

#Get the installation directory
$installDir = (cmd /c "echo %EXACTFERRET%")

#Remove the location of install dir from environment variables
[Environment]::SetEnvironmentVariable("EXACTFERRET", $null, "Machine")

"Removing installed files..."
#Remove installed files
Remove-Item "$installDir\Exact Ferret.exe"
Remove-Item "$installDir\Exact Ferret.exe.config"
Remove-Item "$installDir\Exact Ferret.pdb"
Remove-Item "$installDir\dictionary.txt"
Remove-Item "$installDir\ExactFerretHelp.pdf"

$WindowsVersion = [float]([string][System.Environment]::OSVersion.Version.Major + "." + [string][System.Environment]::OSVersion.Version.Minor)
if ($WindowsVersion -lt 6.19) {
	"Removing Login Screen changing..."

	$dir = "C:\Windows\system32\oobe\info\backgrounds"
	rm "$dir\*"

	cmd /c "NET SHARE efw7 /DELETE /Y" | Out-Null

	Set-ItemProperty "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\Background" -name "OEMBackground" -value 0 | Out-Null
}

#Remove entry from Control Panel > Programs and Features
$regKey = "HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\Exact Ferret"
Remove-Item -path $regKey -ErrorAction SilentlyContinue | Out-Null

"Unconfiguring Exact Ferret from running at startup..."
#Unset Exact Ferret to run on startup
Get-ChildItem HKU:\ | %{
	$regKey = ($_.Name -Replace "HKEY_USERS", "HKU:") + "\Software\Microsoft\Windows\CurrentVersion\Run"
	Remove-ItemProperty -path $regKey -name "Exact Ferret" -ErrorAction SilentlyContinue | Out-Null
}

"Removing shortcuts..."
#Remove shortcuts
$shortcutFolder = "C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Exact Ferret"
Remove-Item $shortcutFolder -recurse -ErrorAction SilentlyContinue

cmd /c start powershell "'DO NOT CLOSE THIS WINDOW.'; while (Test-Path '$installDir\UninstallExactFerret.exe') {Remove-Item '$installDir\UninstallExactFerret.exe' -ErrorAction SilentlyContinue; if ((dir '$installDir') -eq `$null) {Remove-Item '$installDir' -recurse}}"