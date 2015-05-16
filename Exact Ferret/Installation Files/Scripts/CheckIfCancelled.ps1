Param (
	[string] $tempPath
)

if ((Test-Path "$tempPath\finished") -eq $false) {
	$id = [System.Diagnostics.Process]::GetCurrentProcess().Id
	$parent = (gwmi Win32_Process -filter "processid = $id" | select ParentProcessId).ParentProcessId
	$parent = (gwmi Win32_Process -filter "processid = $parent" | select ParentProcessId).ParentProcessId
	Stop-Process $parent
}
