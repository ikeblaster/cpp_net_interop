@echo off
call "C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat"


set prg=client
if exist "%~dp1%prg%.exe" del "%~dp1%prg%.exe"

cl /EHa %prg%.cpp


if exist *.obj del *.obj
if exist *.res del *.res

if not exist "%~dp1%prg%.exe" (
    color 0E
    echo.
    echo.
    echo * ERROR *
    echo.
    echo.
    pause
    goto eof
)

call _4_run.bat

:eof