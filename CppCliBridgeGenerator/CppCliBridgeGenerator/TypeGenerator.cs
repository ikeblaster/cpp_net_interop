using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CppCliBridgeGenerator
{
    /// <summary>
    /// Prepared class abstract for generators.
    /// </summary>
    public abstract class TypeGenerator
    {
        private List<string> GeneratedFiles = new List<string>(); // list of generated files
        protected String OutputFolder; // set output folder

        /// <summary>
        /// New "empty" generator with set output folder.
        /// </summary>
        /// <param name="outputFolder">Output folder.</param>
        protected TypeGenerator(String outputFolder) 
        {
            this.OutputFolder = outputFolder;
        }

        /// <summary>
        /// Called for every new assembly.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        public virtual void AssemblyLoad(Assembly assembly)
        {
        }

        /// <summary>
        /// Called for enums.
        /// </summary>
        /// <param name="type">Type with enum.</param>
        /// <param name="fields">Selected fields of enum.</param>
        public virtual void EnumLoad(Type type, FieldInfo[] fields)
        {
        }

        /// <summary>
        /// Called for classes.
        /// </summary>
        /// <param name="type">Type with class.</param>
        /// <param name="fields">Selected fields.</param>
        /// <param name="ctors">Selected constructors.</param>
        /// <param name="methods">selected methods.</param>
        public virtual void ClassLoad(Type type, FieldInfo[] fields, ConstructorInfo[] ctors, MethodInfo[] methods)
        {
        }

        /// <summary>
        /// Called on generation finalization.
        /// </summary>
        public virtual void GeneratorFinalize()
        {
        }

        /// <summary>
        /// Gets list of generated files.
        /// </summary>
        /// <returns>List of files.</returns>
        public List<string> GetGeneratedFiles()
        {
            return this.GeneratedFiles;
        }

        /// <summary>
        /// Common method for writing generated content into file.
        /// Uses output path set in constructor.
        /// Adds file to collection of generated files.
        /// </summary>
        /// <param name="text">Text to be written.</param>
        /// <param name="filename">Output filename.</param>
        protected void WriteToFile(String text, String filename)
        {
            File.WriteAllText(Path.Combine(this.OutputFolder, filename), text);
            this.GeneratedFiles.Add(filename);
        }


    }
}
