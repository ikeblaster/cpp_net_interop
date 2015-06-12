@echo off

SET PATH=c:\Program Files (x86)\Mono-3.10.0\bin\;%PATH%

set prg=client

if exist "%~dp1%prg%.bat" (
    echo Running %prg%.bat:
) else ( 
    if exist "%~dp1%prg%.exe" echo Running %prg%.exe:
)

echo ________________________________________________________________________________


if exist "%~dp1%prg%.bat" (
    call "%~dp1%prg%.bat" 
) else ( 
    if exist "%~dp1%prg%.exe" ( call "%~dp1%prg%.exe" ) else ( echo %~dp1%prg%.exe not found )
)

echo.
echo ________________________________________________________________________________


pause