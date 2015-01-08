@echo off
call ..\__vcvars.bat


set prg=YahooAPIWrapper
if exist "%~dp1%prg%.dll" del "%~dp1%prg%.dll"
if exist "%~dp1%prg%.lib" del "%~dp1%prg%.lib"

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