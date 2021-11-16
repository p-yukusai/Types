@ECHO OFF & TITLE Types.cpl

: Make

ECHO[
ECHO ################ 32 ################
ECHO[

windres32 Applet.rc %TEMP%/32.Types.cpl.o
gcc32 Applet.c Applet.def %TEMP%/32.Types.cpl.o -shared -o 32.Types.cpl -s -Os -std=c99 -Wall -pedantic

ECHO[
ECHO ################ 64 ################
ECHO[

windres64 Applet.rc %TEMP%/64.Types.cpl.o
gcc64 Applet.c Applet.def %TEMP%/64.Types.cpl.o -shared -o 64.Types.cpl -s -Os -std=c99 -Wall -pedantic

PAUSE & CLS & GOTO Make