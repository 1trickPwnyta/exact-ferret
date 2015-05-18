# Run Values are not removed during uninstallation!
Param (
	[string] $tempPath
)

$installDir = Get-Content "$tempPath\setupPath.txt"

Remove-ItemProperty -Path HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run -Name "Exact Ferret" -ErrorAction SilentlyContinue
New-ItemProperty -Path HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run -Name "Exact Ferret" -PropertyType String -Value "`"$installDir\Exact Ferret Tray Icon.exe`"" -ErrorAction SilentlyContinue

Remove-ItemProperty -Path HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run -Name "Exact Ferret Shortcut Reset" -ErrorAction SilentlyContinue
New-ItemProperty -Path HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run -Name "Exact Ferret Shortcut Reset" -PropertyType String -Value "`"$installDir\Exact Ferret.exe`" -r" -ErrorAction SilentlyContinue
