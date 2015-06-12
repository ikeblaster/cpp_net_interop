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
 
wstring PrintCB(wstring q, int a) {
    wcout << L"° C++ callback: \"" << q << L"\" is \"" << a << L"\"." << endl;
    return L"Name changed in C++ callback";
}

int main()
{
    Arrays* api = new Arrays(L"");
    
    Stuff* ss = new Stuff();
    ss->set_Name(L"jmeno");
    
    wcout << ss->get_Name() << endl;
    
    
    
    wcout << endl << L"Collections" << endl << "====================" << endl;
    
    vector<wstring> vals = Arrays::listGetString();
    
    for(int i=0; i<vals.size(); i++) {
        wcout << vals[i] << endl;
    }

    vals.clear();
    vals.push_back(L"tisk 1");
    vals.push_back(L"tisk 2");
    vals.push_back(L"tisk 3");
    Arrays::listSetStrings(vals);


    
    
    wcout << endl << L"Struct" << endl << "=========" << endl;
    
    Simple* s = new Simple();
    s->set_Position(12345);
    
    api->structSet(s);
    
    s->set_LastValue(321.512345674896);
    api->structSetPosition(3215);

    api->set_s(s);
    
    wcout << s->get_LastValue() << endl;
    wcout << s->get_Position() << endl;


    
    
    wcout << endl << L"Objects & callbacks" << endl << "====================" << endl;

    size_t len;
    
    vector<Stuff*> stuffs = api->GetObjects();
    
    Stuff* stuff = stuffs[0];
    Stuff* stuff2;
    
    api->callbackSet(L"", &PrintCB);
    
    
    wcout << stuff->get_Name() << endl;
    
    api->SetObjects(stuffs);

    wcout << stuff->get_Name() << endl;
    
    api->SetObject(stuff);
    
    wcout << stuff->get_Name() << endl;
    
    api->callbackInvoke();
    
    wcout << stuff->get_Name() << endl;
    
    
    
    stuff2 = api->SetObject(stuff);
    stuff->set_Name(L"ike");
    wcout << stuff2->get_Name() << endl;
    
    api->SetObject(stuff2);
    
    stuff->set_lineending(TestEnum::InfoZip1);
    
    TestEnum le = stuff2->get_lineending();
    
    wcout << le << endl;
    wcout << stuff2->get_Name() << endl; 
    
    
    
    wcout << endl << L"Generic classes" << endl << "====================" << endl;
    
    Genericclass__System_String* gcls = Arrays::GetGenericWithString();
    
    gcls->print();
    gcls->print2();

    
    return 0;
}