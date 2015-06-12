#pragma comment(lib, "YahooAPI")
#include <iostream>
#include "YahooAPI.h"
 
using namespace std;
 
int main()
{

    YahooAPI *api = new YahooAPI();
    
    cout << YahooAPI::test() << endl << endl;

    cout << "MSFT stocks" << endl;

    cout << "Ask: " << api->GetAsk("MSFT") << endl;
    cout << "Bid: " << api->GetBid("MSFT") << endl;
    
    
    return 0;
}