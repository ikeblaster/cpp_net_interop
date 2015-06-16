#include <iostream>

#import <mscorlib.tlb> raw_interfaces_only
#import "TestSuite.tlb" named_guids
 
using namespace std;
 
 

 
int main()
{


    CoInitialize(0);
    {
        
        TestSuite::_ArraysPtr api(__uuidof(TestSuite::Arrays));
        
        for(int i=0; i < 10000000; i++) {
            api->GetObject();
        }

        
    }
    CoUninitialize();
    
    return 0;
}