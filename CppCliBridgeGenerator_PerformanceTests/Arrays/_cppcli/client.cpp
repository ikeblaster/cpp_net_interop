#pragma comment(lib, "Wrapper")
#include <iostream>
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Arrays.h"
 
using namespace std;
using namespace Wrapper::MyCorp::TheProduct::SomeModule::Utilities;
 
void PrintCB(wstring str) {
    
}
 
int main()
{
    
    Arrays *api = new Arrays(L"");
    

    for(int i=0; i < 10000000; i++) {
        api->GetObject();
    }

    
    return 0;
}