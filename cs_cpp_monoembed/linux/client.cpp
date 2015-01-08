#pragma comment(lib, "mono.lib")
#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>
#include <mono/metadata/mono-config.h>
#include <iostream>
#include <stdlib.h>

using namespace std;


class _YahooAPI_static {

    public:
        MonoDomain* domain;
        MonoAssembly* assembly; 
        MonoImage* image;
        MonoClass* klass;
        
    private:
        bool inited;
        bool destroyed;
        
        _YahooAPI_static() {
            inited = false;
            destroyed = false;
            
            domain = NULL;
            assembly = NULL;
            image = NULL;
            klass = NULL;
        }
        
        void init() {
            if(inited || destroyed) return;
            inited = true; 
            
            // create some CLR domain
            domain = mono_jit_init("YahooAPI_domain");
            if(!domain) {
                cerr << "Error: Unable to create CLR domain" << endl;
                exit(2);
            }

            // load config - must do it or some errors may occur
            mono_domain_set_config(domain, ".", "client.config");
            mono_config_parse(NULL);

            // load YahooAPI.dll assembly
            assembly = mono_domain_assembly_open(domain, "YahooAPI.dll");
            if(!assembly) {
                cerr << "Error: Unable to load YahooAPI.dll" << endl;
                exit(2);
            }

            // load assembly image
            image = mono_assembly_get_image(assembly);            
            if(!image) {
                cerr << "Error: Unable to get assembly image from loaded dll" << endl;
                exit(2);
            }


            // get class from assembly
            klass = mono_class_from_name(image, "", "YahooAPI");  
            if(!klass) {
                cerr << "Error: Unable to find class YahooAPI in loaded assembly" << endl;
                exit(2);
            }
            
        }

    public:
        void Cleanup() {
            if(!inited) return;
        
            // cleanup CLR - afterwards you cannot init CLR again
            mono_jit_cleanup(domain);
            
            inited = false;
            destroyed = true;
        }
        
        bool isAlive() {
            return !destroyed;
        }

        static _YahooAPI_static& getICLR(int init){
            static _YahooAPI_static* iclr = new _YahooAPI_static;
            if(init) iclr->init();
            if(!iclr->isAlive()) {
                cerr << "Error: Cannot init CLR" << endl;
                exit(2);
            }
            return *iclr;
        }
};




class YahooAPI {

    private:     
        MonoObject* instance;
        
        static _YahooAPI_static ICLR() {
            return _YahooAPI_static::getICLR(1);
        }     
        
    public:
    
        YahooAPI() {
            // create instance
            instance = mono_object_new(ICLR().domain, ICLR().klass);
            // invoke default argument-less constructor
            mono_runtime_object_init(instance); 
        }
        
        ~YahooAPI() {
        }
      
        double GetBid(char *name) {
            void *args[1];
            args[0] = mono_string_new(ICLR().domain, name);

            // find method
            MonoMethodDesc* method_desc = mono_method_desc_new("YahooAPI:GetBid(string)", 0);
            MonoMethod* method = mono_method_desc_search_in_class(method_desc, ICLR().klass);
            mono_method_desc_free(method_desc);    
            
            if(!method) {
                cerr << "Error: Unable to find method YahooAPI:GetBid(string) in loaded assembly" << endl;
                exit(2);
            }

            // invoke it
            MonoObject* object = mono_runtime_invoke(method, instance, args, NULL);
            
            // get return value
            return *(double *) mono_object_unbox(object);
        }      
        
        double GetAsk(char *name) {
            void *args[1];
            args[0] = mono_string_new(ICLR().domain, name);

            // find method
            MonoMethodDesc* method_desc = mono_method_desc_new("YahooAPI:GetAsk(string)", 0);
            MonoMethod* method = mono_method_desc_search_in_class(method_desc, ICLR().klass);
            mono_method_desc_free(method_desc);    
            
            if(!method) {
                cerr << "Error: Unable to find method YahooAPI:GetAsk(string) in loaded assembly" << endl;
                exit(2);
            }

            // invoke it
            MonoObject* object = mono_runtime_invoke(method, instance, args, NULL);
            
            // get return value
            return *(double *) mono_object_unbox(object);
        }
        
        
        static const char* test() {
            // find static method
            MonoMethodDesc* method_desc = mono_method_desc_new("YahooAPI:test()", 0);
            MonoMethod* method = mono_method_desc_search_in_class(method_desc, ICLR().klass);
            mono_method_desc_free(method_desc);
            
            if(!method) {
                cerr << "Error: Unable to find method YahooAPI:test() in loaded assembly" << endl;
                exit(2);
            }

            // invoke it
            MonoObject* object = mono_runtime_invoke(method, NULL, NULL, NULL);

            // unwrap returned string
            MonoString *obj_string = mono_object_to_string(object, NULL);
            
            // warning: memory leak! Should be freed with mono_free()
            return mono_string_to_utf8(obj_string);
        }
        
    
        static void Cleanup() {
            _YahooAPI_static::getICLR(0).Cleanup();
        }

};



int main() {

    YahooAPI *api = new YahooAPI();
    
    cout << YahooAPI::test() << endl << endl;
    
    cout << "MSFT stocks" << endl;

    cout << "Ask: " << api->GetAsk("MSFT") << endl;
    cout << "Bid: " << api->GetBid("MSFT") << endl;
    
	return 0;
}
