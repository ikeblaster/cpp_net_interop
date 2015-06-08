
:: SET THIS PATH TO CORRECT ONE

@set path__vcvarsall="C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat"



:: DO NOT MODIFY BELOW.
::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

@if exist %path__vcvarsall% (
    @call %path__vcvarsall%
) else (
    @echo Error!
    @echo.
    @echo Please set correct path to vcvarsall.bat in file:
    @echo %~f0
    @echo.
    pause
    exit
)

