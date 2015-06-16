"TestSuite" is an example library created for testing during development of the C++/CLI bridge generator.

It contains:
 - simple enum,
 - simple struct,
 - simple class with 4 public fields,
 - simple generic class,
 - callback function,
 - complex class, which utilizes many arrays, collections and things mentioned above.


It is fully usable in generator as it is (you can check everything and press Generate).


`client.cpp` is an example of using generated bridge. It's quite messy (actually a lot), 
but hopefully at least a bit handy to understand how things work.


`Steps` contains 4 subfolders with different "states" of example during generation or compiling.



Please examine sources and don't be afraid to play around.







   2015-06-06, Ike Blaster