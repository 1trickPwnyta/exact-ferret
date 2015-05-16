Param (
	[string]$installDir
)

[Environment]::SetEnvironmentVariable("EXACTFERRET", $installDir, "Machine")