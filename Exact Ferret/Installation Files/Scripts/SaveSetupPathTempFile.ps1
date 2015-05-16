Param (
	[string] $tempPath,
	[string] $installDir
)

Set-Content -Path "$tempPath\setupPath.txt" -Value $installDir