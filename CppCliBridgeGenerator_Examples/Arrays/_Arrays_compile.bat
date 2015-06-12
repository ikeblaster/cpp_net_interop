@echo off
call ..\..\..\__vcvars.bat

set prg=Arrays
if exist "%~dp1%prg%.dll" del "%~dp1%prg%.dll"

csc /doc:%prg%.xml /target:library %prg%.cs


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