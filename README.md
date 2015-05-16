# README #

### To-Do List ###
* Update the automatic update URL.
* Add the new update URL certificate to trust.

### How to Release a New Version ###
1. Update the help file.
2. Update the information in the NewVersionBox.cs.
3. Increment the DISCRETE_SOFTWARE_VERSION constant in UpdateManager.cs.
4. Adjust the software version in AssemblyInfo.cs.
5. Uncomment the code in Desktop.cs for Windows 8 (if commented)
6. Check for updates to Image Magick (use 32-bit .zip version).
7. Build the Release version of the software.
8. Build the installer file.
9. Perform testing.
10. Set the installer file at https://exactferret.kangaroostandard.com/update.
11. Set the version number at https://exactferret.kangaroostandard.com/version to match the new DISCRETE_SOFTWARE_VERSION.
12. Update the software at http://games.kangaroostandard.com with the new version.

### Test Cases ###
* Definitions
    * Basic functionality - includes opening the settings window, performing a quick run, the background process being able to download a picture and change the desktop and lock screen backgrounds
* Installation
    1. Install with all defaults
    2. Test basic functionality
    3. Uninstall
    4. Install previous version
    5. Install new version on top
    6. Ensure all traces of old version are gone, and test basic functionality
    7. Install on top of current installation
    8. Test basic functionality
    9. Uninstall
    10. Make sure all traces of Exact Ferret are gone
    11. Install
* Features
    1. Make sure all newly added features work
* Tray Icon
    1. Verify start, stop, quick run, settings
    2. Use Exit
    3. Verify all Exact Ferret processes are stopped (except settings window)
    4. Open settings window, start Exact Ferret, verify tray icon comes back
* Updates
    1. Make sure automatic updates work

### How to Add a New Program Setting ###
1. Add the setting to Settings.settings
2. If this is a new property type, add instructions for exporting and importing to PropertiesManager.export/importSettings()
3. Add a control to the form
4. Set the tooltip text for the control
5. Adjust the tab order for the controls on that tab
6. New tabs must have the focus enter event which goes to tab_Enter(), tab_Enter() must have an entry for this tab index, if validation rules are required for anything on this tab, need to mark it as visited
7. Evaulate whether validation rules are needed, add them to SettingsForm.saveAllSettings()
8. Add the property name to the appropriate section in ExportForm.cs
9. For settings which affect the background process: Map the control to the property name in SettingsForm.highlightFormByPropertyName()
10. For settings which affect the background process: Add the control to the list in SettingsForm.removeAllFormHighlighting()
11. Create getter/setter in PropertiesManager

### Information About Creating Shortcuts ###
* Use ShortcutUtil to create shortcuts with hotkeys when hotkeys are needed.
* To determine if the hotkey exists, pass the shortcut to getShortcutHotKey. If the result is 0 or -1 (not found), there is no hot key. Otherwise there is.
* Some specific hot keys:
    * Ctrl+Alt+P: 1616
    * Ctrl+Alt+Q: 1617
    * Ctrl+Alt+A: 1601
    * Ctrl+Alt+B: 1602

### How to Annotate Images ###
* Annotate the middle

        convert $ef$2311118044_24a2c2874a_o.jpg -fill white -undercolor #00000080 -pointsize 45 -gravity Center -annotate +0+0 " Bananas aplenty " $ef$2311118044_24a2c2874a_o.jpg

* Annotate the top

        convert $ef$2311118044_24a2c2874a_o.jpg -fill white -undercolor #00000080 -pointsize 45 -gravity North -annotate +0+64 " Bananas aplenty " $ef$2311118044_24a2c2874a_o.jpg


### Using Google Search Without the API (Deprecated) ###
* The following query strings can be used.
* Safe search
    * (On) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=active
    * (Off) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images
* Image size
    * (1024x768) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=isz:lt,islt:xga
    * (800x600) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=isz:lt,islt:svga
    * (640x480) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=isz:lt,islt:vga
    * (400x300) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=isz:lt,islt:qsvga
* Image ratio
    * (Tall) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=iar:t
    * (Square) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=iar:s
    * (Wide) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=iar:w
    * (Panoramic) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=iar:xw
* Image size and ratio together
    * (800x600, Wide) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=isz:lt,islt:svga,iar:w
* Colors in image
    * (Full color) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=ic:color
    * (Black and white) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=ic:gray
    * (Specific color) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=ic:specific,isc:red
        * Options for the color: red, orange, yellow, green, blue, purple, teal, pink, white, gray, black, brown
* Kind of image
    * (Face) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=itp:face
    * (Photo) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=itp:photo
    * (Clip art) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=itp:clipart
    * (Line drawing) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=itp:lineart
    * (Animated) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=itp:animated
* Image file type
    * (PNG) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=ift:png
    * (GIF) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=ift:gif
    * (BMP) as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=&safe=images&tbs=ift:bmp
* Site search
    * as_st=y&tbm=isch&hl=en&as_q=banana&as_epq=&as_oq=&as_eq=&cr=&as_sitesearch=kangaroostandard.com&safe=images