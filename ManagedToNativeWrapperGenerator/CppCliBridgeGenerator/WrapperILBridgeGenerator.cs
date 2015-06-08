using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CppCliBridgeGenerator
{
    /// <summary>
    /// Generator for IL bridge header file.
    /// </summary>
    public class WrapperILBridgeGenerator : TypeGenerator
    {
        private readonly bool usesMarshalContext = false;

        /// <summary>
        /// Generator for main header file.
        /// </summary>
        /// <param name="outputFolder">Output folder.</param>
        public WrapperILBridgeGenerator(string outputFolder)
            : base(outputFolder)
        {
        }


        /// <summary>
        /// Called for enums - only includes main header file, which contains enum definition.
        /// </summary>
        /// <param name="type">Type with enum.</param>
        /// <param name="fields">Selected fields of enum.</param>
        public override void EnumLoad(Type type, FieldInfo[] fields)
        {
            var builder = new StringBuilder();

            builder.AppendLine("#pragma once");
            builder.AppendLine("#pragma managed");

            builder.AppendLine("#include \"" + WrapperHeaderGenerator.GetFileName(type) + "\"");
            builder.AppendLine();

            WriteToFile(builder.ToString(), GetFileName(type));
        }

        /// <summary>
        /// Called for classes.
        /// </summary>
        /// <param name="type">Type with class.</param>
        /// <param name="fields">Selected fields.</param>
        /// <param name="ctors">Selected constructors.</param>
        /// <param name="methods">selected methods.</param>
        public override void ClassLoad(Type type, FieldInfo[] fields, ConstructorInfo[] ctors, MethodInfo[] methods)
        {
            if (typeof(MulticastDelegate).IsAssignableFrom(type)) // skip delegates (processed in another generator)
                return;

            var builder = new StringBuilder();

            builder.AppendLine("#pragma once");
            builder.AppendLine("#pragma managed");

            builder.AppendLine("#include <msclr\\auto_gcroot.h>");
            builder.AppendLine();

            builder.AppendLine("#using \"" + type.Module.Name + "\"");
            builder.AppendLine("#include \"" + WrapperHeaderGenerator.GetFileName(type) + "\"");
            builder.AppendLine();

            // IL bridge in namespaces
            WrapperSourceGenerator.GenerateNamespaces(type, builder);
            GenerateILBridgeClass(type, builder);
            WrapperSourceGenerator.GenerateEndNamespaces(type, builder);

            // marshalling extensions
            GenerateMarshalExtension(type, builder);

            WriteToFile(builder.ToString(), GetFileName(type));
        }

        #region Helpers

        /// <summary>
        /// Generation of IL bridge itself.
        /// </summary>
        /// <param name="type">Type for which should be IL bridge generated.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateILBridgeClass(Type type, StringBuilder builder)
        {
            builder.AppendLine("class " + Utils.GetWrapperILBridgeTypeNameFor(type) + " {");
            builder.AppendLine("\tpublic:");

            // wrap instance in gcroot if needed (classes), or leave it as it is (structs)
            if (type.IsValueType) 
                builder.AppendLine("\t\t" + Utils.GetCppCliTypeFullNameFor(type) + " __Impl;");
            else 
                builder.AppendLine("\t\tmsclr::auto_gcroot<" + Utils.GetCppCliTypeFullNameFor(type) + "^> __Impl;");

            // marshalling context, if needed
            if (this.usesMarshalContext) 
                builder.AppendLine("\t\tmsclr::auto_gcroot<marshal_context^> __Context;");

            builder.AppendLine("};");
        }

        /// <summary>
        /// Generation of marshalling extensions for de/encapsulation of managed object.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateMarshalExtension(Type type, StringBuilder builder)
        {
            var retValTransl = TypeConverter.TranslateType(type);

            // template - wrap instance into bridge
            builder.AppendLine();
            builder.AppendLine("template <typename TTo> ");
            builder.AppendLine("inline " + retValTransl.NativeType + " marshal_as(" + Utils.GetCppCliTypeFor(type) + " const from)");
            builder.AppendLine("{");
            builder.AppendLine("\t" + retValTransl.ILBridgeType + " bridge = new " + Utils.GetWrapperILBridgeTypeFullNameFor(retValTransl.ManagedType) + ";");
            builder.AppendLine("\t" + retValTransl.NativeType + " wrapper = new " + Utils.GetWrapperTypeFullNameFor(retValTransl.ManagedType) + "(bridge);");
            builder.AppendLine("\tbridge->__Impl = from;");
            builder.AppendLine("\treturn wrapper;");
            builder.AppendLine("}");

            // template - unwrap instance from bridge
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


        /// <summary>
        /// Gets output filename of this generator for type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Filename</returns>
        public static string GetFileName(Type type)
        {
            return Utils.GetWrapperILBridgeFileNameFor(type) + ".h";
        }


    }
}