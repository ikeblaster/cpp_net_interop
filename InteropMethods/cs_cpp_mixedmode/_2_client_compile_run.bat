@echo off
call ..\__vcvars.bat


set prg=client
if exist "%~dp1%prg%.exe" del "%~dp1%prg%.exe"

cl /clr %prg%.cpp


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

call _3_run.bat

:eof