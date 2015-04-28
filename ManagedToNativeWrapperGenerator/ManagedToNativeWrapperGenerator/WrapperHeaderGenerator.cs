using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ManagedToNativeWrapperGenerator
{
    public class WrapperHeaderGenerator : TypeGenerator
    {
        private HashSet<Type> declaredClasses;
        private StringBuilder outClass;
        private StringBuilder outHeader;

        public WrapperHeaderGenerator(string outputFolder)
            : base(outputFolder)
        {
        }

        public override void EnumLoad(Type type, FieldInfo[] fields)
        {
            this.outHeader = new StringBuilder();
            this.outClass = new StringBuilder();

            this.declaredClasses = new HashSet<Type>();
            this.declaredClasses.Add(type);

            this.outHeader.AppendLine("#pragma once");
            this.outHeader.AppendLine();

            GenerateEnum(type, fields);

            // append class text to header
            this.outHeader.Append(this.outClass);

            WriteToFile(this.outHeader.ToString(), GetFileName(type));
        }

        #region EnumValues generator

        private void GenerateEnum(Type type, FieldInfo[] fields)
        {
            WrapperSourceGenerator.GenerateNamespaces(type, this.outClass);

            this.outClass.AppendLine("enum " + Utils.GetWrapperTypeNameFor(type) + " {"); // Wrapper enum

            var strFields = fields.Select(f => f.Name + " = " + Convert.ChangeType(f.GetValue(null), typeof (ulong))).ToArray();
            this.outClass.AppendLine("\t" + string.Join("," + Environment.NewLine + "\t", strFields));

            this.outClass.AppendLine("};");


            WrapperSourceGenerator.GenerateEndNamespaces(type, this.outClass);
        }

        #endregion

        public override void ClassLoad(Type type, FieldInfo[] fields, ConstructorInfo[] ctors, MethodInfo[] methods)
        {
            if (typeof(MulticastDelegate).IsAssignableFrom(type))
                return;

            this.outHeader = new StringBuilder();
            this.outClass = new StringBuilder();

            this.declaredClasses = new HashSet<Type>();
            this.declaredClasses.Add(type);

            this.outHeader.AppendLine("#pragma once");
            this.outHeader.AppendLine("#include <string>");
            this.outHeader.AppendLine("#include <vector>");
            this.outHeader.AppendLine();

            this.outHeader.AppendLine("#ifndef _LNK");
            this.outHeader.AppendLine("#define _LNK __declspec(dllimport)");
            this.outHeader.AppendLine("#endif");
            this.outHeader.AppendLine();


            GenerateClass(type);

            GenerateCtors(type, ctors);

            GenerateFields(type, fields);

            GenerateMethods(type, methods);

            GenerateEndClass(type);

            // append class text to header
            this.outHeader.Append(this.outClass);

            WriteToFile(this.outHeader.ToString(), GetFileName(type));
        }


        #region Class generators

        private void GenerateClass(Type type)
        {
            WrapperSourceGenerator.GenerateNamespaces(type, this.outClass);

            this.outClass.AppendLine("class " + Utils.GetWrapperILBridgeTypeNameFor(type) + ";"); // IL Bridge
            this.outClass.AppendLine();

            this.outClass.AppendLine("class _LNK " + Utils.GetWrapperTypeNameFor(type) + " {"); // Wrapper class
            this.outClass.AppendLine();
            this.outClass.AppendLine("\tpublic:");
            this.outClass.AppendLine("\t\t" + Utils.GetWrapperILBridgeTypeNameFor(type) + "* __IL;"); // IL Bridge instance
            this.outClass.AppendLine();
        }

        private void GenerateEndClass(Type type)
        {
            this.outClass.AppendLine("};");
            WrapperSourceGenerator.GenerateEndNamespaces(type, this.outClass);
        }

        #endregion

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
                this.outClass.AppendLine("\t\t" + className + "();"); // empty constructor
            }

            this.outClass.AppendLine("\t\t" + className + "(" + Utils.GetWrapperILBridgeTypeNameFor(type) + "* IL);"); // IL constructor
            this.outClass.AppendLine();
            this.outClass.AppendLine("\t\t~" + className + "();"); // Destructor
        }

        private void GenerateCtor(ConstructorInfo ctor, StringBuilder builder)
        {
            // generate method parameters
            var parList = new List<string>();
            GenerateParametersList(ctor.GetParameters(), ref parList);

            builder.Append("\t\t");
            builder.Append(Utils.GetWrapperTypeNameFor(ctor.DeclaringType) + "("); // method name
            builder.Append(string.Join(", ", parList));
            builder.AppendLine(");");
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
                GenerateWrapperDeclaration(typeTransl.ManagedType, this.outHeader);
            }

            // GETTER
            {
                builder.Append("\t\t");

                if (field.IsStatic) builder.Append("static "); // static keyword
                builder.AppendLine(typeTransl.NativeType + " get_" + field.Name + "();");
            }

            // SETTER
            if (!field.IsInitOnly)
            {
                builder.AppendLine();
                builder.Append("\t\t");

                if (field.IsStatic) builder.Append("static "); // static keyword
                builder.AppendLine("void set_" + field.Name + "(" + typeTransl.NativeType + " value);");
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
            // translate return type
            var returnTypeTransl = TypeConverter.TranslateParameterType(method.ReturnType);
            if (returnTypeTransl.IsWrapperRequired)
            {
                GenerateWrapperDeclaration(returnTypeTransl.ManagedType, this.outHeader);
            }

            // generate method parameters
            var parList = new List<string>();
            GenerateParametersList(method.GetParameters(), ref parList);

            builder.Append("\t\t");

            if (method.IsStatic) builder.Append("static "); // static keyword
            builder.Append(returnTypeTransl.NativeType + " " + method.Name + "("); // method name
            builder.Append(string.Join(", ", parList));
            builder.AppendLine(");");
        }

        #endregion

        #region Helpers

        private void GenerateParametersList(ParameterInfo[] parameters, ref List<string> parList)
        {
            foreach (var parameter in parameters)
            {
                if (typeof (MulticastDelegate).IsAssignableFrom(parameter.ParameterType))
                {
                    var method = parameter.ParameterType.GetMethod("Invoke");
                    var delegateParList = new List<string>();

                    var retTypeTransl = TypeConverter.TranslateParameterType(method.ReturnType);
                    if (retTypeTransl.IsWrapperRequired)
                    {
                        GenerateWrapperDeclaration(retTypeTransl.ManagedType, this.outHeader);
                    }

                    GenerateParametersList(method.GetParameters(), ref delegateParList);

                    parList.Add(retTypeTransl.NativeType + " (*" + parameter.Name + ")(" + string.Join(", ", delegateParList) + ")");
                }                          
                else
                {
                    var parTypeTransl = TypeConverter.TranslateParameterType(parameter.ParameterType);
                    if (parTypeTransl.IsWrapperRequired)
                    {
                        GenerateWrapperDeclaration(parTypeTransl.ManagedType, this.outHeader);
                    }

                    parList.Add(parTypeTransl.NativeType + " " + parameter.Name);
                }
            }
        }

        private void GenerateWrapperDeclaration(Type type, StringBuilder builder)
        {
            var t = type;
            if (t.IsArray && t.HasElementType)
            {
                t = t.GetElementType();
            }

            if (this.declaredClasses.Contains(t)) return; // already declared class

            WrapperSourceGenerator.GenerateNamespaces(t, builder);

            if (t.IsEnum)
                builder.AppendLine("enum " + Utils.GetWrapperTypeNameFor(t) + ";");
            else
                builder.AppendLine("class " + Utils.GetWrapperTypeNameFor(t) + ";");

            WrapperSourceGenerator.GenerateEndNamespaces(t, builder);
            builder.AppendLine();

            this.declaredClasses.Add(t);
        }

        #endregion




        public static string GetFileName(Type type)
        {
            return Utils.GetWrapperFileNameFor(type) + ".h";
        }

        public override string GetFileNameFor(Type type)
        {
            return GetFileName(type);
        }
    }
}