@echo off
call "C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat"


set prg=YahooAPI
if exist "%~dp1%prg%.dll" del "%~dp1%prg%.dll"

cl /clr /LD %prg%.cpp


if exist *.obj del *.obj
if exist *.res del *.res

if not exist "%~dp1%prg%.dll" (
    color 0E
    echo.
    echo.
    echo * ERROR *
    echo.
    echo.
)

echo.

pause