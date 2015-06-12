using MyCorp.TheProduct.SomeModule.Utilities;

public class Hello1
{
    static void PrintCB(string str) {
        //cout << msclr::interop::marshal_as<std::string>(str).c_str() << endl << endl;
        // cout << (const char*) Marshal::StringToHGlobalAnsi(str).ToPointer() << endl << endl;
    }
    
    public static void Main()
    {
        Arrays api = new Arrays("");
        
        for(int i=0; i < 10000000; i++) {
            api.GetObject();
        }
    }
}