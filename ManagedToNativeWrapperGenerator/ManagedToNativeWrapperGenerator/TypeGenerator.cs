using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace ManagedToNativeWrapperGenerator
{
    public abstract class TypeGenerator
    {
        private List<string> generatedFiles = new List<string>();
        protected String outputFolder;



        protected TypeGenerator(String outputFolder) 
        {
            this.outputFolder = outputFolder;
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



        public abstract string GetFileNameFor(Type type);


        public List<string> GetGeneratedFiles()
        {
            return this.generatedFiles;
        }

        protected void WriteToFile(String text, String filename)
        {
            StreamWriter sw = new StreamWriter(Path.Combine(outputFolder, filename));
            sw.Write(text);
            sw.Close();

            this.generatedFiles.Add(filename);
        }


    }
}
