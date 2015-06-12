@echo off
call ..\..\__vcvars.bat

set prg=Arrays
if exist "%~dp1%prg%.dll" del "%~dp1%prg%.dll"

csc /o /target:library %prg%.cs Arrays.AssemblyInfo.cs


if not exist "%~dp1%prg%.dll" (
    color 0E
    echo.
    echo.
    echo * ERROR *
    echo.
    echo.
) else (
    regasm %prg%.dll /tlb:%prg%.tlb
)

echo.

pause