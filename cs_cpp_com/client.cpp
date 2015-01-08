#include <iostream>

#import <mscorlib.tlb> raw_interfaces_only
#import "YahooAPI.tlb" named_guids
 
using namespace std;
 
 
void PrintCB(const char* str) {
    cout << str << endl << endl;
}
 
int main()
{

    CoInitialize(0);
    {
    
        YahooAPI::_YahooAPIPtr api(__uuidof(YahooAPI::YahooAPI));
        
        api->test_2("C++/CLI bridge", (long) &PrintCB);

        cout << "MSFT stocks" << endl;

        // cout << "Ask: " << api->GetAsk("MSFT") << endl;
        // cout << "Bid: " << api->GetBid("MSFT") << endl;

    }
    CoUninitialize();
    
    return 0;
}