@echo off
call ..\__vcvars.bat

if not exist "%~dp1mono.lib" lib /nologo /def:mono.def /out:mono.lib /machine:x86

set prg=client
if exist "%~dp1%prg%.exe" del "%~dp1%prg%.exe"

cl /I "c:\Program Files (x86)\Mono-3.10.0\include\mono-2.0" %prg%.cpp


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