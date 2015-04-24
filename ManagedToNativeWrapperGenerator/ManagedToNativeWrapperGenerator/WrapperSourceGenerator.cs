using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ManagedToNativeWrapperGenerator
{
    public class WrapperSourceGenerator : TypeGenerator
    {
        public WrapperSourceGenerator(String outputFolder)
            : base(outputFolder)
        {

        }

        StringBuilder outHeader;
        StringBuilder outClass; 

        HashSet<Type> includedTypes;

        private bool usesMarshalContext = false;



        public override void ClassLoad(Type type, FieldInfo[] fields, ConstructorInfo[] ctors, MethodInfo[] methods)
        {
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
    
            this.outHeader.AppendLine("#include \"" + WrapperILBridgesGenerator.GetFileName(type) + "\"");



            WrapperSourceGenerator.GenerateNamespaces(type, this.outClass);

            this.GenerateCtors(type, ctors);

            this.GenerateFields(type, fields);

            this.GenerateMethods(type, methods);

            WrapperSourceGenerator.GenerateEndNamespaces(type, this.outClass);



            // append class text to header
            this.outHeader.AppendLine();
            this.outHeader.Append(this.outClass.ToString());

            this.WriteToFile(this.outHeader.ToString(), GetFileName(type));
        }



        #region Ctors/dctors generators

        private void GenerateCtors(Type type, ConstructorInfo[] ctors)
        {
            string className = Utils.GetWrapperTypeNameFor(type);

            foreach (ConstructorInfo ctor in ctors)
            {
                if (ctor.DeclaringType == type)
                {
                    GenerateCtor(ctor, this.outClass);
                    this.outClass.AppendLine();
                }
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
            List<string> parList = new List<string>();

            foreach (ParameterInfo parameter in ctor.GetParameters())
            {
                // translate parameter
                TypeConverter.TypeTranslation parTypeTransl = TypeConverter.TranslateParameterType(parameter.ParameterType);
                if (parTypeTransl.isWrapperRequired)
                {
                    this.GenerateIlBridgeInclude(parTypeTransl.ManagedType, this.outHeader);
                }

                parList.Add(parTypeTransl.NativeType + " " + parameter.Name);
                WrapperSourceGenerator.GenerateArrayLengthParameters(parameter.Name, parameter.ParameterType, parList, WrapperSourceGenerator.GenParametersType.Parameter); // generate parameters for parameter-array lengths (if array used)
            }

            string className = Utils.GetWrapperTypeNameFor(ctor.DeclaringType);

            builder.AppendLine(className + "::" + className + "(" + String.Join(", ", parList) + ") {");

            foreach (ParameterInfo parameter in ctor.GetParameters())
            {
                GenerateMarshalParameter(parameter.Name, parameter.ParameterType, builder);
            }

            builder.AppendLine("\t__IL = new " + Utils.GetWrapperILBridgeTypeNameFor(ctor.DeclaringType) + ";");
            builder.Append("\t__IL->__Impl = gcnew " + Utils.GetCppCliTypeNameFor(ctor.DeclaringType) + "(");
            builder.Append(String.Join(", ", ctor.GetParameters().Select(par => Utils.GetLocalTempNameFor(par)).ToArray()));
            builder.AppendLine(");");

            if (this.usesMarshalContext) builder.AppendLine("\t__IL->__Context = gcnew marshal_context;");
            builder.AppendLine("}");
        }


        #endregion


        #region Fields generator

        private void GenerateFields(Type type, FieldInfo[] fields)
        {
            foreach (FieldInfo field in fields)
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
            TypeConverter.TypeTranslation typeTransl = TypeConverter.TranslateParameterType(field.FieldType);
            if (typeTransl.isWrapperRequired)
            {
                this.GenerateIlBridgeInclude(typeTransl.ManagedType, this.outHeader);
            }

            // GETTER
            {
                List<string> parList = new List<string>();
                WrapperSourceGenerator.GenerateArrayLengthParameters(Utils.GetLocalTempNameForReturn(), field.FieldType, parList, WrapperSourceGenerator.GenParametersType.OutParameter);

                // signature
                builder.Append(typeTransl.NativeType + " " + Utils.GetWrapperTypeNameFor(field.DeclaringType) + "::get_" + field.Name + "(");
                builder.Append(String.Join(", ", parList));
                builder.AppendLine(") {");

                // body
                builder.Append("\t");
                builder.Append(Utils.GetCppCliTypeFor(field.FieldType) + " " + Utils.GetLocalTempNameForReturn() + " = ");

                if (field.IsStatic)
                    builder.Append(Utils.GetCppCliTypeNameFor(field.DeclaringType) + "::");
                else
                    builder.Append("__IL->__Impl->");

                builder.AppendLine(field.Name + ";");


                this.GenerateReturnLengthAssignments(field.FieldType, builder);

                this.GenerateReturnVal(field.FieldType, ref typeTransl, builder);

                builder.AppendLine("}");
            }

            // SETTER
            if (!field.IsInitOnly)
            {
                List<string> parList = new List<string>();
                parList.Add(typeTransl.NativeType + " value");
                WrapperSourceGenerator.GenerateArrayLengthParameters("value", field.FieldType, parList, WrapperSourceGenerator.GenParametersType.OutParameter);


                // signature
                builder.Append("void " + Utils.GetWrapperTypeNameFor(field.DeclaringType) + "::set_" + field.Name + "(");
                builder.Append(String.Join(", ", parList));
                builder.AppendLine(") {");


                // body
                GenerateMarshalParameter("value", field.FieldType, builder);

                builder.Append("\t");

                if (field.IsStatic)
                    builder.Append(Utils.GetCppCliTypeNameFor(field.DeclaringType) + "::");
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
            foreach (MethodInfo method in methods)
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
            
            TypeConverter.TypeTranslation retValTransl = TypeConverter.TranslateParameterType(method.ReturnType);
            if (retValTransl.isWrapperRequired)
            {
                this.GenerateIlBridgeInclude(retValTransl.ManagedType, this.outHeader);
            }

            // signature
            this.GenerateMethod_Signature(method, ref retValTransl, builder);

            builder.AppendLine("{");

            // method body
            this.GenerateMethod_MarshalParameters(method, builder);

            this.GenerateMethod_CallManaged(method, builder);

            this.GenerateReturnLengthAssignments(method.ReturnType, builder);

            this.GenerateReturnVal(method.ReturnType, ref retValTransl, builder);

            builder.AppendLine("}");
        }

      
        private void GenerateMethod_Signature(MethodInfo method, ref TypeConverter.TypeTranslation retValTransl, StringBuilder builder)
        {
            List<string> parList = new List<string>();

            // METHOD NAME
            builder.Append(retValTransl.NativeType + " " + Utils.GetWrapperTypeNameFor(method.DeclaringType) + "::" + method.Name + "(");

            // METHOD PARAMETERS
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                TypeConverter.TypeTranslation parTypeTransl = TypeConverter.TranslateParameterType(parameter.ParameterType);
                if (parTypeTransl.isWrapperRequired)
                {
                    GenerateIlBridgeInclude(parTypeTransl.ManagedType, this.outHeader);
                }

                parList.Add(parTypeTransl.NativeType + " " + parameter.Name);
                GenerateArrayLengthParameters(parameter.Name, parameter.ParameterType, parList, GenParametersType.Parameter);
            }
            GenerateArrayLengthParameters(Utils.GetLocalTempNameForReturn(), method.ReturnType, parList, GenParametersType.OutParameter);

            builder.Append(String.Join(", ", parList));
            builder.Append(") ");
        }


        private void GenerateMethod_MarshalParameters(MethodInfo method, StringBuilder builder)
        {
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                GenerateMarshalParameter(parameter.Name, parameter.ParameterType, builder);
            }
        }


        private void GenerateMethod_CallManaged(MethodInfo method, StringBuilder builder)
        {
            //  GET RETURN VALUE
            builder.Append("\t");

            if (method.ReturnType != typeof(void))
            {
                builder.Append(Utils.GetCppCliTypeFor(method.ReturnType) + " " + Utils.GetLocalTempNameForReturn() + " = ");
            }

            if (method.IsStatic)
                builder.Append(Utils.GetCppCliTypeNameFor(method.DeclaringType) + "::");
            else
                builder.Append("__IL->__Impl->");


            // CALL METHOD
            if (!method.IsSpecialName)
            {
                builder.Append(method.Name + "(");
                builder.Append(String.Join(", ", method.GetParameters().Select(par => Utils.GetLocalTempNameFor(par)).ToArray()));
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

        public enum GenParametersType { Parameter, OutParameter, NameOnly };

        public static void GenerateArrayLengthParameters(String name, Type parameterType, List<string> parList, GenParametersType genType)
        {
            string type = "";

            switch (genType)
            {
                case GenParametersType.Parameter: type = "size_t "; break;
                case GenParametersType.OutParameter: type = "size_t& "; break;
            }

            if (parameterType.IsArray)
            {
                // Multidimensional array
                if (parameterType.GetArrayRank() > 1)
                {
                    for (int i = 0; i < parameterType.GetArrayRank(); i++)
                    {
                        parList.Add(type + name + "_len" + i);
                    }
                }
                else // Jagged arrays
                {
                    Type elementType = parameterType;
                    int i = 0;

                    while (elementType.IsArray && elementType.HasElementType)
                    {
                        elementType = elementType.GetElementType();
                        parList.Add(type + name + "_len" + (i++));
                    }
                }
            }
        }


        public static void GenerateNamespaces(Type type, StringBuilder builder)
        {
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
        }

        #endregion


        #region Helpers

        private static void GenerateMarshalParameter(string parameterName, Type parameterType, StringBuilder builder)
        {
            string parType = Utils.GetCppCliTypeFor(parameterType); 
            string parVar = Utils.GetLocalTempNameFor(parameterName);
            TypeConverter.TypeTranslation parTypeTransl = TypeConverter.TranslateParameterType(parameterType);

            if (parTypeTransl.isWrapperRequired && parTypeTransl.isILObject && false)
            {
                builder.AppendLine("\t" + parType + " " + parVar + " = " + parameterName + "->__IL->__Impl.get();");
            }
            else if (parTypeTransl.isMarshalingRequired)
            {
                List<string> parList = new List<string>();
                parList.Add(parameterName);

                string marshalTo = parType;

                if (parameterType.IsArray)
                {
                    marshalTo = Utils.GetCppCliTypeFor(parameterType.GetElementType());
                    GenerateArrayLengthParameters(parameterName, parameterType, parList, GenParametersType.NameOnly);
                }

                if (parTypeTransl.isMarshalingInContext)
                    builder.AppendLine("\t" + parType + " " + parVar + " = __IL->__Context->_marshal_as<" + marshalTo + ">(" + String.Join(", ", parList) + ");");
                else
                    builder.AppendLine("\t" + parType + " " + parVar + " = _marshal_as<" + marshalTo + ">(" + String.Join(", ", parList) + ");");

            }
            else if (parTypeTransl.isCastRequired)
            {
                builder.AppendLine("\t" + parType + " " + parVar + " = static_cast<" + parType + ">(" + parameterName + ");");
            }
            else
            {
                builder.AppendLine("\t" + parType + " " + parVar + " = " + parameterName + ";");
            }
        }


        private void GenerateReturnLengthAssignments(Type returnType, StringBuilder builder)
        {
            if (returnType.IsArray)
            {
                string returnVar = Utils.GetLocalTempNameForReturn();

                // Multidimensional array
                if (returnType.GetArrayRank() > 1)
                {
                    for (int i = 0; i < returnType.GetArrayRank(); i++)
                    {
                        builder.AppendLine("\t" + returnVar + "_len" + i + " = " + returnVar + "->GetLength(" + i + ");");
                    }
                }
                else // Jagged arrays
                {
                    Type elementType = returnType;
                    int i = 0;
                    string rank = "";

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
            if (returnType != typeof(void))
            {
                string returnVar = Utils.GetLocalTempNameForReturn();

                if (retValTransl.isMarshalingRequired)
                {
                    returnVar += "Marshaled";

                    if (retValTransl.isMarshalingInContext)
                        builder.AppendLine("\t" + retValTransl.NativeType + " " + returnVar + " = __IL->__Context->_marshal_as<" + retValTransl.NativeType + ">(__ReturnVal);");
                    else
                        builder.AppendLine("\t" + retValTransl.NativeType + " " + returnVar + " = _marshal_as<" + retValTransl.NativeType + ">(__ReturnVal);");

                    // TODO: Generate marshalers if needed? Maybe for multidimensional arrays?
                }
                else if (retValTransl.isCastRequired)
                {
                    builder.AppendLine("\t" + retValTransl.NativeType + " " + returnVar + "Cast = static_cast<" + retValTransl.NativeType + ">(" + returnVar + ");");
                    
                    returnVar += "Cast";
                }

                builder.AppendLine("\treturn " + returnVar + ";");
            }
        }



        private void GenerateIlBridgeInclude(Type type, StringBuilder builder)
        {
            Type t = type;
            if (t.IsArray && t.HasElementType)
            {
                t = t.GetElementType();
            }

            if (this.includedTypes.Contains(t)) return;

            this.outHeader.AppendLine("#include \"" + WrapperILBridgesGenerator.GetFileName(t) + "\"");

            this.includedTypes.Add(t);
        }

        #endregion



        public static string GetFileName(Type type)
        {
            return Utils.GetWrapperTypeNameFor(type) + ".cpp";
        }

        public override string GetFileNameFor(Type type)
        {
            return GetFileName(type);
        }


    }
}
