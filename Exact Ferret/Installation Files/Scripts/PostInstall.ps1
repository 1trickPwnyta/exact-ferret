Param (
	[string] $tempPath
)

$installDir = Get-Content "$tempPath\setupPath.txt"

if (Test-Path "$tempPath\openHelpAfter") {
	#cmd /c "$installDir\ExactFerretHelp.pdf"
	[System.Diagnostics.Process]::Start("http://exactferret.kangaroostandard.com/help/")
}