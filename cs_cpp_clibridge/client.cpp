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

    // cout << "Ask: " << api->GetAsk("MSFT") << endl;
    // cout << "Bid: " << api->GetBid("MSFT") << endl;

    
    return 0;
}