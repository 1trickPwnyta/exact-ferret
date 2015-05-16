Param (
	[string] $tempPath
)

$installDir = Get-Content "$tempPath\setupPath.txt"
[Environment]::SetEnvironmentVariable("EXACTFERRET", $installDir, "Process")

cmd /c start "Dummy Title" "%EXACTFERRET%\Exact Ferret Tray Icon.exe"
