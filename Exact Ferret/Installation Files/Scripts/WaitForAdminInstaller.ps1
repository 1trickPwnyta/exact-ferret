Start-Sleep -s 1
while (Get-Process "InstallExactFerret-admin") {
	Start-Sleep -s 1
}
