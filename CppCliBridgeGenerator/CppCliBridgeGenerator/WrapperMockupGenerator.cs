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
        private readonly List<Generator> generatorChain;
        private readonly Dictionary<Type, bool> usedTypes = new Dictionary<Type, bool>();

        private readonly Dictionary<Type, FieldInfo[]> preselectedFields = new Dictionary<Type, FieldInfo[]>();
        private readonly Dictionary<Type, ConstructorInfo[]> preselectedCtors = new Dictionary<Type, ConstructorInfo[]>();
        private readonly Dictionary<Type, MethodInfo[]> preselectedMethods = new Dictionary<Type, MethodInfo[]>();

        /// <summary>
        /// Mockup generator - generates empty bridges using other generators (for used but not included types).
        /// </summary>
        /// <param name="outputFolder">Output folder.</param>
        /// <param name="generatorChain">Chain of all generators.</param>
        public WrapperMockupGenerator(string outputFolder, List<Generator> generatorChain)
            : base(outputFolder)
        {
            this.generatorChain = generatorChain;
        }

        /// <summary>
        /// Called for enums.
        /// </summary>
        /// <param name="type">Type with enum.</param>
        /// <param name="fields">Selected fields of enum.</param>
        public override void EnumLoad(Type type, FieldInfo[] fields)
        {
            AddUsedType(type, true);

            fields.ForEach(f => AddUsedType(f.FieldType, false)); // add field types as used and NOT declared
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
            if (type.IsGenericTypeDefinition)
            {
                this.preselectedFields.Add(type, fields);
                this.preselectedCtors.Add(type, ctors);
                this.preselectedMethods.Add(type, methods);
                return;
            }

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
                AddUsedType(method.ReturnType, false); // add delegate return type
                method.GetParameters().ForEach(p => AddUsedType(p.ParameterType, false)); // add delegate parameter types
            }

            if (type.HasElementType)
            {
                AddUsedType(type.GetElementType(), false); // add underlying element type - arrays
                return;
            }

            if (type.IsArray)
                return;

            if (TypeConverter.IsMarshalledCollection(type)) // collection element type
            {
                AddUsedType(type.GetGenericArguments()[0], false);
                return;
            }

            // translate type
            var trans = TypeConverter.TranslateType(type);
            if (!trans.IsWrapperRequired) // add only types, which needs wrapper/bridge
                return;


            if (!this.usedTypes.ContainsKey(type)) 
                this.usedTypes.Add(type, false);

            if (declared) 
                this.usedTypes[type] = true; // mark as declared
        }

        /// <summary>
        /// Finalization - process all used and still NOT declared types and generate mockups for them.
        /// </summary>
        public override void GeneratorFinalize()
        {
            var chain = this.generatorChain;

            // empty fields/ctors/methods selections = mockup
            var emptyFields = new FieldInfo[] { };
            var emptyCtors = new ConstructorInfo[] { };
            var emptyMethods = new MethodInfo[] { };

            int depth = 0;

            // repeat until something is missing to depth 5
            while (this.usedTypes.Any(t => !t.Value) && depth++ < 10)
            {
                var types = this.usedTypes.Where(t => t.Value == false).Select(t => t.Key).ToList();

                // process everything through chain
                foreach (var type in types)
                {
                    var fields = emptyFields;
                    var ctors = emptyCtors;
                    var methods = emptyMethods;

                    // get preselected values for generic types
                    if (type.IsGenericType)
                    {
                        Type genericType = type.GetGenericTypeDefinition();

                        // INFO: preselected fields for generic classes - can be problematic with generic methods
                        // find preselected fields/ctors/methods in current type - no better solution than using ToString?
                        if (this.preselectedFields.ContainsKey(genericType))
                        {
                            fields = type.GetFields().Where(a => this.preselectedCtors[genericType].Any(b => b.ToString() == a.ToString())).ToArray();
                        }
                        if (this.preselectedCtors.ContainsKey(genericType))
                        {
                            ctors = type.GetConstructors().Where(a => this.preselectedCtors[genericType].Any(b => b.ToString() == a.ToString())).ToArray();
                        }
                        if (this.preselectedMethods.ContainsKey(genericType))
                        {
                            methods = type.GetMethods().Where(a => this.preselectedMethods[genericType].Any(b => b.ToString() == a.ToString())).ToArray();
                        }
                    }

                    foreach (var gen in chain)
                    {
                        gen.AssemblyLoad(type.Assembly);

                        if (type.IsEnum)
                            gen.EnumLoad(type, type.GetFields(BindingFlags.Public | BindingFlags.Static));
                        else if (type.IsClass)
                            gen.ClassLoad(type, fields, ctors, methods);
                        else // structs
                            gen.ClassLoad(type, type.GetFields(BindingFlags.Public | BindingFlags.Static), ctors, methods);
                    }
                }
            }
            // INFO: iterations - are they even needed?
        }


    }
}