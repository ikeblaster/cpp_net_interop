#pragma comment(lib, "Wrapper")
#include <iostream>
#include "Wrapper_YahooAPI.h"
 
using namespace std;
using namespace Wrapper;
 
void PrintCB(wstring str) {
    wcout << L"Callback: " << str << endl << endl;
}
 
int main()
{

    YahooAPI *api = new YahooAPI();
    
    
    wcout << YahooAPI::test() << endl;
    YahooAPI::test(L"C++/CLI bridge", &PrintCB);

    
    
    wcout << L"GOOG stocks info:" << endl;
    
    vector<wstring> vals = api->GetValues(L"GOOG", L"abcde");
    
    for(int i=0; i<vals.size(); i++) {
        wcout << L"\t" << vals[i] << endl;
    }
    

    wcout << endl;
    wcout << L"GOOG ask: " << api->GetAsk(L"GOOG") << endl;
    wcout << L"MSFT bid: " << api->GetBid(L"MSFT") << endl;

    
    return 0;
}