#pragma comment(lib, "Wrapper")
#include <stdlib.h>
#include <iostream>
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Arrays.h"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Simple.h"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Stuff.h"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_TestEnum.h"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Genericclass__System_String.h"
 
using namespace std;
using namespace Wrapper::MyCorp::TheProduct::SomeModule::Utilities;
 
wstring PrintCB(const wstring& q, int a) {
    wcout << L"° C++ callback: \"" << q << L"\" is \"" << a << L"\"." << endl;
    return L"Name changed in C++ callback";
}

int main()
{
      
    Arrays* api = new Arrays(L"");
    
    Stuff* ss = new Stuff();
    ss->SetName(L"that's me");
    
    wcout << ss->GetName() << endl;
    
    
    
    wcout << endl << L"Collections" << endl << "====================" << endl;
    
    vector<wstring> vals = Arrays::listGetString();
    
    for(int i=0; i<vals.size(); i++) {
        wcout << vals[i] << endl;
    }

    vals.clear();
    vals.push_back(L"print 1");
    vals.push_back(L"print 2");
    vals.push_back(L"print 3");
    Arrays::listSetStrings(vals);


    
    
    wcout << endl << L"Struct" << endl << "=========" << endl;
    
    Simple* s = new Simple();
    s->SetPosition(12345);
    
    api->structSet(s);
    
    s->SetLastValue(321.512345674896);
    api->structSetPosition(3215);

    api->SetS(s);
    
    wcout << s->GetLastValue() << endl;
    wcout << s->GetPosition() << endl;


    
    
    wcout << endl << L"Objects & callbacks" << endl << "====================" << endl;

    size_t len;
    
    vector<Stuff*> stuffs = api->GetObjects();
    
    Stuff* stuff = stuffs[0];
    Stuff* stuff2;
    
    api->callbackSet(L"", &PrintCB);
    
    
    wcout << stuff->GetName() << endl;
    
    api->SetObjects(stuffs);

    wcout << stuff->GetName() << endl;
    
    api->SetObject(stuff);
    
    wcout << stuff->GetName() << endl;
    
    api->callbackInvoke();
    
    wcout << stuff->GetName() << endl;
    
    
    
    stuff2 = api->SetObject(stuff);
    stuff->SetName(L"ike");
    wcout << stuff2->GetName() << endl;
    
    api->SetObject(stuff2);
    
    stuff->SetLineending(TestEnum::InfoZip1);
    
    TestEnum::TestEnumType le = stuff2->GetLineending();
    
    wcout << le << endl;
    wcout << stuff2->GetName() << endl; 
    
    
    
    
    wcout << endl << L"Generic classes" << endl << "====================" << endl;
    
    Genericclass__System_String* gcls = Arrays::GetGenericWithString();
    
    gcls->print();
    gcls->print2();

    
    
    return 0;
}