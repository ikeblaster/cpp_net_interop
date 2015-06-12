#pragma comment(lib, "Wrapper")
#include <iostream>
#include "Wrapper_YahooAPI.h"
 
using namespace std;
using namespace Wrapper;
 
void PrintCB(wstring str) {
    
}
 
int main()
{
    
    YahooAPI *api = new YahooAPI();
    
    for(int i=0; i < 10000000; i++) {
        api->test(L"hello", &PrintCB);
    }

    
    return 0;
}