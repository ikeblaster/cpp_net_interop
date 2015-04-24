#using <System.dll>
#include <msclr\auto_gcroot.h>

#define _LNK __declspec(dllexport)
#include "YahooAPI.h"

using namespace System::Net; // WebClient
using namespace System::Globalization; // CultureInfo
using namespace System::Runtime::InteropServices; // Marshal
 
private ref class YahooAPIManaged
{

    private: 
        static WebClient^ webClient = gcnew WebClient();
     
        static System::String^ UrlTemplate = "http://download.finance.yahoo.com/d/quotes.csv?s={0}&f={1}";
     
    public:
        static int instanceCounter = 0;
    
        YahooAPIManaged() 
        {
            instanceCounter++;
        }    
    
        double ParseDouble(System::String^ value)
        {
            return System::Double::Parse(value->Trim(), CultureInfo::InvariantCulture);
        }
     
        array<System::String^>^ GetDataFromYahoo(System::String^ symbol, System::String^ fields)
        {
            System::String^ request = System::String::Format(UrlTemplate, symbol, fields);
     
            System::String^ rawData = webClient->DownloadString(request)->Trim();
     
            return rawData->Split(',');
        }
        
};

class YahooAPIManagedWrapper
{
    public: 
        msclr::auto_gcroot<YahooAPIManaged^> yahooAPI;
};
 
 
 
YahooAPI::YahooAPI()
{
    _private = new YahooAPIManagedWrapper();
    _private->yahooAPI = gcnew YahooAPIManaged();
}

YahooAPI::~YahooAPI()
{
    delete _private;
}
 
double YahooAPI::GetBid(const char* symbol)
{
    return _private->yahooAPI->ParseDouble(_private->yahooAPI->GetDataFromYahoo(gcnew System::String(symbol), "b")[0]);
}

double YahooAPI::GetAsk(const char* symbol)
{
    return _private->yahooAPI->ParseDouble(_private->yahooAPI->GetDataFromYahoo(gcnew System::String(symbol), "a")[0]);
}

const char* YahooAPI::GetCapitalization(const char* symbol)
{
    System::String^ managedCapi = _private->yahooAPI->GetDataFromYahoo(gcnew System::String(symbol), "j1")[0];
    
    return (const char*) Marshal::StringToHGlobalAnsi(managedCapi).ToPointer();
}

const char** YahooAPI::GetValues(const char* symbol, const char* fields)
{
    array<System::String^>^ managedValues = _private->yahooAPI->GetDataFromYahoo(gcnew System::String(symbol), gcnew System::String(fields));

    const char** unmanagedValues = new const char*[managedValues->Length];

    for (int i = 0; i < managedValues->Length; ++i)
    {
        unmanagedValues[i] = (const char*) Marshal::StringToHGlobalAnsi(managedValues[i]).ToPointer();
    }

    return unmanagedValues;
}

const char* YahooAPI::test()
{
    return (const char*) Marshal::StringToHGlobalAnsi(gcnew System::String("YahooAPI library - C++/CLI as flat API - Instances: " + YahooAPIManaged::instanceCounter)).ToPointer();
}
