using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;

namespace ManagedToNativeWrapperGenerator
{
    public class WrapperILBridgesGenerator : TypeGenerator
    {


        public WrapperILBridgesGenerator(String outputFolder)
            : base(outputFolder)
        {

        }


        public override void EnumLoad(Type type, FieldInfo[] fields)
        {
            StringBuilder builder = new StringBuilder();

            // This is a managed only h ilbridge file.
            builder.AppendLine("#pragma once");
            builder.AppendLine("#pragma managed");

            builder.AppendLine("#include \"" + WrapperHeaderGenerator.GetFileName(type) + "\"");
            builder.AppendLine();

            this.WriteToFile(builder.ToString(), GetFileName(type));
        }



        public override void ClassLoad(Type type, FieldInfo[] fields, ConstructorInfo[] ctors, MethodInfo[] methods)
        {
            StringBuilder builder = new StringBuilder();

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

            this.WriteToFile(builder.ToString(), GetFileName(type));
        }

        private bool usesMarshalContext = false;



        private void GenerateILBridgeClass(Type type, StringBuilder builder)
        {
            builder.AppendLine("class " + Utils.GetWrapperILBridgeTypeNameFor(type) + " {");
            builder.AppendLine("\tpublic:");
            builder.AppendLine("\t\tmsclr::auto_gcroot<" + Utils.GetCppCliTypeNameFor(type) + "^> __Impl;");
            if(usesMarshalContext) builder.AppendLine("\t\tmsclr::auto_gcroot<marshal_context^> __Context;");
            builder.AppendLine("};");
        }


        private void GenerateMarshalExtension(Type type, StringBuilder builder)
        {
            TypeConverter.TypeTranslation retValTransl = TypeConverter.TranslateParameterType(type);

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
            builder.AppendLine("\treturn from->__IL->__Impl.get();;");
            builder.AppendLine("}");
        }


        public static string GetFileName(Type type)
        {
            return Utils.GetWrapperILBridgeTypeNameFor(type) + ".h";
        }

        public override string GetFileNameFor(Type type)
        {
            return GetFileName(type);
        }

    }
}
