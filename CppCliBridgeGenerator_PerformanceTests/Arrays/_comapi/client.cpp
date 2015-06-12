#include <iostream>

#import <mscorlib.tlb> raw_interfaces_only
#import "Arrays.tlb" named_guids
 
using namespace std;
 
 

 
int main()
{


    CoInitialize(0);
    {
        
        Arrays::_ArraysPtr api(__uuidof(Arrays::Arrays));
        
        for(int i=0; i < 10000000; i++) {
            api->GetObject();
        }

        
    }
    CoUninitialize();
    
    return 0;
}