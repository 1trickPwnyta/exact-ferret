Param (
	[string]$installDir
)

[Environment]::SetEnvironmentVariable("Path", ($env:Path -replace [regex]::escape(";$installDir\bin"), ""), [System.EnvironmentVariableTarget]::Machine)