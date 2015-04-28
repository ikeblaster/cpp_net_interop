using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ManagedToNativeWrapperGenerator
{
    public class WrapperILDelegateGenerator : TypeGenerator
    {
        private HashSet<Type> includedTypes;

        public WrapperILDelegateGenerator(string outputFolder)
            : base(outputFolder)
        {
        }

        public override void ClassLoad(Type type, FieldInfo[] fields, ConstructorInfo[] ctors, MethodInfo[] methods)
        {
            if (!typeof(MulticastDelegate).IsAssignableFrom(type))
                return;

            this.includedTypes = new HashSet<Type>();
            this.includedTypes.Add(type);

            var builder = new StringBuilder();

            // This is a managed only h ilbridge file.
            builder.AppendLine("#pragma once");
            builder.AppendLine("#pragma managed");

            builder.AppendLine("#include <msclr\\auto_gcroot.h>");
            builder.AppendLine();

            builder.AppendLine("#using \"" + type.Module.Name + "\"");
            builder.AppendLine();

            WrapperSourceGenerator.GenerateNamespaces(type, builder);
            GenerateILBridgeDelegate(type, builder);
            WrapperSourceGenerator.GenerateEndNamespaces(type, builder);


            GenerateMarshalExtension(type, builder);

            WriteToFile(builder.ToString(), GetFileName(type));
        }

        #region Helpers

        private void GenerateILBridgeDelegate(Type type, StringBuilder builder)
        {
            // TODO: Refactor

            var method = type.GetMethod("Invoke");
            var nativeParList = new List<string>();

            var retTypeTransl = TypeConverter.TranslateParameterType(method.ReturnType);
            if (retTypeTransl.IsWrapperRequired)
            {
                GenerateIlBridgeInclude(retTypeTransl.ManagedType, builder);
            }

            GenerateParametersList(method.GetParameters(), ref nativeParList, builder);

            string varName = "cb";
            string varCallback = retTypeTransl.NativeType + " (*" + varName + ")(" + string.Join(", ", nativeParList) + ")";

            string managedPars = string.Join(", ", method.GetParameters().Select(p => Utils.GetCppCliTypeFor(p.ParameterType) + " " + p.Name));

            builder.AppendLine("ref class " + Utils.GetWrapperILBridgeTypeNameFor(type) + " {");
            builder.AppendLine("\tpublic:");
            builder.AppendLine();
            builder.AppendLine("\t\t" + varCallback + ";");
            builder.AppendLine();

            builder.AppendLine("\t\t" + Utils.GetWrapperILBridgeTypeNameFor(type) + "(" + varCallback + ") {");
            builder.AppendLine("\t\t\t" + "this->" + varName + " = " + varName + ";");
            builder.AppendLine("\t\t}");

            builder.AppendLine();

            builder.AppendLine("\t\t" + Utils.GetCppCliTypeFor(method.ReturnType) + " Invoke" + "(" + managedPars + ") {");

            foreach (var parameter in method.GetParameters())
            {
                GenerateMarshalParameter(parameter.Name, parameter.ParameterType, builder);
            }

            builder.Append("\t\t\t");
            if (method.ReturnType != typeof(void))
            {
                builder.Append(retTypeTransl.NativeType + " " + Utils.GetLocalTempNameForReturn() + " = ");
            }
            builder.AppendLine("this->" + varName + "(" + string.Join(", ", method.GetParameters().Select(p => Utils.GetLocalTempNameFor(p))) + ");");

            GenerateReturnVal(method.ReturnType, ref retTypeTransl, builder);


            builder.AppendLine("\t\t}");

            builder.AppendLine();
            builder.AppendLine("};");
        }

        private void GenerateParametersList(ParameterInfo[] parameters, ref List<string> parList, StringBuilder builder)
        {
            foreach (var parameter in parameters)
            {
                if (typeof(MulticastDelegate).IsAssignableFrom(parameter.ParameterType))
                {
                    var method = parameter.ParameterType.GetMethod("Invoke");
                    var delegateParList = new List<string>();

                    var retTypeTransl = TypeConverter.TranslateParameterType(method.ReturnType);
                    if (retTypeTransl.IsWrapperRequired)
                    {
                        GenerateIlBridgeInclude(retTypeTransl.ManagedType, builder);
                    }

                    GenerateParametersList(method.GetParameters(), ref delegateParList, builder);

                    parList.Add(retTypeTransl.NativeType + " (*" + parameter.Name + ")(" + string.Join(", ", delegateParList) + ")");
                }
                else
                {
                    var parTypeTransl = TypeConverter.TranslateParameterType(parameter.ParameterType);
                    if (parTypeTransl.IsWrapperRequired)
                    {
                        GenerateIlBridgeInclude(parTypeTransl.ManagedType, builder);
                    }

                    parList.Add(parTypeTransl.NativeType + " " + parameter.Name);
                }
            }
        }

        private void GenerateMarshalParameter(string parameterName, Type parameterType, StringBuilder builder)
        {
            var parType = Utils.GetCppCliTypeFor(parameterType);
            var parVar = Utils.GetLocalTempNameFor(parameterName);
            var typeTransl = TypeConverter.TranslateParameterType(parameterType);

            if (typeTransl.IsMarshalingRequired)
            {
                if (typeTransl.IsMarshalingInContext)
                    builder.AppendLine("\t\t\t" + typeTransl.NativeType + " " + parVar + " = __IL->__Context->_marshal_as<" + typeTransl.NativeType + ">(" + parameterName + ");");
                else
                    builder.AppendLine("\t\t\t" + typeTransl.NativeType + " " + parVar + " = _marshal_as<" + typeTransl.NativeType + ">(" + parameterName + ");");
            }
            else if (typeTransl.IsCastRequired)
            {
                builder.AppendLine("\t\t\t" + typeTransl.NativeType + " " + parVar + " = static_cast<" + parType + ">(" + parameterName + ");");
            }
            else
            {
                builder.AppendLine("\t\t\t" + typeTransl.NativeType + " " + parVar + " = " + parameterName + ";");
            }
        }

        private void GenerateReturnVal(Type returnType, ref TypeConverter.TypeTranslation retValTransl, StringBuilder builder)
        {
            //  - RETURN / MARSHAL RETURN / WRAPPED RETURN
            if (returnType != typeof(void))
            {
                var returnVar = Utils.GetLocalTempNameForReturn();

                if (retValTransl.IsMarshalingRequired)
                {
                    builder.AppendLine("\t\t\t" + Utils.GetCppCliTypeFor(returnType) + " " + returnVar + "Marshaled = _marshal_as<" + Utils.GetCppCliTypeFor(returnType) + ">(" + returnVar + ");");
                    returnVar += "Marshaled";
                }
                else if (retValTransl.IsCastRequired)
                {
                    builder.AppendLine("\t\t\t" + Utils.GetCppCliTypeFor(returnType) + " " + returnVar + "Cast = static_cast<" + Utils.GetCppCliTypeFor(returnType) + ">(" + returnVar + ");");
                    returnVar += "Cast";
                }

                builder.AppendLine("\t\t\treturn " + returnVar + ";");
            }
        }

        private void GenerateMarshalExtension(Type type, StringBuilder builder)
        {
            var method = type.GetMethod("Invoke");
            var nativeParList = new List<string>();

            var retTypeTransl = TypeConverter.TranslateParameterType(method.ReturnType);
            if (retTypeTransl.IsWrapperRequired)
            {
                GenerateIlBridgeInclude(retTypeTransl.ManagedType, builder);
            }

            GenerateParametersList(method.GetParameters(), ref nativeParList, builder);

            string varName = "from";
            string varCallback = retTypeTransl.NativeType + " (*" + varName + ")(" + string.Join(", ", nativeParList) + ")";

            builder.AppendLine();
            builder.AppendLine("template <typename TTo> ");
            builder.AppendLine("inline " + Utils.GetCppCliTypeFor(type) + " marshal_as(" + varCallback + ")");
            builder.AppendLine("{");
            builder.AppendLine("\t" + Utils.GetWrapperILBridgeTypeFullNameFor(type) + "^ bridge = gcnew " + Utils.GetWrapperILBridgeTypeFullNameFor(type) + "(from);");
            builder.AppendLine("\treturn gcnew " + Utils.GetCppCliTypeFullNameFor(type) + "(bridge, &" + Utils.GetWrapperILBridgeTypeFullNameFor(type) + "::Invoke);");
            builder.AppendLine("}");
        }

        private void GenerateIlBridgeInclude(Type type, StringBuilder builder)
        {
            var t = type;
            if (t.IsArray && t.HasElementType)
            {
                t = t.GetElementType();
            }

            if (this.includedTypes.Contains(t)) return;

            builder.AppendLine("#include \"" + WrapperILBridgeGenerator.GetFileName(t) + "\"");

            this.includedTypes.Add(t);
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