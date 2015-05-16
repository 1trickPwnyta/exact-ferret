$dir = "C:\Windows\system32\oobe\info\backgrounds"
rm "$dir\*"

cmd /c "NET SHARE efw7 /DELETE /Y"