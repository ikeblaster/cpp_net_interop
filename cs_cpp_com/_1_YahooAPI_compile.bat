@echo off
call "C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat"

set prg=YahooAPI
if exist "%~dp1%prg%.dll" del "%~dp1%prg%.dll"

csc /target:library %prg%.cs YahooAPI.AssemblyInfo.cs


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