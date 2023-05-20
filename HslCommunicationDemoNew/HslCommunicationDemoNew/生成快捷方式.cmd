ECHO OFF
ECHO Set WshShell = Wscript.CreateObject("Wscript.Shell") >%temp%\tmp.vbs
CMD /c "ECHO ^Set MyLink = WshShell.CreateShortcut("C:\Users\lzj\Desktop\HslCommunicationDemo.lnk")" >>%temp%\tmp.vbs"
ECHO MyLink.TargetPath = "C:\Users\lzj\Desktop\HslCommunicationDemoNew\HslCommunicationDemoNew\HslCommunicationDemo.exe" >>%temp%\tmp.vbs
ECHO MyLink.Save >>%temp%\tmp.vbs
cscript /nologo %temp%\tmp.vbs
DEL /q /s %temp%\tmp.vbs 2>nul 1>nul