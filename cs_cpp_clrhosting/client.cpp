#include <windows.h>

#include <metahost.h>
#pragma comment(lib, "mscoree.lib")

// Import mscorlib.tlb (Microsoft Common Language Runtime Class Library).
#import "mscorlib.tlb" raw_interfaces_only				\
    high_property_prefixes("_get","_put","_putref")		\
    rename("ReportEvent", "InteropServices_ReportEvent")
using namespace mscorlib;


#include <iostream>

using namespace std;


class _YahooAPI_static {

    public:
        ICLRMetaHost* pMetaHost;
        ICLRRuntimeInfo* pRuntimeInfo;
        
        // ICorRuntimeHost and ICLRRuntimeHost are the two CLR hosting interfaces
        // supported by CLR 4.0. Here we demo the ICorRuntimeHost interface that 
        // was provided in .NET v1.x, and is compatible with all .NET Frameworks. 
        ICorRuntimeHost* pCorRuntimeHost;
        
        _AssemblyPtr spAssembly;
        _TypePtr spType;  
        
    private:
        int inited;
        
        _YahooAPI_static() {
            inited = 0;
        }
        void init() {
            if(inited) return;
            inited = 1;
        
            HRESULT hr;

            pMetaHost = NULL;
            pRuntimeInfo = NULL;
            pCorRuntimeHost = NULL;
            spAssembly = NULL;
            spType = NULL;
            
            IUnknownPtr spAppDomainThunk = NULL;
            _AppDomainPtr spDefaultAppDomain = NULL;

            // The .NET assembly to load.
            bstr_t bstrAssemblyName("YahooAPI");

            // The .NET class to instantiate.
            bstr_t bstrClassName("YahooAPI");

            // 
            // Load and start the .NET runtime.
            // 

            wprintf(L"Load and start the .NET runtime\n");

            hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&pMetaHost));
            if (FAILED(hr))
            {
                wprintf(L"CLRCreateInstance failed w/hr 0x%08lx\n", hr);
                Cleanup();
                return;
            }

            // Get the ICLRRuntimeInfo corresponding to a particular CLR version. It 
            // supersedes CorBindToRuntimeEx with STARTUP_LOADER_SAFEMODE.
            hr = pMetaHost->GetRuntime(L"v4.0.30319", IID_PPV_ARGS(&pRuntimeInfo));
            if (FAILED(hr))
            {
                wprintf(L"ICLRMetaHost::GetRuntime failed w/hr 0x%08lx\n", hr);
                Cleanup();
                return;
            }

            // Check if the specified runtime can be loaded into the process. This 
            // method will take into account other runtimes that may already be 
            // loaded into the process and set pbLoadable to TRUE if this runtime can 
            // be loaded in an in-process side-by-side fashion. 
            BOOL fLoadable;
            hr = pRuntimeInfo->IsLoadable(&fLoadable);
            if (FAILED(hr))
            {
                wprintf(L"ICLRRuntimeInfo::IsLoadable failed w/hr 0x%08lx\n", hr);
                Cleanup();
                return;
            }

            if (!fLoadable)
            {
                wprintf(L".NET runtime cannot be loaded\n");
                Cleanup();
                return;
            }

            // Load the CLR into the current process and return a runtime interface 
            // pointer. ICorRuntimeHost and ICLRRuntimeHost are the two CLR hosting  
            // interfaces supported by CLR 4.0. Here we demo the ICorRuntimeHost 
            // interface that was provided in .NET v1.x, and is compatible with all 
            // .NET Frameworks. 
            hr = pRuntimeInfo->GetInterface(CLSID_CorRuntimeHost, IID_PPV_ARGS(&pCorRuntimeHost));
            if (FAILED(hr))
            {
                wprintf(L"ICLRRuntimeInfo::GetInterface failed w/hr 0x%08lx\n", hr);
                Cleanup();
                return;
            }

            // Start the CLR.
            hr = pCorRuntimeHost->Start();
            if (FAILED(hr))
            {
                wprintf(L"CLR failed to start w/hr 0x%08lx\n", hr);
                Cleanup();
                return;
            }

            // 
            // Load the NET assembly. Call the static method GetStringLength of the 
            // class CSSimpleObject. Instantiate the class CSSimpleObject and call 
            // its instance method ToString.
            // 


            // Get a pointer to the default AppDomain in the CLR.
            hr = pCorRuntimeHost->GetDefaultDomain(&spAppDomainThunk);
            if (FAILED(hr))
            {
                wprintf(L"ICorRuntimeHost::GetDefaultDomain failed w/hr 0x%08lx\n", hr);
                Cleanup();
                return;
            }

            hr = spAppDomainThunk->QueryInterface(IID_PPV_ARGS(&spDefaultAppDomain));
            if (FAILED(hr))
            {
                wprintf(L"Failed to get default AppDomain w/hr 0x%08lx\n", hr);
                Cleanup();
                return;
            }

            // Load the .NET assembly.
            wprintf(L"Load the assembly\n");
            hr = spDefaultAppDomain->Load_2(bstrAssemblyName, &spAssembly);
            if (FAILED(hr))
            {
                wprintf(L"Failed to load the assembly w/hr 0x%08lx\n", hr);
                Cleanup();
                return;
            }

            // Get the Type of CSSimpleObject.
            
            hr = spAssembly->GetType_2(bstrClassName, &spType);
            if (FAILED(hr))
            {
                wprintf(L"Failed to get the Type interface w/hr 0x%08lx\n", hr);
                Cleanup();
                return;
            }
        }

    public:
        void Cleanup() {
            if(!inited) return;
        
            if (pCorRuntimeHost != NULL)
            {
                wprintf(L"Unload the CLR\n");
                // Please note that after a call to Stop, the CLR cannot be 
                // reinitialized into the same process. This step is usually not 
                // necessary. You can leave the .NET runtime loaded in your process.
                //wprintf(L"Stop the .NET runtime\n");
                pCorRuntimeHost->Stop();

                pCorRuntimeHost->Release();
                pCorRuntimeHost = NULL;
            }
            if (pMetaHost != NULL)
            {
                wprintf(L"Unload the .NET host\n");
                pMetaHost->Release();
                pMetaHost = NULL;
            }
            if (pRuntimeInfo != NULL)
            {
                wprintf(L"Unload the .NET runtime info\n");
                pRuntimeInfo->Release();
                pRuntimeInfo = NULL;
            }

            
            inited = 0;
        }

        static _YahooAPI_static& getICLR(int init){
            static _YahooAPI_static* iclr = new _YahooAPI_static;
            if(init) iclr->init();
            return *iclr;
        }
};





class YahooAPI {

    private:     
        variant_t vtObject;
        
        static _YahooAPI_static ICLR() {
            return _YahooAPI_static::getICLR(1);
        }     
        
    public:
    
        YahooAPI() {
            HRESULT hr;

            // Instantiate the class.
            hr = ICLR().spAssembly->CreateInstance(bstr_t("YahooAPI"), &vtObject);
            if (FAILED(hr))
            {
                wprintf(L"Assembly::CreateInstance failed w/hr 0x%08lx\n", hr);
            }    
    
        }
        
        ~YahooAPI() {
            vtObject = NULL;
        }
      
        double GetBid(char *name) {
            HRESULT hr;

            SAFEARRAY *psaArgs = NULL;
            variant_t vtRet;            
            

            // Create a safe array to contain the arguments of the method.
            psaArgs = SafeArrayCreateVector(VT_VARIANT, 0, 1);
            LONG index = 0;
            hr = SafeArrayPutElement(psaArgs, &index, &variant_t(name));
            if (FAILED(hr))
            {
                wprintf(L"SafeArrayPutElement failed w/hr 0x%08lx\n", hr);
                return 0;
            }
                
            // Invoke the "ToString" method from the Type interface.
            hr = ICLR().spType->InvokeMember_3(bstr_t("GetBid"), static_cast<BindingFlags>(BindingFlags_InvokeMethod | BindingFlags_Instance | BindingFlags_Public), NULL, vtObject, psaArgs, &vtRet);
            if (FAILED(hr))
            {
                wprintf(L"Failed to invoke GetBid w/hr 0x%08lx\n", hr);
                return 0;
            }

            return vtRet.dblVal;
        }      
        
        double GetAsk(char *name) {
            HRESULT hr;

            SAFEARRAY *psaArgs = NULL;
            variant_t vtRet;            
            

            // Create a safe array to contain the arguments of the method.
            psaArgs = SafeArrayCreateVector(VT_VARIANT, 0, 1);
            LONG index = 0;
            hr = SafeArrayPutElement(psaArgs, &index, &variant_t(name));
            if (FAILED(hr))
            {
                wprintf(L"SafeArrayPutElement failed w/hr 0x%08lx\n", hr);
                return 0;
            }
                
            // Invoke the "ToString" method from the Type interface.
            hr = ICLR().spType->InvokeMember_3(bstr_t("GetAsk"), static_cast<BindingFlags>(BindingFlags_InvokeMethod | BindingFlags_Instance | BindingFlags_Public), NULL, vtObject, psaArgs, &vtRet);
            if (FAILED(hr))
            {
                wprintf(L"Failed to invoke GetAsk w/hr 0x%08lx\n", hr);
                return 0;
            }

            return vtRet.dblVal;
        }
        
        
        static const char* test() {
            HRESULT hr;

            SAFEARRAY *psaArgs = NULL;
            variant_t vtRet;
        
            // Call the static method of the class: 
            //   public static int GetStringLength(string str);

            // Create a safe array to contain the arguments of the method. The safe 
            // array must be created with vt = VT_VARIANT because .NET reflection 
            // expects an array of Object - VT_VARIANT. There is only one argument, 
            // so cElements = 1.
            psaArgs = SafeArrayCreateVector(VT_VARIANT, 0, 0);

            // Invoke the "GetStringLength" method from the Type interface.
            hr = ICLR().spType->InvokeMember_3(bstr_t("test"), static_cast<BindingFlags>(BindingFlags_InvokeMethod | BindingFlags_Static | BindingFlags_Public), NULL, variant_t(), psaArgs, &vtRet);
            if (FAILED(hr))
            {
                wprintf(L"Failed to invoke test w/hr 0x%08lx\n", hr);
                return NULL;
            }

            SafeArrayDestroy(psaArgs);

            return _com_util::ConvertBSTRToString(vtRet.bstrVal);
        }
        
    
        static void Cleanup() {
            _YahooAPI_static::getICLR(0).Cleanup();
        }

};



int main()
{

    YahooAPI *api = new YahooAPI();
    
    wcout << YahooAPI::test() << endl << endl;
    
    cout << "MSFT stocks" << endl;

    cout << "Ask: " << api->GetAsk("MSFT") << endl;
    cout << "Bid: " << api->GetBid("MSFT") << endl;

    return 0;
}