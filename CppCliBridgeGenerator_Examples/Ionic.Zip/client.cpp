#pragma comment(lib, "Wrapper")
#include <stdlib.h>
#include <iostream>
#include "Wrapper_Ionic_Zip_ZipFile.h"
 
using namespace std;
using namespace Wrapper::Ionic::Zip;
 

int main()
{
    
    ZipFile *zip = new ZipFile();

    zip->AddFile(L"Ionic.Zip.dll");
 
    zip->Save(L"out.zip");
    
    return 0;
}