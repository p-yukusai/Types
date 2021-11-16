@ECHO OFF & TITLE Types Front
SETLOCAL ENABLEDELAYEDEXPANSION

: Make

SET r=
FOR /f %%f IN ('DIR /b Skin') DO SET r=!r! /resource:Skin\%%f
csc2 /nologo /target:winexe /o+ /out:Types.exe /win32icon:Skin/Types.ico /recurse:*.cs /recurse:..\Type\*.cs /recurse:..\Stuff\*.cs %r%

PAUSE & CLS & GOTO Make