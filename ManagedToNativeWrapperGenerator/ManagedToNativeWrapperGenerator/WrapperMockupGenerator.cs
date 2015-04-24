using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace ManagedToNativeWrapperGenerator
{
    public class WrapperMockupGenerator : TypeGenerator
    {

        List<TypeGenerator> generatorChain;

        Dictionary<Type, bool> usedTypes = new Dictionary<Type, bool>();


        public WrapperMockupGenerator(String outputFolder, List<TypeGenerator> generatorChain)
            : base(outputFolder)
        {
            this.generatorChain = generatorChain;
        }


        public override void AssemblyLoad(Assembly assembly)
        {
        }

        public override void EnumLoad(Type type, FieldInfo[] fields)
        {
        }

        public override void ClassLoad(Type type, FieldInfo[] fields, ConstructorInfo[] ctors, MethodInfo[] methods)
        {
            this.addUsedType(type, true);

            fields.ForEach(f => this.addUsedType(f.FieldType, false));

            ctors.SelectMany(c => c.GetParameters()).ForEach(p => this.addUsedType(p.ParameterType, false));

            methods.ForEach(m => this.addUsedType(m.ReturnType, false));
            methods.SelectMany(c => c.GetParameters()).ForEach(p => this.addUsedType(p.ParameterType, false));
        }


        private void addUsedType(Type type, bool declared)
        {
            Type t = type;
            if (t.IsArray && t.HasElementType)
            {
                t = t.GetElementType();
            }

            TypeConverter.TypeTranslation trans = TypeConverter.TranslateParameterType(t);


            if (trans.isWrapperRequired && !this.usedTypes.ContainsKey(t))
                this.usedTypes.Add(t, false);

            if (declared) this.usedTypes[t] = true;
        }


        public override void GeneratorFinalize()
        {
            var notFound = this.usedTypes.Where(t => !t.Value).Select(t => t.Key).Cast<Type>();
            var chain = this.generatorChain.Where(f => f != this);

            var empFields = new FieldInfo[] { };
            var empCtors = new ConstructorInfo[] { };
            var empMethods = new MethodInfo[] { };

            foreach (var type in notFound)
            {
                foreach (var gen in chain)
                {
                    gen.AssemblyLoad(type.Assembly);

                    if (type.IsClass) gen.ClassLoad(type, empFields, empCtors, empMethods);
                    else if (type.IsEnum) gen.EnumLoad(type, type.GetFields(BindingFlags.Public | BindingFlags.Static));
                }    
            }
        }




        public override string GetFileNameFor(Type type)
        {
            return null;
        }


    }

}
