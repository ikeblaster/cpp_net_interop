#pragma comment(lib, "YahooAPIWrapper")
#include <iostream>
#include "YahooAPIWrapper.h"
 
using namespace std;
 
void PrintCB(const char* str) {
    cout << str << endl << endl;
}
 
int main()
{

    YahooAPIWrapper *api = new YahooAPIWrapper();
    
    YahooAPIWrapper::test("C++/CLI bridge", &PrintCB);

    cout << "MSFT stocks" << endl;
    
    const char** vals = api->GetValues("MSFT", "abcde");
    
    for(int i=0; i<5; i++) {
        cout << vals[i] << endl;
    }

    cout << "Ask: " << api->GetAsk("MSFT") << endl;
    cout << "Bid: " << api->GetBid("MSFT") << endl;

    
    return 0;
}