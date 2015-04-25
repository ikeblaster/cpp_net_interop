using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ManagedToNativeWrapperGenerator
{
    public class WrapperILBridgeGenerator : TypeGenerator
    {
        private readonly bool usesMarshalContext = false;

        public WrapperILBridgeGenerator(string outputFolder)
            : base(outputFolder)
        {
        }

        public override void EnumLoad(Type type, FieldInfo[] fields)
        {
            var builder = new StringBuilder();

            // This is a managed only h ilbridge file.
            builder.AppendLine("#pragma once");
            builder.AppendLine("#pragma managed");

            builder.AppendLine("#include \"" + WrapperHeaderGenerator.GetFileName(type) + "\"");
            builder.AppendLine();

            WriteToFile(builder.ToString(), GetFileName(type));
        }

        public override void ClassLoad(Type type, FieldInfo[] fields, ConstructorInfo[] ctors, MethodInfo[] methods)
        {
            if (typeof(MulticastDelegate).IsAssignableFrom(type))
                return;

            var builder = new StringBuilder();

            // This is a managed only h ilbridge file.
            builder.AppendLine("#pragma once");
            builder.AppendLine("#pragma managed");

            builder.AppendLine("#include <msclr\\auto_gcroot.h>");
            builder.AppendLine();

            builder.AppendLine("#using \"" + type.Module.Name + "\"");
            builder.AppendLine("#include \"" + WrapperHeaderGenerator.GetFileName(type) + "\"");
            builder.AppendLine();

            WrapperSourceGenerator.GenerateNamespaces(type, builder);
            GenerateILBridgeClass(type, builder);
            WrapperSourceGenerator.GenerateEndNamespaces(type, builder);

            GenerateMarshalExtension(type, builder);

            WriteToFile(builder.ToString(), GetFileName(type));
        }

        #region Helpers

        private void GenerateILBridgeClass(Type type, StringBuilder builder)
        {
            builder.AppendLine("class " + Utils.GetWrapperILBridgeTypeNameFor(type) + " {");
            builder.AppendLine("\tpublic:");

            if (type.IsValueType) 
                builder.AppendLine("\t\t" + Utils.GetCppCliTypeFullNameFor(type) + " __Impl;");
            else 
                builder.AppendLine("\t\tmsclr::auto_gcroot<" + Utils.GetCppCliTypeFullNameFor(type) + "^> __Impl;");

            if (this.usesMarshalContext) 
                builder.AppendLine("\t\tmsclr::auto_gcroot<marshal_context^> __Context;");

            builder.AppendLine("};");
        }


        private void GenerateMarshalExtension(Type type, StringBuilder builder)
        {
            var retValTransl = TypeConverter.TranslateParameterType(type);

            builder.AppendLine();
            builder.AppendLine("template <typename TTo> ");
            builder.AppendLine("inline " + retValTransl.NativeType + " marshal_as(" + Utils.GetCppCliTypeFor(type) + " const from)");
            builder.AppendLine("{");
            builder.AppendLine("\t" + retValTransl.ILBridgeType + " bridge = new " + Utils.GetWrapperILBridgeTypeFullNameFor(retValTransl.ManagedType) + ";");
            builder.AppendLine("\t" + retValTransl.NativeType + " wrapper = new " + Utils.GetWrapperTypeFullNameFor(retValTransl.ManagedType) + "(bridge);");
            builder.AppendLine("\tbridge->__Impl = from;");
            builder.AppendLine("\treturn wrapper;");
            builder.AppendLine("}");

            builder.AppendLine();
            builder.AppendLine("template <typename TTo> ");
            builder.AppendLine("inline " + Utils.GetCppCliTypeFor(type) + " marshal_as(" + retValTransl.NativeType + " const from)");
            builder.AppendLine("{");
            if (type.IsValueType)
                builder.AppendLine("\treturn from->__IL->__Impl;");
            else
                builder.AppendLine("\treturn from->__IL->__Impl.get();");
            builder.AppendLine("}");
        }


        #endregion



        public static string GetFileName(Type type)
        {
            return Utils.GetWrapperILBridgeFileNameFor(type) + ".h";
        }

        public override string GetFileNameFor(Type type)
        {
            return GetFileName(type);
        }
    }
}