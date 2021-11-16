@ECHO OFF & TITLE Types Setup
SETLOCAL ENABLEDELAYEDEXPANSION

: Make

SET r=

FOR /f %%f IN ('DIR /b *.ico') DO SET r=!r! /resource:%%f
FOR /f %%f IN ('DIR /b ..\Panel\Vista\*.xml') DO SET r=!r! /resource:..\Panel\Vista\%%f
FOR /f %%f IN ('DIR /b ..\Panel\XP\*.cpl') DO SET r=!r! /resource:..\Panel\XP\%%f

csc2 /nologo /target:winexe /win32icon:Remove.ico /o+ /out:Remove.exe Remove.cs ..\Stuff\Options.cs ..\Stuff\Own.cs Deployment\Uninstaller.cs
csc2 /nologo /target:winexe /o+ /out:Types.Setup.exe /win32icon:Setup.ico Setup.cs ..\Stuff\TongueCombo.cs ..\Stuff\Own.cs ..\Stuff\Options.cs ..\Stuff\Version.cs /resource:..\Front\Types.exe Deployment\Installer.cs /resource:Remove.exe %r%

mt -nologo -manifest Admin.manifest -outputresource:Remove.exe;#1
mt -nologo -manifest Admin.manifest -outputresource:Types.Setup.exe;#1

PAUSE & CLS & GOTO Make