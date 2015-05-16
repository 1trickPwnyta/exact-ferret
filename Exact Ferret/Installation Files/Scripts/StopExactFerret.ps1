Get-Process "Exact Ferret" | %{Stop-Process $_.Id -Force}

Start-Sleep -s 1
	
#Wait for Exact Ferret to really be stopped
$sleepCounter = 0
while (Get-Process "Exact Ferret") {
	Start-Sleep -s 1
	$sleepCounter++
	if ($sleepCounter -gt 10) {
		exit
	}
}

Get-Process "Exact Ferret Tray Icon" | %{Stop-Process $_.Id -Force}

Start-Sleep -s 1
	
#Wait for Exact Ferret to really be stopped
$sleepCounter = 0
while (Get-Process "Exact Ferret Tray Icon") {
	Start-Sleep -s 1
	$sleepCounter++
	if ($sleepCounter -gt 10) {
		exit
	}
}