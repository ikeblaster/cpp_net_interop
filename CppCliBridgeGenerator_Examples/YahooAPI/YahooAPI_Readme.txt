"YahooAPI" is an example library created originally for testing methods of interop
between native and managed code. It uses public Yahoo Finance API for getting
informations about stocks.


It is fully usable in generator as it is (you can check everything and press Generate).

-----------------------------------------------------------------------------------------------------------

`client.cpp` is an example of using generated bridge.

`_client_compile.bat` is prepared script for compiling `client.cpp`, but to make it work
you have to put these files next to it:

    client.cpp
    YahooAPI.dll
    Wrapper_YahooAPI.h	        (generated header file)
    Wrapper.dll, Wrapper.lib    (compiled wrapper)


`Steps` contains 4 subfolders with different "states" of example during generation or compiling, 
including compiled version of client.cpp.

-----------------------------------------------------------------------------------------------------------

Please examine sources and don't be afraid to play around.


    2015-06-06, Ike Blaster