$shortcut_name = "Exact Ferret Help"
$shortcut_target = "http://exactferret.kangaroostandard.com/help/"
$sh = new-object -com "WScript.Shell"
$p = $env:ALLUSERSPROFILE + "\Microsoft\Windows\Start Menu\Programs\Exact Ferret"
$lnk = $sh.CreateShortcut( (join-path $p $shortcut_name) + ".lnk" )  
$lnk.TargetPath = $shortcut_target 
$lnk.IconLocation = "C:\Windows\System32\url.dll" 
$lnk.Save()