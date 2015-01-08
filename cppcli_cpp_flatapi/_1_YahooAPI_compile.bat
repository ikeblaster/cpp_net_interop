@echo off
call ..\__vcvars.bat


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