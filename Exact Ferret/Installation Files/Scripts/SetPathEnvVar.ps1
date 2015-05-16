Param (
	[string]$installDir
)

[Environment]::SetEnvironmentVariable("Path", $env:Path + ";$installDir\bin", [System.EnvironmentVariableTarget]::Machine)