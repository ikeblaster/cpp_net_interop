#using "YahooAPI.dll"
#include <iostream>
#include <msclr\auto_gcroot.h>
 
using namespace System::Runtime::InteropServices; // Marshal
using namespace std;


void PrintCB(System::String^ str) {
    cout << (const char*) Marshal::StringToHGlobalAnsi(str).ToPointer() << endl << endl;
}
 
int main()
{

    msclr::auto_gcroot<YahooAPI^> api = gcnew YahooAPI();
    
    YahooAPI::test(gcnew System::String("C++/CLI using mixed-mode"), gcnew YahooAPI::PrintCallback(PrintCB));


    cout << "MSFT stocks" << endl;
    
    cout << "Ask: " << api->GetAsk(gcnew System::String("MSFT")) << endl;
    cout << "Bid: " << api->GetBid(gcnew System::String("MSFT")) << endl;

    
    return 0;
}