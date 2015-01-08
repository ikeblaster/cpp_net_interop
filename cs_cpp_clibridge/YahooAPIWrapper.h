#ifndef _LNK
#define _LNK __declspec(dllimport)
#endif

class YahooAPIWrapperPrivate;

 
class _LNK YahooAPIWrapper
{
    private:
        YahooAPIWrapperPrivate* _private;

    public: 
        YahooAPIWrapper();
     
        ~YahooAPIWrapper();
     
        double GetBid(const char* symbol);
        
        double GetAsk(const char* symbol);
     
        const char* GetCapitalization(const char* symbol);
     
        const char** GetValues(const char* symbol, const char* fields);
        
        static const char* test();
        
        static void test(const char* mode, void (*printcb)(const char *));
};