using System;
 
namespace MyCorp.TheProduct.SomeModule.Utilities
{ 
        
    public enum TestEnum
    {
        None = 0,
        DOS = 1,
        Windows = 2,
        Unix = 4,
        InfoZip1 = 8
    }    
    
    public struct Simple
    {
        public int Position;
        public bool Exists;
        public double LastValue;
    };


    public class Stuff 
    {
        public string Name = "Slartibartfass";        
        public int[] IntsProperty { get; set; }
        public int[] IntsField;
        public TestEnum lineending = TestEnum.DOS;
    }

    public class Arrays
    {
        
        public delegate string PrintCallback(string str, int i);
        
        private PrintCallback callback;
   
   
        public Arrays(string name) {
            
        }       
        public Arrays(string name, string password) {
            
        }      
        public Arrays(string[] names, int password) {
            
        }     
        public Arrays(bool cond, int[] ints) {
            
        }
        
        public string Name { get; set; }
        
        public string GetStrings1()
        {
            return String.Format("String {0}.", 1);
        } 
        
        public string[] GetStrings2()
        {
            string[] ret = new string[2];
            ret[0] = String.Format("String {0}.", 1);
            ret[1] = String.Format("String {0}.", 2);
            
            return ret;
        }  
        
        public void test(string[,] strs) {
            
        }
        
        public string[,] GetStrings3()
        {
            string[,] ret = new string[2,2];
            ret[0,0] = String.Format("String {0}.", 1);
            ret[0,1] = String.Format("String {0}.", 2);
            ret[1,0] = String.Format("String {0}.", 3);
            ret[1,1] = String.Format("String {0}.", 4);
            
            return ret;
        }         
        
        public string[][] GetStrings4()
        {
            string[][] ret = new string[3][];
            ret[0] = new string[2];
            ret[1] = new string[2];
            ret[2] = new string[2];
            
            ret[0][0] = String.Format("String {0}.", 1);
            ret[0][1] = String.Format("String {0}.", 2);
            ret[1][0] = String.Format("String {0}.", 3);
            ret[1][1] = String.Format("String {0}.", 4);
            ret[2][0] = String.Format("String {0}.", 5);
            ret[2][1] = String.Format("String {0}.", 6);
            
            return ret;
        }         
        
        public void SetInt(int i)
        {

        }       
        
        public static int[] GetInts1()
        {
            int[] ret = new int[2];
            
            ret[0] = 1;
            ret[1] = 2;
            
            return ret;
        }         
        
        public int[][] GetInts2()
        {
            int[][] ret = new int[2][];
            ret[0] = new int[2];
            ret[1] = new int[2];
            
            ret[0][0] = 1;
            ret[0][1] = 2;
            ret[1][0] = 3;
            ret[1][1] = 4;
            
            return ret;
        }     
    
        public Stuff GetObject()
        {
            return new Stuff();
        }      
        
        public Stuff[] GetObjects()
        {
            return new Stuff[]{ new Stuff(), this.SetObject(new Stuff()) };
        }     
        
        Stuff last;
        
        public Stuff SetObject(Stuff s)
        {
            last = s;
            s.Name = "Some filthy Vogon";
            return s;
        } 
                
        public Stuff SetObjects(Stuff[] s)
        {
            s[0].Name = "More filthy Vogons";
            return s[0];
        } 
        
            
        public void callbackSet(string mode, PrintCallback cb)
        {
            this.callback = cb;
        }
        
        public void callbackInvoke() 
        {
            string answer = this.callback("The answer to life the universe and everything", 42);
            if(last != null) last.Name = answer;
        }
        
        
        public Simple s;
        
        public void structSet(Simple s) 
        {
            this.s = s;
        }
        
        public Simple structGet() 
        {
            return this.s;
        }
        
                
        public void structSetPosition(int i) 
        {
            this.s.Position = i;
        }       
        
        // can't work
        public void structSetPosition(Simple s, int i) 
        {
            s.Position = i;
        }
        
        public int structGetPosition() 
        {
            return this.s.Position;
        }      
        
        public int structGetPosition(Simple s) 
        {
            return s.Position;
        } 
 

    }
    
}