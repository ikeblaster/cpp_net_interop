using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CppCliBridgeGenerator
{
    public abstract class TypeGenerator
    {
        private List<string> GeneratedFiles = new List<string>();
        protected String OutputFolder;



        protected TypeGenerator(String outputFolder) 
        {
            this.OutputFolder = outputFolder;
        }

        public virtual void AssemblyLoad(Assembly assembly)
        {
        }

        public virtual void EnumLoad(Type type, FieldInfo[] fields)
        {
        }

        public virtual void ClassLoad(Type type, FieldInfo[] fields, ConstructorInfo[] ctors, MethodInfo[] methods)
        {
        }

        public virtual void GeneratorFinalize()
        {
        }

        public List<string> GetGeneratedFiles()
        {
            return this.GeneratedFiles;
        }

        protected void WriteToFile(String text, String filename)
        {
            StreamWriter sw = new StreamWriter(Path.Combine(this.OutputFolder, filename));
            sw.Write(text);
            sw.Close();

            this.GeneratedFiles.Add(filename);
        }


    }
}
