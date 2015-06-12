public class Hello1
{
    static void PrintCB(string str) {
        //cout << msclr::interop::marshal_as<std::string>(str).c_str() << endl << endl;
        // cout << (const char*) Marshal::StringToHGlobalAnsi(str).ToPointer() << endl << endl;
    }
    
    public static void Main()
    {
        YahooAPI api = new YahooAPI();
        
        for(int i=0; i < 10000000; i++) {
            api.GetStringLength("Vm0weE1GbFdiRmRWV0doVVlrZFNWMWx0ZEV0Vk1XeFpZMFZrYVUxV2NIaFZWelZyVkRGYWRGVnNhRnBXVmxsM1ZrUkdZVll4VG5OVWJIQk9VbXh3V1ZZeFdtRmhNVWw1Vkd0c1ZXSklRbTlVVjNOM1pVWmtjbFZyZEZSTlYxSklWakkxVjFZeVNsbFZiRTVWVmxaYU0xWnFSbXRYUjA1R1kwVTVWMDFFUlRGV2EyUjNWakZXZEZOc2FHaFRSVXBoV1d0YWQxTkdiSFJsUjBaVFlraENSMWRyWkRCV01rcFZZW");
        }
    }
}