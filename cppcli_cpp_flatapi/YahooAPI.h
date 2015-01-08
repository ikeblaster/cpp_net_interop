#ifndef _LNK
#define _LNK __declspec(dllimport)
#endif

class YahooAPIManagedWrapper;

class _LNK YahooAPI
{
    private:
        YahooAPIManagedWrapper* _private;

    public: 
        YahooAPI();
     
        ~YahooAPI();
     
        double GetBid(const char* symbol);
        
        double GetAsk(const char* symbol);
     
        const char* GetCapitalization(const char* symbol);
     
        const char** GetValues(const char* symbol, const char* fields);
        
        static const char* test();
};