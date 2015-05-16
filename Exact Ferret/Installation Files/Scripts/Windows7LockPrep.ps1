$dir = "C:\Windows\sysnative\oobe\info\backgrounds"
New-Item -ItemType Directory $dir -ErrorAction SilentlyContinue

icacls $dir /grant 'Users:(OI)(CI)F'

$dir = "C:\Windows\system32\oobe\info\backgrounds"
cmd /c "NET SHARE efw7=$dir /GRANT:Users,CHANGE"
