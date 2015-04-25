using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ManagedToNativeWrapperGenerator
{
    public class WrapperSourceGenerator : TypeGenerator
    {
        private readonly bool usesMarshalContext = false;
        private HashSet<Type> includedTypes;
        private StringBuilder outClass;
        private StringBuilder outHeader;

        public WrapperSourceGenerator(string outputFolder)
            : base(outputFolder)
        {
        }

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

            this.outHeader.AppendLine("#include <msclr\\auto_gcroot.h>");
            this.outHeader.AppendLine("#include <msclr\\marshal_cppstd.h>");
            this.outHeader.AppendLine("using namespace msclr::interop;");
            this.outHeader.AppendLine();
            this.outHeader.AppendLine("#include \"marshaler_ext.h\"");
            this.outHeader.AppendLine();
            this.outHeader.AppendLine("#define _LNK __declspec(dllexport)");
            this.outHeader.AppendLine("#using \"" + type.Module.Name + "\"");

            this.outHeader.AppendLine("#include \"" + WrapperILBridgeGenerator.GetFileName(type) + "\"");

            GenerateNamespaces(type, this.outClass);
            GenerateCtors(type, ctors);
            GenerateFields(type, fields);
            GenerateMethods(type, methods);
            GenerateEndNamespaces(type, this.outClass);

            // append class text to header
            this.outHeader.AppendLine();
            this.outHeader.Append(this.outClass);

            WriteToFile(this.outHeader.ToString(), GetFileName(type));
        }


        #region Ctors/dctors generators

        private void GenerateCtors(Type type, ConstructorInfo[] ctors)
        {
            var className = Utils.GetWrapperTypeNameFor(type);

            foreach (var ctor in ctors)
            {
                if (ctor.DeclaringType == type)
                {
                    GenerateCtor(ctor, this.outClass);
                    this.outClass.AppendLine();
                }
            }

            if (type.IsValueType && ctors.Length == 0)
            {
                this.outClass.AppendLine(className + "::" + className + "() {");
                this.outClass.AppendLine("\t__IL = new " + Utils.GetWrapperILBridgeTypeNameFor(type) + ";");
                if (this.usesMarshalContext) this.outClass.AppendLine("\t__IL->__Context = gcnew marshal_context;");
                this.outClass.AppendLine("}");
                this.outClass.AppendLine();
            }

            this.outClass.AppendLine(className + "::" + className + "(" + Utils.GetWrapperILBridgeTypeNameFor(type) + "* IL) {");
            this.outClass.AppendLine("\t__IL = IL;");
            if (this.usesMarshalContext) this.outClass.AppendLine("\t__IL->__Context = gcnew marshal_context;");
            this.outClass.AppendLine("}");

            this.outClass.AppendLine();

            this.outClass.AppendLine(className + "::~" + className + "() {");
            this.outClass.AppendLine("\tdelete __IL;");
            this.outClass.AppendLine("}");
        }

        private void GenerateCtor(ConstructorInfo ctor, StringBuilder builder)
        {
            // generate method parameters
            var parList = new List<string>();
            GenerateParametersList(ctor.GetParameters(), ref parList);


            var className = Utils.GetWrapperTypeNameFor(ctor.DeclaringType);

            builder.AppendLine(className + "::" + className + "(" + string.Join(", ", parList) + ") {");

            foreach (var parameter in ctor.GetParameters())
            {
                GenerateMarshalParameter(parameter.Name, parameter.ParameterType, builder);
            }

            builder.AppendLine("\t__IL = new " + Utils.GetWrapperILBridgeTypeNameFor(ctor.DeclaringType) + ";");
            builder.Append("\t__IL->__Impl = gcnew " + Utils.GetCppCliTypeFullNameFor(ctor.DeclaringType) + "(");
            builder.Append(string.Join(", ", ctor.GetParameters().Select(par => Utils.GetLocalTempNameFor(par)).ToArray()));
            builder.AppendLine(");");

            if (this.usesMarshalContext) builder.AppendLine("\t__IL->__Context = gcnew marshal_context;");
            builder.AppendLine("}");
        }

        #endregion

        #region Fields generator

        private void GenerateFields(Type type, FieldInfo[] fields)
        {
            foreach (var field in fields)
            {
                if (field.DeclaringType == type)
                {
                    this.outClass.AppendLine();
                    GenerateField(field, this.outClass);
                }
            }
        }

        private void GenerateField(FieldInfo field, StringBuilder builder)
        {
            // translate return type
            var typeTransl = TypeConverter.TranslateParameterType(field.FieldType);
            if (typeTransl.IsWrapperRequired)
            {
                GenerateIlBridgeInclude(typeTransl.ManagedType, this.outHeader);
            }

            // GETTER
            {
                var parList = new List<string>();
                GenerateArrayLengthParameters(Utils.GetLocalTempNameForReturn(), field.FieldType, ref parList, GenParametersType.OutParameter);

                // signature
                builder.Append(typeTransl.NativeType + " " + Utils.GetWrapperTypeNameFor(field.DeclaringType) + "::get_" + field.Name + "(");
                builder.Append(string.Join(", ", parList));
                builder.AppendLine(") {");

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


                GenerateReturnLengthAssignments(field.FieldType, builder);

                GenerateReturnVal(field.FieldType, ref typeTransl, builder);

                builder.AppendLine("}");
            }

            // SETTER
            if (!field.IsInitOnly)
            {
                var parList = new List<string>();
                parList.Add(typeTransl.NativeType + " value");
                GenerateArrayLengthParameters("value", field.FieldType, ref parList, GenParametersType.OutParameter);

                // signature
                builder.Append("void " + Utils.GetWrapperTypeNameFor(field.DeclaringType) + "::set_" + field.Name + "(");
                builder.Append(string.Join(", ", parList));
                builder.AppendLine(") {");

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

        private void GenerateMethods(Type type, MethodInfo[] methods)
        {
            foreach (var method in methods)
            {
                if (method.DeclaringType == type)
                {
                    this.outClass.AppendLine();
                    GenerateMethod(method, this.outClass);
                }
            }
        }

        private void GenerateMethod(MethodInfo method, StringBuilder builder)
        {
            var retValTransl = TypeConverter.TranslateParameterType(method.ReturnType);
            if (retValTransl.IsWrapperRequired)
            {
                GenerateIlBridgeInclude(retValTransl.ManagedType, this.outHeader);
            }

            // signature
            GenerateMethod_Signature(method, ref retValTransl, builder);

            builder.AppendLine("{");

            // method body
            GenerateMethod_MarshalParameters(method, builder);

            GenerateMethod_CallManaged(method, builder);

            GenerateReturnLengthAssignments(method.ReturnType, builder);

            GenerateReturnVal(method.ReturnType, ref retValTransl, builder);

            builder.AppendLine("}");
        }

        private void GenerateMethod_Signature(MethodInfo method, ref TypeConverter.TypeTranslation retValTransl, StringBuilder builder)
        {
            var parList = new List<string>();

            // METHOD NAME
            builder.Append(retValTransl.NativeType + " " + Utils.GetWrapperTypeNameFor(method.DeclaringType) + "::" + method.Name + "(");

            // METHOD PARAMETERS
            GenerateParametersList(method.GetParameters(), ref parList);
            GenerateArrayLengthParameters(Utils.GetLocalTempNameForReturn(), method.ReturnType, ref parList, GenParametersType.OutParameter);

            builder.Append(string.Join(", ", parList));
            builder.Append(") ");
        }

        private void GenerateMethod_MarshalParameters(MethodInfo method, StringBuilder builder)
        {
            foreach (var parameter in method.GetParameters())
            {
                GenerateMarshalParameter(parameter.Name, parameter.ParameterType, builder);
            }
        }

        private void GenerateMethod_CallManaged(MethodInfo method, StringBuilder builder)
        {
            //  GET RETURN VALUE
            builder.Append("\t");

            if (method.ReturnType != typeof (void))
            {
                builder.Append(Utils.GetCppCliTypeFor(method.ReturnType) + " " + Utils.GetLocalTempNameForReturn() + " = ");
            }

            if (method.IsStatic)
                builder.Append(Utils.GetCppCliTypeFullNameFor(method.DeclaringType) + "::");
            else
                builder.Append("__IL->__Impl->");

            // CALL METHOD
            if (!method.IsSpecialName || method.DeclaringType == null)
            {
                builder.Append(method.Name + "(");
                builder.Append(string.Join(", ", method.GetParameters().Select(par => Utils.GetLocalTempNameFor(par))));
                builder.AppendLine(");");
            }
            // GET/SET PROPERTY
            else
            {
                var property = method.DeclaringType.GetProperty(method.Name.Substring(4));
                builder.Append(property.Name);
                if (method.GetParameters().Length == 1) builder.Append(" = " + Utils.GetLocalTempNameFor(method.GetParameters().First()));
                builder.AppendLine(";");
            }
        }

        #endregion

        #region Helpers - public static

        public enum GenParametersType
        {
            Parameter,
            OutParameter,
            NameOnly
        };

        public static void GenerateArrayLengthParameters(string name, Type parameterType, ref List<string> parList, GenParametersType genType)
        {
            var type = "";

            switch (genType)
            {
                case GenParametersType.Parameter:
                    type = "size_t ";
                    break;
                case GenParametersType.OutParameter:
                    type = "size_t& ";
                    break;
            }

            if (parameterType.IsArray)
            {
                // Multidimensional array
                if (parameterType.GetArrayRank() > 1)
                {
                    for (var i = 0; i < parameterType.GetArrayRank(); i++)
                    {
                        parList.Add(type + name + "_len" + i);
                    }
                }
                else // Jagged arrays
                {
                    var elementType = parameterType;
                    var i = 0;

                    while (elementType.IsArray && elementType.HasElementType)
                    {
                        elementType = elementType.GetElementType();
                        parList.Add(type + name + "_len" + (i++));
                    }
                }
            }
        }

        public static void GenerateMarshalParameter(string parameterName, Type parameterType, StringBuilder builder)
        {
            var parType = Utils.GetCppCliTypeFor(parameterType);
            var parVar = Utils.GetLocalTempNameFor(parameterName);
            var parTypeTransl = TypeConverter.TranslateParameterType(parameterType);

            if (parTypeTransl.IsMarshalingRequired)
            {
                var parList = new List<string>();
                parList.Add(parameterName);

                var marshalTo = parType;

                if (parameterType.IsArray)
                {
                    marshalTo = Utils.GetCppCliTypeFor(parameterType.GetElementType());
                    GenerateArrayLengthParameters(parameterName, parameterType, ref parList, GenParametersType.NameOnly);
                }

                if (parTypeTransl.IsMarshalingInContext)
                    builder.AppendLine("\t" + parType + " " + parVar + " = __IL->__Context->_marshal_as<" + marshalTo + ">(" + string.Join(", ", parList) + ");");
                else
                    builder.AppendLine("\t" + parType + " " + parVar + " = _marshal_as<" + marshalTo + ">(" + string.Join(", ", parList) + ");");
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

        public static void GenerateNamespaces(Type type, StringBuilder builder)
        {
            builder.AppendLine("namespace " + Utils.GetWrapperProjectName() + " {");

            if (type.Namespace != null)
            {
                var namespaces = type.Namespace.Split('.');
                foreach (var ns in namespaces)
                {
                    builder.AppendLine("namespace " + ns + " {");
                }
            }
            builder.AppendLine();
        }

        public static void GenerateEndNamespaces(Type type, StringBuilder builder)
        {
            if (type.Namespace != null)
            {
                var namespaces = type.Namespace.Split('.');
                builder.AppendLine();

                foreach (var ns in namespaces)
                {
                    builder.AppendLine("}");
                }
            }
            builder.AppendLine("}");
        }

        #endregion

        #region Helpers

        private void GenerateParametersList(ParameterInfo[] parameters, ref List<string> parList)
        {
            foreach (var parameter in parameters)
            {
                var parTypeTransl = TypeConverter.TranslateParameterType(parameter.ParameterType);
                if (parTypeTransl.IsWrapperRequired)
                {
                    GenerateIlBridgeInclude(parTypeTransl.ManagedType, this.outHeader);
                }

                if (typeof(MulticastDelegate).IsAssignableFrom(parameter.ParameterType))
                {
                    var method = parameter.ParameterType.GetMethod("Invoke");
                    var delegateParList = new List<string>();

                    var retTypeTransl = TypeConverter.TranslateParameterType(method.ReturnType);
                    if (retTypeTransl.IsWrapperRequired)
                    {
                        GenerateIlBridgeInclude(retTypeTransl.ManagedType, this.outHeader);
                    }

                    GenerateParametersList(method.GetParameters(), ref delegateParList);

                    parList.Add(retTypeTransl.NativeType + " (*" + parameter.Name + ")(" + string.Join(", ", delegateParList) + ")");
                }
                else
                {
                    parList.Add(parTypeTransl.NativeType + " " + parameter.Name);
                    GenerateArrayLengthParameters(parameter.Name, parameter.ParameterType, ref parList, GenParametersType.Parameter);
                }
            }
        }

        

        private void GenerateReturnLengthAssignments(Type returnType, StringBuilder builder)
        {
            if (returnType.IsArray)
            {
                var returnVar = Utils.GetLocalTempNameForReturn();

                // Multidimensional array
                if (returnType.GetArrayRank() > 1)
                {
                    for (var i = 0; i < returnType.GetArrayRank(); i++)
                    {
                        builder.AppendLine("\t" + returnVar + "_len" + i + " = " + returnVar + "->GetLength(" + i + ");");
                    }
                }
                else // Jagged arrays
                {
                    var elementType = returnType;
                    var i = 0;
                    var rank = "";

                    while (elementType.IsArray && elementType.HasElementType)
                    {
                        elementType = elementType.GetElementType();
                        builder.AppendLine("\t" + returnVar + "_len" + (i++) + " = " + returnVar + rank + "->Length;");
                        rank += "[0]";
                    }
                }
            }
        }

        private void GenerateReturnVal(Type returnType, ref TypeConverter.TypeTranslation retValTransl, StringBuilder builder)
        {
            //  - RETURN / MARSHAL RETURN / WRAPPED RETURN
            if (returnType != typeof (void))
            {
                var returnVar = Utils.GetLocalTempNameForReturn();

                if (retValTransl.IsMarshalingRequired)
                {
                    if (retValTransl.IsMarshalingInContext)
                        builder.AppendLine("\t" + retValTransl.NativeType + " " + returnVar + "Marshaled = __IL->__Context->_marshal_as<" + retValTransl.NativeType + ">(" + returnVar + ");");
                    else
                        builder.AppendLine("\t" + retValTransl.NativeType + " " + returnVar + "Marshaled = _marshal_as<" + retValTransl.NativeType + ">(" + returnVar + ");");

                    returnVar += "Marshaled";

                    // TODO: Generate marshalers if needed? Maybe for multidimensional arrays?
                }
                else if (retValTransl.IsCastRequired)
                {
                    builder.AppendLine("\t" + retValTransl.NativeType + " " + returnVar + "Cast = static_cast<" + retValTransl.NativeType + ">(" + returnVar + ");");

                    returnVar += "Cast";
                }

                builder.AppendLine("\treturn " + returnVar + ";");
            }
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
            return Utils.GetWrapperFileNameFor(type) + ".cpp";
        }

        public override string GetFileNameFor(Type type)
        {
            return GetFileName(type);
        }

    }
}