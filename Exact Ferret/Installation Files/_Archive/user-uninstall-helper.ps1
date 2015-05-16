"DO NOT CLOSE THIS WINDOW UNTIL UNINSTALLATION IS COMPLETE."

#Launch the admin uninstaller
$result = (cmd /c .\Admin-UninstallExactFerret.exe 2>&1)
if ($result) {
	$result = $result.ToString()
	if ($result -eq "Access is denied.") {
		exit
	}
}

#Check the output from the admin uninstaller
$tempDir = $env:PUBLIC + "\Temp"
mkdir $tempDir -ErrorAction SilentlyContinue | Out-Null
$adminResult = (Get-Content "$tempDir\Exact Ferret\admin-result.txt")
if ($adminResult -ne "finished") {
	exit
}

"Unconfiguring Exact Ferret from running at startup..."
#Unset Exact Ferret to run on startup
$regKey = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run"
Remove-ItemProperty -path $regKey -name "Exact Ferret" -ErrorAction SilentlyContinue | Out-Null

"All done."
cmd /c pause