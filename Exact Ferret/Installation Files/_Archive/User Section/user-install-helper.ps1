"DO NOT CLOSE THIS WINDOW UNTIL INSTALLATION IS COMPLETE."

$tempDir = $env:PUBLIC + "\Temp"
mkdir $tempDir -ErrorAction SilentlyContinue | Out-Null

#Launch the admin installer
$result = (cmd /c .\Admin-InstallExactFerret.exe 2>&1)
if ($result) {
	$result = $result.ToString()
	if ($result -eq "Access is denied.") {
		exit
	}
}

#Check the output from the admin installer
$adminResult = (Get-Content "$tempDir\Exact Ferret\admin-result.txt")
if ($adminResult -ne "finished") {
	exit
}

#Get location of install dir from temp dir text file
$installDir = (Get-Content "$tempDir\Exact Ferret\installation_directory.txt")
[Environment]::SetEnvironmentVariable("EXACTFERRET", $installDir, "Process")

#Set Exact Ferret to run on startup
$regKey = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run"
Remove-ItemProperty -path $regKey -name "Exact Ferret" -ErrorAction SilentlyContinue | Out-Null
New-ItemProperty -path $regKey -name "Exact Ferret" -PropertyType String -value "`"$installDir\Exact Ferret.exe`"" | Out-Null

"Starting Exact Ferret..."
[system.diagnostics.process]::start("$installDir\Exact Ferret.exe") | Out-Null

if ($installDir) {
	#Ask if the user wants to read the help file
	$ok = Read-Host "Do you want to read the Exact Ferret help file? (y/n)"
	if ($ok -eq 'y') {
		cmd /c $installDir\ExactFerretHelp.pdf
	}

	#Ask if the user wants to configure now
	$ok = Read-Host "Do you want to configure Exact Ferret now? (y/n)"
	if ($ok -eq 'y') {
		[system.diagnostics.process]::start("$installDir\Exact Ferret.exe", "-settings") | Out-Null
	}
}