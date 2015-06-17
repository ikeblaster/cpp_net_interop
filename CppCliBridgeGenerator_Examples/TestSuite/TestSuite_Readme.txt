"TestSuite" is an example library created for testing during development of the C++/CLI bridge generator.

It contains:
 - simple enum,
 - simple struct,
 - simple class with 4 public fields,
 - simple generic class,
 - callback function,
 - complex class, which utilizes many arrays, collections and things mentioned above.


It is fully usable in generator as it is (you can check everything and press Generate).

-----------------------------------------------------------------------------------------------------------

`client.cpp` is an example of using generated bridge. It's quite messy (actually a lot), 
but hopefully at least a bit handy to show its possibilities.

`_client_compile.bat` is prepared script for compiling `client.cpp`, but to make it work
you have to put these files next to it:

    client.cpp
    TestSuite.dll
    Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Arrays.h
    Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Simple.h
    Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Stuff.h
    Wrapper_MyCorp_TheProduct_SomeModule_Utilities_TestEnum.h
    Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Genericclass__System_String.h    (generated header files)
    Wrapper.dll, Wrapper.lib 	                                                    (compiled wrapper)



`Steps` contains 4 subfolders with different "states" of example during generation or compiling, 
including compiled version of client.cpp.

-----------------------------------------------------------------------------------------------------------

Please examine sources and don't be afraid to play around.


    2015-06-06, Ike Blaster