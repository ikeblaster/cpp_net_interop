using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CppCliBridgeGenerator
{
    /// <summary>
    /// Generator for IL bridge header file for delegates.
    /// </summary>
    public class WrapperILDelegateGenerator : Generator
    {
        private HashSet<Type> includedTypes; 
        private StringBuilder outClass;
        private StringBuilder outHeader;

        /// <summary>
        /// Generator for main header file for delegates.
        /// </summary>
        /// <param name="outputFolder">Output folder.</param>
        public WrapperILDelegateGenerator(string outputFolder)
            : base(outputFolder)
        {
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
            if (!typeof(MulticastDelegate).IsAssignableFrom(type)) // accept only delegates
                return;

            this.outHeader = new StringBuilder();
            this.outClass = new StringBuilder();

            this.includedTypes = new HashSet<Type>();
            this.includedTypes.Add(type);


            this.outHeader.AppendLine("#pragma once");
            this.outHeader.AppendLine("#pragma managed");

            this.outHeader.AppendLine("#include <msclr\\auto_gcroot.h>");
            this.outHeader.AppendLine();

            this.outHeader.AppendLine("#using \"" + type.Module.Name + "\"");
            this.outHeader.AppendLine();


            // IL bridge in namespaces
            WrapperSourceGenerator.GenerateNamespaces(type, this.outClass);
            GenerateILBridgeDelegate(type, this.outClass);
            WrapperSourceGenerator.GenerateEndNamespaces(type, this.outClass);

            // marshalling extensions
            GenerateMarshalExtension(type, this.outClass);

            // append class text to header
            this.outHeader.Append(this.outClass);

            WriteToFile(this.outHeader.ToString(), GetFileName(type));
        }


        /// <summary>
        /// Generation of IL bridge for delegate - managed class with Invoke method.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateILBridgeDelegate(Type type, StringBuilder builder)
        {
            var method = type.GetMethod("Invoke"); // find Invoke method in delegate
            var nativeParList = new List<string>();

            var retTypeTransl = TypeConverter.TranslateType(method.ReturnType); // translate delegate return type
            if (retTypeTransl.IsWrapperRequired)
            {
                GenerateIlBridgeInclude(retTypeTransl.ManagedType, this.outHeader); // include so far unknown bridges
            }

            GenerateParametersList(method.GetParameters(), ref nativeParList); // generate list of delegate parameters

            string varName = "cb"; // name of callback variable
            string varCallback = retTypeTransl.NativeType + " (*" + varName + ")(" + string.Join(", ", nativeParList) + ")"; // native callback signature

            string managedPars = string.Join(", ", method.GetParameters().Select(p => Utils.GetCppCliTypeFor(p.ParameterType) + " " + p.Name)); // list of managed parameters for Invoke method

            // GENERATE MANAGED CLASS
            builder.AppendLine("ref class " + Utils.GetWrapperILBridgeTypeNameFor(type) + " {");
            builder.AppendLine("\tpublic:");
            builder.AppendLine();

            // - POINTER TO SAVED NATIVE CALLBACK
            builder.AppendLine("\t\t" + varCallback + ";"); // variable in which will be pointer to native callback
            builder.AppendLine();

            // - CONSTRUCTOR
            builder.AppendLine("\t\t" + Utils.GetWrapperILBridgeTypeNameFor(type) + "(" + varCallback + ") {"); // constructor with 1 parameter - native callback
            builder.AppendLine("\t\t\t" + "this->" + varName + " = " + varName + ";"); // saved into previous variable
            builder.AppendLine("\t\t}");
            builder.AppendLine();

            // - MANAGED INVOKE METHOD
            builder.AppendLine("\t\t" + Utils.GetCppCliTypeFor(method.ReturnType) + " Invoke" + "(" + managedPars + ") {"); // signature of managed Invoke method

            // translate/marshal all parameters to native versions
            foreach (var parameter in method.GetParameters())
            {
                GenerateMarshalParameter(parameter.Name, parameter.ParameterType, builder);
            }

            // call native callback function
            builder.Append("\t\t\t");
            if (method.ReturnType != typeof(void))
            {
                builder.Append(retTypeTransl.NativeType + " " + Utils.GetLocalTempNameForReturn() + " = ");
            }
            builder.AppendLine("this->" + varName + "(" + string.Join(", ", method.GetParameters().Select(p => Utils.GetLocalTempNameFor(p))) + ");");

            // return value (marshal/translate)
            GenerateReturnVal(method.ReturnType, ref retTypeTransl, builder);

            builder.AppendLine("\t\t}");


            // - CLOSING OF CLASS
            builder.AppendLine();
            builder.AppendLine("};");
        }


        #region Helpers

        /// <summary>
        /// Generation of marshalling extensions for encapsulation of native callback function.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateMarshalExtension(Type type, StringBuilder builder)
        {
            var method = type.GetMethod("Invoke");
            var nativeParList = new List<string>();

            // return type translation
            var retTypeTransl = TypeConverter.TranslateType(method.ReturnType);
            if (retTypeTransl.IsWrapperRequired)
            {
                GenerateIlBridgeInclude(retTypeTransl.ManagedType, this.outHeader); // include unknown bridge
            }

            // parameters of delegate
            GenerateParametersList(method.GetParameters(), ref nativeParList);

            var varCallback = retTypeTransl.NativeType + " (*from)(" + string.Join(", ", nativeParList) + ")"; // callback signature

            // template - wrap callback
            builder.AppendLine();
            builder.AppendLine("template <typename TTo> ");
            builder.AppendLine("inline " + Utils.GetCppCliTypeFor(type) + " marshal_as(" + varCallback + ")");
            builder.AppendLine("{");
            builder.AppendLine("\t" + Utils.GetWrapperILBridgeTypeFullNameFor(type) + "^ bridge = gcnew " + Utils.GetWrapperILBridgeTypeFullNameFor(type) + "(from);");
            builder.AppendLine("\treturn gcnew " + Utils.GetCppCliTypeFullNameFor(type) + "(bridge, &" + Utils.GetWrapperILBridgeTypeFullNameFor(type) + "::Invoke);");
            builder.AppendLine("}");
        }

        /// <summary>
        /// Generation of parameters for methods/constructors.
        /// </summary>
        /// <param name="parameters">Selected parameters.</param>
        /// <param name="parList">Output list of parameters.</param>
        private void GenerateParametersList(ParameterInfo[] parameters, ref List<string> parList)
        {
            foreach (var parameter in parameters)
            {
                // translate parameter
                var parTypeTransl = TypeConverter.TranslateType(parameter.ParameterType);
                if (parTypeTransl.IsWrapperRequired)
                {
                    GenerateIlBridgeInclude(parTypeTransl.ManagedType, this.outHeader); // include missing bridge
                }

                // delegates
                if (typeof(MulticastDelegate).IsAssignableFrom(parameter.ParameterType))
                {
                    var method = parameter.ParameterType.GetMethod("Invoke");
                    var delegateParList = new List<string>();

                    // return type translation
                    var retTypeTransl = TypeConverter.TranslateType(method.ReturnType);
                    if (retTypeTransl.IsWrapperRequired)
                    {
                        GenerateIlBridgeInclude(retTypeTransl.ManagedType, this.outHeader); // include missing bridge
                    }

                    // get parameters of delegate
                    GenerateParametersList(method.GetParameters(), ref delegateParList);

                    // add to output
                    parList.Add(retTypeTransl.NativeType + " (*" + parameter.Name + ")(" + string.Join(", ", delegateParList) + ")");
                }
                else // everything else
                {
                    // add to output
                    parList.Add(parTypeTransl.NativeType + " " + parameter.Name);
                }
            }
        }


        /// <summary>
        /// Generation of parameter translation/marshalling.
        /// </summary>
        /// <param name="parameterName">Variable name for parameter.</param>
        /// <param name="parameterType">Type of parameter.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateMarshalParameter(string parameterName, Type parameterType, StringBuilder builder)
        {
            var parTypeTransl = TypeConverter.TranslateType(parameterType); // type translation
            var parType = parTypeTransl.NativeType; // type
            var parVar = Utils.GetLocalTempNameFor(parameterName); // variable name

            if (parTypeTransl.IsMarshalingRequired)
            {
                if (parTypeTransl.IsMarshalingInContext)
                    builder.AppendLine("\t\t\t" + parType + " " + parVar + " = __IL->__Context->_marshal_as<" + parType + ">(" + parameterName + ");");
                else
                    builder.AppendLine("\t\t\t" + parType + " " + parVar + " = _marshal_as<" + parType + ">(" + parameterName + ");");
            }
            else if (parTypeTransl.IsCastRequired)
            {
                builder.AppendLine("\t\t\t" + parType + " " + parVar + " = static_cast<" + parType + ">(" + parameterName + ");");
            }
            else
            {
                builder.AppendLine("\t\t\t" + parType + " " + parVar + " = " + parameterName + ";");
            }
        }

        /// <summary>
        /// Generation of return value translation/marshalling.
        /// </summary>
        /// <param name="returnType">Return value type.</param>
        /// <param name="retValTransl">Return value type translation.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateReturnVal(Type returnType, ref TypeConverter.TypeTranslation retValTransl, StringBuilder builder)
        {
            if (returnType != typeof(void))
            {
                var retType = Utils.GetCppCliTypeFor(returnType); // type
                var retVar = Utils.GetLocalTempNameForReturn(); // variable name

                if (retValTransl.IsMarshalingRequired)
                {
                    if (retValTransl.IsMarshalingInContext)
                        builder.AppendLine("\t\t\t" + retType + " " + retVar + "Marshaled = __IL->__Context->_marshal_as<" + retType + ">(" + retVar + ");");
                    else
                        builder.AppendLine("\t\t\t" + retType + " " + retVar + "Marshaled = _marshal_as<" + retType + ">(" + retVar + ");");

                    retVar += "Marshaled";
                }
                else if (retValTransl.IsCastRequired)
                {
                    builder.AppendLine("\t\t\t" + retType + " " + retVar + "Cast = static_cast<" + retType + ">(" + retVar + ");");
                    retVar += "Cast";
                }

                builder.AppendLine("\t\t\treturn " + retVar + ";");
            }
        }

        /// <summary>
        /// Generates include for bridge (for specified type) if not yet included.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateIlBridgeInclude(Type type, StringBuilder builder)
        {
            var t = type;
            if (t.IsArray && t.HasElementType)
            {
                t = t.GetElementType(); // get underlying type of array
            }

            if (this.includedTypes.Contains(t)) return; // skip, if already included

            builder.AppendLine("#include \"" + WrapperILBridgeGenerator.GetFileName(t) + "\"");

            this.includedTypes.Add(t);
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