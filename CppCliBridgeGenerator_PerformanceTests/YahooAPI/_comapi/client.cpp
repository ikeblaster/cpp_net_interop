#include <iostream>

#import <mscorlib.tlb> raw_interfaces_only
#import "YahooAPI.tlb" named_guids
 
using namespace std;
 
 
void PrintCB(const char* str) {
    
}
 
int main()
{


    wstring out;

    CoInitialize(0);
    {
        
        YahooAPI::_YahooAPIPtr api(__uuidof(YahooAPI::YahooAPI));
        

    for(int i=0; i < 10000000; i++) {
        api->GetStringLength("Vm0weE1GbFdiRmRWV0doVVlrZFNWMWx0ZEV0Vk1XeFpZMFZrYVUxV2NIaFZWelZyVkRGYWRGVnNhRnBXVmxsM1ZrUkdZVll4VG5OVWJIQk9VbXh3V1ZZeFdtRmhNVWw1Vkd0c1ZXSklRbTlVVjNOM1pVWmtjbFZyZEZSTlYxSklWakkxVjFZeVNsbFZiRTVWVmxaYU0xWnFSbXRYUjA1R1kwVTVWMDFFUlRGV2EyUjNWakZXZEZOc2FHaFRSVXBoV1d0YWQxTkdiSFJsUjBaVFlraENSMWRyWkRCV01rcFZZWVm0weE1GbFdiRmRWV0doVVlrZFNWMWx0ZEV0Vk1XeFpZMFZrYVUxV2NIaFZWelZyVkRGYWRGVnNhRnBXVmxsM1ZrUkdZVll4VG5OVWJIQk9VbXh3V1ZZeFdtRmhNVWw1Vkd0c1ZXSklRbTlVVjNOM1pVWmtjbFZyZEZSTlYxSklWakkxVjFZeVNsbFZiRTVWVmxaYU0xWnFSbXRYUjA1R1kwVTVWMDFFUlRGV2EyUjNWakZXZEZOc2FHaFRSVXBoV1d0YWQxTkdiSFJsUjBaVFlraENSMWRyWkRCV01rcFZZW");
    }

        
    }
    CoUninitialize();
    
    return 0;
}