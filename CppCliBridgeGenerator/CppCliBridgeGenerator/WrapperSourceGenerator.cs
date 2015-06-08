using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CppCliBridgeGenerator
{
    /// <summary>
    /// Generator for bridge source file.
    /// </summary>
    public class WrapperSourceGenerator : Generator
    {
        private readonly bool usesMarshalContext = false;
        private HashSet<Type> includedTypes;
        private StringBuilder outClass;
        private StringBuilder outHeader;

        /// <summary>
        /// Generator for bridge source file.
        /// </summary>
        /// <param name="outputFolder">Output folder.</param>
        public WrapperSourceGenerator(string outputFolder)
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
            if (typeof(MulticastDelegate).IsAssignableFrom(type))
                return;

            this.outHeader = new StringBuilder();
            this.outClass = new StringBuilder();

            this.includedTypes = new HashSet<Type>();
            this.includedTypes.Add(type);

            this.outHeader.AppendLine("#pragma once");
            this.outHeader.AppendLine("#pragma managed");

            this.outHeader.AppendLine();
            this.outHeader.AppendLine("#include \"marshaller_ext.h\"");
            this.outHeader.AppendLine();
            this.outHeader.AppendLine("#define _LNK __declspec(dllexport)");
            this.outHeader.AppendLine("#using \"" + type.Module.Name + "\""); // reference to assembly

            this.outHeader.AppendLine("#include \"" + WrapperILBridgeGenerator.GetFileName(type) + "\""); // include own IL bridge

            GenerateNamespaces(type, this.outClass); // opening namespace
            GenerateCtors(type, ctors); // constructors
            GenerateFields(type, fields); // class fields getters/setters
            GenerateMethods(type, methods); // methods
            GenerateEndNamespaces(type, this.outClass); // closing namespace

            // append class text to header
            this.outHeader.AppendLine();
            this.outHeader.Append(this.outClass);

            WriteToFile(this.outHeader.ToString(), GetFileName(type));
        }


        #region Ctors/dctors generators

        /// <summary>
        /// Generation of constructors
        /// </summary>
        /// <param name="type">Parent class.</param>
        /// <param name="ctors">Selected constructors</param>
        private void GenerateCtors(Type type, ConstructorInfo[] ctors)
        {
            var className = Utils.GetWrapperTypeNameFor(type); // bridge class name

            foreach (var ctor in ctors)
            {
                if (ctor.DeclaringType == type)
                {
                    GenerateCtor(ctor, this.outClass); // generate one constructor
                    this.outClass.AppendLine();
                }
            }

            // contrustor for valuetype classes (used for struct wrappers)
            if (type != null && type.IsValueType && ctors.Length == 0)
            {
                this.outClass.AppendLine(className + "::" + className + "() {");
                this.outClass.AppendLine("\t__IL = new " + Utils.GetWrapperILBridgeTypeNameFor(type) + ";");
                if (this.usesMarshalContext) this.outClass.AppendLine("\t__IL->__Context = gcnew marshal_context;");
                this.outClass.AppendLine("}");
                this.outClass.AppendLine();
            }

            // constructor for wrapping generated objects (with IL bridge parameter)
            this.outClass.AppendLine(className + "::" + className + "(" + Utils.GetWrapperILBridgeTypeNameFor(type) + "* IL) {");
            this.outClass.AppendLine("\t__IL = IL;");
            if (this.usesMarshalContext) this.outClass.AppendLine("\t__IL->__Context = gcnew marshal_context;");
            this.outClass.AppendLine("}");
            this.outClass.AppendLine();

            // destructor
            this.outClass.AppendLine(className + "::~" + className + "() {");
            this.outClass.AppendLine("\tdelete __IL;");
            this.outClass.AppendLine("}");
        }

        /// <summary>
        /// Generation of one constructor.
        /// </summary>
        /// <param name="ctor">Constructor.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateCtor(ConstructorInfo ctor, StringBuilder builder)
        {
            // generate ctor parameters
            var parList = new List<string>();
            GenerateParametersList(ctor.GetParameters(), ref parList);

            var className = Utils.GetWrapperTypeNameFor(ctor.DeclaringType); // bridge class name

            // signature
            builder.AppendLine(className + "::" + className + "(" + string.Join(", ", parList) + ") {");

            // body
            foreach (var parameter in ctor.GetParameters())
            {
                GenerateMarshalParameter(parameter.Name, parameter.ParameterType, builder); // translate/marshal parameters
            }

            builder.AppendLine("\t__IL = new " + Utils.GetWrapperILBridgeTypeNameFor(ctor.DeclaringType) + ";"); // create IL bridge instance
            builder.Append("\t__IL->__Impl = gcnew " + Utils.GetCppCliTypeFullNameFor(ctor.DeclaringType) + "("); // create instance of managed object
            builder.Append(string.Join(", ", ctor.GetParameters().Select(par => Utils.GetLocalTempNameFor(par)))); // with parameters
            builder.AppendLine(");");

            if (this.usesMarshalContext) builder.AppendLine("\t__IL->__Context = gcnew marshal_context;"); // create marshalling context, if needed
            builder.AppendLine("}");
        }

        #endregion

        #region Fields generator

        /// <summary>
        /// Generation of class fields signatures.
        /// </summary>
        /// <param name="type">Parent type.</param>
        /// <param name="fields">Selected fields.</param>
        private void GenerateFields(Type type, FieldInfo[] fields)
        {
            foreach (var field in fields)
            {
                if (field.DeclaringType == type) // skip inherited
                {
                    this.outClass.AppendLine();
                    GenerateField(field, this.outClass); 
                }
            }
        }

        /// <summary>
        /// Generate one field - getter and setter.
        /// </summary>
        /// <param name="field">Field.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateField(FieldInfo field, StringBuilder builder)
        {
            // translate return type
            var typeTransl = TypeConverter.TranslateType(field.FieldType);
            if (typeTransl.IsWrapperRequired)
            {
                GenerateIlBridgeInclude(typeTransl.ManagedType, this.outHeader);
            }

            // GETTER
            {
                // signature
                builder.AppendLine(typeTransl.NativeType + " " + Utils.GetWrapperTypeNameFor(field.DeclaringType) + "::get_" + field.Name + "() {");

                // body
                builder.Append("\t");
                builder.Append(Utils.GetCppCliTypeFor(field.FieldType) + " " + Utils.GetLocalTempNameForReturn() + " = ");

                if (field.IsStatic)
                    builder.Append(Utils.GetCppCliTypeFullNameFor(field.DeclaringType) + "::");
                else if(field.DeclaringType != null && field.DeclaringType.IsValueType)
                    builder.Append("__IL->__Impl.");
                else
                    builder.Append("__IL->__Impl->");

                builder.AppendLine(field.Name + ";");

                // return value
                GenerateReturnVal(field.FieldType, ref typeTransl, builder);

                builder.AppendLine("}");
            }

            // SETTER
            if (!field.IsInitOnly)
            {
                builder.AppendLine();

                // signature
                builder.AppendLine("void " + Utils.GetWrapperTypeNameFor(field.DeclaringType) + "::set_" + field.Name + "(" + typeTransl.NativeType + " value) {");

                // body
                GenerateMarshalParameter("value", field.FieldType, builder);

                builder.Append("\t");

                if (field.IsStatic)
                    builder.Append(Utils.GetCppCliTypeFullNameFor(field.DeclaringType) + "::");
                else if (field.DeclaringType != null && field.DeclaringType.IsValueType)
                    builder.Append("__IL->__Impl.");
                else
                    builder.Append("__IL->__Impl->");

                builder.AppendLine(field.Name + " = " + Utils.GetLocalTempNameFor("value") + ";");

                builder.AppendLine("}");
            }
        }

        #endregion

        #region Methods generator

        /// <summary>
        /// Generation of methods.
        /// </summary>
        /// <param name="type">Parent class.</param>
        /// <param name="methods">Selected methods.</param>
        private void GenerateMethods(Type type, MethodInfo[] methods)
        {
            foreach (var method in methods)
            {
                if (method.DeclaringType == type) // skip inherited
                {
                    this.outClass.AppendLine();
                    GenerateMethod(method, this.outClass);
                }
            }
        }

        /// <summary>
        /// Generation of one method.
        /// </summary>
        /// <param name="method">Method.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateMethod(MethodInfo method, StringBuilder builder)
        {
            // translate return value type
            var retValTransl = TypeConverter.TranslateType(method.ReturnType);
            if (retValTransl.IsWrapperRequired)
            {
                GenerateIlBridgeInclude(retValTransl.ManagedType, this.outHeader); // include missing bridge
            }

            // signature
            GenerateMethod_Signature(method, ref retValTransl, builder);

            builder.AppendLine("{");

            // method body
            GenerateMethod_MarshalParameters(method, builder);

            GenerateMethod_CallManaged(method, builder);

            GenerateReturnVal(method.ReturnType, ref retValTransl, builder);

            builder.AppendLine("}");
        }

        /// <summary>
        /// Method - signature generation.
        /// </summary>
        /// <param name="method">Method.</param>
        /// <param name="retValTransl">Return value type translation.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateMethod_Signature(MethodInfo method, ref TypeConverter.TypeTranslation retValTransl, StringBuilder builder)
        {
            var parList = new List<string>();

            // method name
            builder.Append(retValTransl.NativeType + " " + Utils.GetWrapperTypeNameFor(method.DeclaringType) + "::" + method.Name + "(");

            // parameters
            GenerateParametersList(method.GetParameters(), ref parList);

            builder.Append(string.Join(", ", parList));
            builder.Append(") ");
        }

        /// <summary>
        /// Method - marshal/translate parameters.
        /// </summary>
        /// <param name="method">Method.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateMethod_MarshalParameters(MethodInfo method, StringBuilder builder)
        {
            foreach (var parameter in method.GetParameters())
            {
                GenerateMarshalParameter(parameter.Name, parameter.ParameterType, builder);
            }
        }

        /// <summary>
        /// Method - generate call of managed method.
        /// </summary>
        /// <param name="method">Method.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateMethod_CallManaged(MethodInfo method, StringBuilder builder)
        {
            builder.Append("\t");

            // prepare return variable
            if (method.ReturnType != typeof (void))
            {
                builder.Append(Utils.GetCppCliTypeFor(method.ReturnType) + " " + Utils.GetLocalTempNameForReturn() + " = ");
            }

            // call prefix
            if (method.IsStatic)
                builder.Append(Utils.GetCppCliTypeFullNameFor(method.DeclaringType) + "::");
            else
                builder.Append("__IL->__Impl->");

            // call managed method itself
            if (!method.IsSpecialName || method.DeclaringType == null)
            {
                builder.Append(method.Name + "(");
                builder.Append(string.Join(", ", method.GetParameters().Select(par => Utils.GetLocalTempNameFor(par)))); // pass parameters
                builder.AppendLine(");");
            }
            // or get/set according field (for properties)
            else
            {
                var property = method.DeclaringType.GetProperty(method.Name.Substring(4));
                builder.Append(property.Name);
                if (method.GetParameters().Length == 1) builder.Append(" = " + Utils.GetLocalTempNameFor(method.GetParameters().First()));
                builder.AppendLine(";");
            }
        }

        #endregion


        #region Helpers



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
            var parType = Utils.GetCppCliTypeFor(parameterType); // type
            var parVar = Utils.GetLocalTempNameFor(parameterName); // variable name

            if (parTypeTransl.IsMarshalingRequired)
            {
                if (parTypeTransl.IsMarshalingInContext)
                    builder.AppendLine("\t" + parType + " " + parVar + " = __IL->__Context->_marshal_as<" + parType + ">(" + parameterName + ");");
                else
                    builder.AppendLine("\t" + parType + " " + parVar + " = _marshal_as<" + parType + ">(" + parameterName + ");");
            }
            else if (parTypeTransl.IsCastRequired)
            {
                builder.AppendLine("\t" + parType + " " + parVar + " = static_cast<" + parType + ">(" + parameterName + ");");
            }
            else
            {
                builder.AppendLine("\t" + parType + " " + parVar + " = " + parameterName + ";");
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
                var retType = retValTransl.NativeType; // type
                var retVar = Utils.GetLocalTempNameForReturn(); // variable name

                if (retValTransl.IsMarshalingRequired)
                {
                    if (retValTransl.IsMarshalingInContext)
                        builder.AppendLine("\t" + retType + " " + retVar + "Marshaled = __IL->__Context->_marshal_as<" + retType + ">(" + retVar + ");");
                    else
                        builder.AppendLine("\t" + retType + " " + retVar + "Marshaled = _marshal_as<" + retType + ">(" + retVar + ");");

                    retVar += "Marshaled";
                }
                else if (retValTransl.IsCastRequired)
                {
                    builder.AppendLine("\t" + retType + " " + retVar + "Cast = static_cast<" + retType + ">(" + retVar + ");");

                    retVar += "Cast";
                }

                builder.AppendLine("\treturn " + retVar + ";");
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

        #region Helpers - public static

        /// <summary>
        /// Generation of opening namespaces
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="builder">Output StringBuilder.</param>
        public static void GenerateNamespaces(Type type, StringBuilder builder)
        {
            builder.AppendLine("namespace " + Utils.GetWrapperProjectName() + " {"); // wrapper namespace

            if (type.Namespace != null)
            {
                var namespaces = type.Namespace.Split('.');
                foreach (var ns in namespaces)
                {
                    builder.AppendLine("namespace " + ns + " {"); // class namespaces
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Generation of ending (closing) namespaces
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="builder">Output StringBuilder.</param>
        public static void GenerateEndNamespaces(Type type, StringBuilder builder)
        {
            builder.AppendLine();

            if (type.Namespace != null)
            {
                var namespaces = type.Namespace.Split('.');
                foreach (var ns in namespaces)
                {
                    builder.AppendLine("}"); // class namespaces
                }
            }
            builder.AppendLine("}"); // wrapper namespace
        }


        #endregion


        /// <summary>
        /// Gets output filename of this generator for type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Filename</returns>
        public static string GetFileName(Type type)
        {
            return Utils.GetWrapperFileNameFor(type) + ".cpp";
        }


    }
}