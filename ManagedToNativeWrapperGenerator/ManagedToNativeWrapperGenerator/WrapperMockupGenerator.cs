using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CppCliBridgeGenerator
{
    public class WrapperMockupGenerator : TypeGenerator
    {
        private readonly List<TypeGenerator> GeneratorChain;
        private readonly Dictionary<Type, bool> UsedTypes = new Dictionary<Type, bool>();

        public WrapperMockupGenerator(string outputFolder, List<TypeGenerator> generatorChain)
            : base(outputFolder)
        {
            this.GeneratorChain = generatorChain;
        }

        public override void AssemblyLoad(Assembly assembly)
        {
        }

        public override void EnumLoad(Type type, FieldInfo[] fields)
        {
        }

        public override void ClassLoad(Type type, FieldInfo[] fields, ConstructorInfo[] ctors, MethodInfo[] methods)
        {
            AddUsedType(type, true);

            fields.ForEach(f => AddUsedType(f.FieldType, false));

            ctors.SelectMany(c => c.GetParameters()).ForEach(p => AddUsedType(p.ParameterType, false));

            methods.ForEach(m => AddUsedType(m.ReturnType, false));
            methods.SelectMany(c => c.GetParameters()).ForEach(p => AddUsedType(p.ParameterType, false));
        }

        private void AddUsedType(Type type, bool declared)
        {
            if (typeof (MulticastDelegate).IsAssignableFrom(type))
            {
                var method = type.GetMethod("Invoke");
                AddUsedType(method.ReturnType, declared);
                method.GetParameters().ForEach(p => AddUsedType(p.ParameterType, declared));
            }

            if (type.HasElementType)
            {
                AddUsedType(type.GetElementType(), declared);
                return;
            }

            if (type.IsArray)
                return;

            var trans = TypeConverter.TranslateParameterType(type);

            if (trans.IsWrapperRequired && !this.UsedTypes.ContainsKey(type))
                this.UsedTypes.Add(type, false);

            if (declared) this.UsedTypes[type] = true;
        }

        public override void GeneratorFinalize()
        {
            var notFound = this.UsedTypes.Where(t => !t.Value).Select(t => t.Key);
            var chain = this.GeneratorChain.Where(f => f != this);

            var empFields = new FieldInfo[] {};
            var empCtors = new ConstructorInfo[] {};
            var empMethods = new MethodInfo[] {};

            foreach (var type in notFound)
            {
                foreach (var gen in chain)
                {
                    gen.AssemblyLoad(type.Assembly);

                    if (type.IsEnum)
                        gen.EnumLoad(type, type.GetFields(BindingFlags.Public | BindingFlags.Static));
                    else if (type.IsClass)
                        gen.ClassLoad(type, empFields, empCtors, empMethods);
                    else
                        gen.ClassLoad(type, type.GetFields(BindingFlags.Public | BindingFlags.Static), empCtors, empMethods); // structs
                }
            } 

            // TODO: recursive?
        }


    }
}