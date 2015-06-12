@echo off
call ..\__vcvars.bat

set prg=client
if exist "%~dp1%prg%.exe" del "%~dp1%prg%.exe"

cl /MD /EHa %prg%.cpp


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


if exist "%~dp1%prg%.exe" echo Running %prg%.exe:

echo ________________________________________________________________________________


if exist "%~dp1%prg%.exe" ( call "%~dp1%prg%.exe" ) else ( echo %~dp1%prg%.exe not found )

echo.
echo ________________________________________________________________________________


pause

:eof