#pragma comment(lib, "Wrapper")
#include <stdlib.h>
#include <iostream>
#include "Wrapper_ZipFile.h"
 
using namespace std;
using namespace Wrapper::Ionic::Zip;
 

int main()
{
    
    ZipFile *zip = new ZipFile();
    
    zip->AddFile(L"client.cpp");
 
    zip->Save(L"out.zip");
    
    return 0;
}