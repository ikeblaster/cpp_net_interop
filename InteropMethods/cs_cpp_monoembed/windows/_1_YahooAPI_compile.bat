@echo off
call ..\..\__vcvars.bat

set prg=YahooAPI
if exist "%~dp1%prg%.dll" del "%~dp1%prg%.dll"

csc /target:library %prg%.cs


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