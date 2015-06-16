
:: SET THIS PATH TO CORRECT ONE

@set path__vcvarsall="c:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\Tools\vcvars32.bat"



:: DO NOT MODIFY BELOW.
::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

@set VS_VERSION=

@if exist %path__vcvarsall% (
    @call %path__vcvarsall%
    @set VS_VERSION="??"
)

@if "%VS_VERSION%"=="" (
   @if not "%VS130COMNTOOLS%"=="" (
       @call "%VS130COMNTOOLS%\vsvars32.bat"
       @set VS_VERSION="130"
   )
)

@if "%VS_VERSION%"=="" (
   @if not "%VS120COMNTOOLS%"=="" (
       @call "%VS120COMNTOOLS%\vsvars32.bat"
       @set VS_VERSION="120"
   )
)

@if "%VS_VERSION%"=="" (
   @if not "%VS110COMNTOOLS%"=="" (
       @call "%VS110COMNTOOLS%\vsvars32.bat"
       @set VS_VERSION="110"
   )
)

@if "%VS_VERSION%"=="" (
   @if not "%VS100COMNTOOLS%"=="" (
       @call "%VS100COMNTOOLS%\vsvars32.bat"
       @set VS_VERSION="100"
   )
)


@if "%VS_VERSION%"=="" (
    @echo Error!
    @echo.
    @echo Please set correct path to vcvarsall.bat or vsvars32.bat in file:
    @echo %~f0
    @echo.
    pause
    exit
)

