using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CppCliBridgeGenerator
{
    /// <summary>
    /// Mockup generator - generates empty bridges using other generators.
    /// </summary>
    public class WrapperMockupGenerator : Generator
    {
        private readonly List<Generator> GeneratorChain;
        private readonly Dictionary<Type, bool> UsedTypes = new Dictionary<Type, bool>();

        /// <summary>
        /// Mockup generator - generates empty bridges using other generators (for used but not included types).
        /// </summary>
        /// <param name="outputFolder">Output folder.</param>
        /// <param name="generatorChain">Chain of all generators.</param>
        public WrapperMockupGenerator(string outputFolder, List<Generator> generatorChain)
            : base(outputFolder)
        {
            this.GeneratorChain = generatorChain;
        }

        /// <summary>
        /// Called for classes. Adds every found type as used.
        /// </summary>
        /// <param name="type">Type with class.</param>
        /// <param name="fields">Selected fields.</param>
        /// <param name="ctors">Selected constructors.</param>
        /// <param name="methods">selected methods.</param>
        public override void ClassLoad(Type type, FieldInfo[] fields, ConstructorInfo[] ctors, MethodInfo[] methods)
        {
            AddUsedType(type, true); // add this class as used and declared

            fields.ForEach(f => AddUsedType(f.FieldType, false)); // add field types as used and NOT declared

            ctors.SelectMany(c => c.GetParameters()).ForEach(p => AddUsedType(p.ParameterType, false)); // add constructors parameter types as used and NOT declared

            methods.ForEach(m => AddUsedType(m.ReturnType, false)); // add method return types as used and NOT declared
            methods.SelectMany(c => c.GetParameters()).ForEach(p => AddUsedType(p.ParameterType, false)); // add method parameter types as used and NOT declared
        }


        /// <summary>
        /// Maintain usage and declaration of all types in bridge.
        /// "Declared" can only change from false to true.
        /// </summary>
        /// <param name="type">Type to be added or modified.</param>
        /// <param name="declared">Mark type as already declared.</param>
        private void AddUsedType(Type type, bool declared)
        {
            // delegates
            if (typeof (MulticastDelegate).IsAssignableFrom(type))
            {
                var method = type.GetMethod("Invoke");
                AddUsedType(method.ReturnType, declared); // add delegate return type
                method.GetParameters().ForEach(p => AddUsedType(p.ParameterType, declared)); // add delegate parameter types
            }

            if (type.HasElementType)
            {
                AddUsedType(type.GetElementType(), declared); // add underlying element type - arrays
                return;
            }

            if (type.IsArray)
                return;

            // translate type
            var trans = TypeConverter.TranslateType(type);

            if (trans.IsWrapperRequired && !this.UsedTypes.ContainsKey(type)) // add only types, which needs wrapper/bridge
                this.UsedTypes.Add(type, false);

            if (declared && this.UsedTypes.ContainsKey(type)) this.UsedTypes[type] = true; // mark as declared
        }

        /// <summary>
        /// Finalization - process all used and still NOT declared types and generate mockups for them.
        /// </summary>
        public override void GeneratorFinalize()
        {
            var notFound = this.UsedTypes.Where(t => !t.Value).Select(t => t.Key); // find not declared types
            var chain = this.GeneratorChain.Where(f => f != this); // remove this generator from chain

            // empty fields/ctors/methods selections = mockup
            var emptyFields = new FieldInfo[] {};
            var emptyCtors = new ConstructorInfo[] {};
            var emptyMethods = new MethodInfo[] {};

            // process everything through chain
            foreach (var type in notFound)
            {
                foreach (var gen in chain)
                {
                    gen.AssemblyLoad(type.Assembly);

                    if (type.IsEnum)
                        gen.EnumLoad(type, type.GetFields(BindingFlags.Public | BindingFlags.Static));
                    else if (type.IsClass)
                        gen.ClassLoad(type, emptyFields, emptyCtors, emptyMethods);
                    else
                        gen.ClassLoad(type, type.GetFields(BindingFlags.Public | BindingFlags.Static), emptyCtors, emptyMethods); // structs
                }
            } 

            // INFO: may be used recursively, if changed to include something (eg. some methods)
        }


    }
}