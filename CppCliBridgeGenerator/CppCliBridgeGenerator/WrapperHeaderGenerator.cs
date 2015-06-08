using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CppCliBridgeGenerator
{
    /// <summary>
    /// Generator for main header file.
    /// </summary>
    public class WrapperHeaderGenerator : Generator
    {
        private HashSet<Type> declaredClasses;
        private XmlDocHelper xmlDoc;
        private StringBuilder outClass;
        private StringBuilder outHeader;

        /// <summary>
        /// Generator for main header file.
        /// </summary>
        /// <param name="outputFolder">Output folder.</param>
        public WrapperHeaderGenerator(string outputFolder)
            : base(outputFolder)
        {
        }

        /// <summary>
        /// Called for every new assembly.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        public override void AssemblyLoad(Assembly assembly)
        {
            string dllPath = assembly.Location; 
            string docuPath = dllPath.Substring(0, dllPath.LastIndexOf(".")) + ".XML";

            this.xmlDoc = new XmlDocHelper(docuPath);
        }


        #region Enum generator
        
        /// <summary>
        /// Called for enums.
        /// </summary>
        /// <param name="type">Type with enum.</param>
        /// <param name="fields">Selected fields of enum.</param>
        public override void EnumLoad(Type type, FieldInfo[] fields)
        {
            this.outHeader = new StringBuilder();
            this.outClass = new StringBuilder();

            this.declaredClasses = new HashSet<Type>();
            this.declaredClasses.Add(type);

            this.outHeader.AppendLine("#pragma once");
            this.outHeader.AppendLine();

            GenerateEnum(type, fields);

            // append enum text to header
            this.outHeader.Append(this.outClass);

            WriteToFile(this.outHeader.ToString(), GetFileName(type));
        }

        /// <summary>
        /// Generation of enum.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fields"></param>
        private void GenerateEnum(Type type, FieldInfo[] fields)
        {
            WrapperSourceGenerator.GenerateNamespaces(type, this.outClass); // generate namespaces

            this.outClass.AppendLine("enum " + Utils.GetWrapperTypeNameFor(type) + " {"); // wrapper enum

            var strFields = fields.Select(f => f.Name + " = " + Convert.ChangeType(f.GetValue(null), typeof (ulong))).ToArray(); // get all enum values
            this.outClass.AppendLine("\t" + string.Join("," + Environment.NewLine + "\t", strFields)); // format them into enum

            this.outClass.AppendLine("};"); // enum closing


            WrapperSourceGenerator.GenerateEndNamespaces(type, this.outClass);
        }

        #endregion


        #region Class generators 
   
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


            GenerateClass(type); // class opening
            GenerateCtors(type, ctors); // constructors
            GenerateFields(type, fields); // class fields
            GenerateMethods(type, methods); // methods
            GenerateEndClass(type); // class closing

            // append class text to header
            this.outHeader.Append(this.outClass);

            WriteToFile(this.outHeader.ToString(), GetFileName(type));
        }

        /// <summary>
        /// Opening for class including pointer for IL bridge.
        /// </summary>
        /// <param name="type">Type.</param>
        private void GenerateClass(Type type)
        {
            WrapperSourceGenerator.GenerateNamespaces(type, this.outClass);

            this.outClass.AppendLine("class " + Utils.GetWrapperILBridgeTypeNameFor(type) + ";"); // IL Bridge
            this.outClass.AppendLine();

            GenerateDocComment(this.xmlDoc.getDocDomment(type), this.outClass, "/// ");
            this.outClass.AppendLine("class _LNK " + Utils.GetWrapperTypeNameFor(type) + " {"); // Wrapper class
            this.outClass.AppendLine();
            this.outClass.AppendLine("\tpublic:");
            this.outClass.AppendLine("\t\t" + Utils.GetWrapperILBridgeTypeNameFor(type) + "* __IL;"); // IL Bridge instance
            this.outClass.AppendLine();
        }

        /// <summary>
        /// Closing for class.
        /// </summary>
        /// <param name="type">Type.</param>
        private void GenerateEndClass(Type type)
        {
            this.outClass.AppendLine("};");
            WrapperSourceGenerator.GenerateEndNamespaces(type, this.outClass);
        }

        #endregion

        #region Class - ctors/dctors generators

        /// <summary>
        /// Generation of constructor signatures.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="ctors">Selected constructors.</param>
        private void GenerateCtors(Type type, ConstructorInfo[] ctors)
        {
            var className = Utils.GetWrapperTypeNameFor(type); // bridge class name

            foreach (var ctor in ctors)
            {
                if (ctor.DeclaringType == type) // skip inherited
                {
                    GenerateCtor(ctor, this.outClass); // generate signature for one constructor
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

        /// <summary>
        /// Generation of one constructor.
        /// </summary>
        /// <param name="ctor">Constructor.</param>
        /// <param name="builder">Output StringBuilder.</param>
        private void GenerateCtor(ConstructorInfo ctor, StringBuilder builder)
        {
            // generate method parameters
            var parList = new List<string>();
            GenerateParametersList(ctor.GetParameters(), ref parList);

            GenerateDocComment(this.xmlDoc.getDocDomment(ctor), builder); // add documentation comment for constructor
            builder.Append("\t\t");
            builder.Append(Utils.GetWrapperTypeNameFor(ctor.DeclaringType) + "("); // method name
            builder.Append(string.Join(", ", parList)); // parameters
            builder.AppendLine(");");
        }

        #endregion

        #region Class - fields generator

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
                GenerateWrapperDeclaration(typeTransl.ManagedType, this.outHeader);
            }

            // GETTER
            {
                GenerateDocComment(this.xmlDoc.getDocDomment(field), builder);
                builder.Append("\t\t");

                if (field.IsStatic) builder.Append("static "); // static keyword
                builder.AppendLine(typeTransl.NativeType + " get_" + field.Name + "();");
            }

            // SETTER
            if (!field.IsInitOnly)
            {
                builder.AppendLine();
                GenerateDocComment(this.xmlDoc.getDocDomment(field), builder);
                builder.Append("\t\t");

                if (field.IsStatic) builder.Append("static "); // static keyword
                builder.AppendLine("void set_" + field.Name + "(" + typeTransl.NativeType + " value);");
            }
        }

        #endregion

        #region Class - methods generator

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
            // translate return type
            var returnTypeTransl = TypeConverter.TranslateType(method.ReturnType);
            if (returnTypeTransl.IsWrapperRequired)
            {
                GenerateWrapperDeclaration(returnTypeTransl.ManagedType, this.outHeader);
            }

            // generate method parameters
            var parList = new List<string>();
            GenerateParametersList(method.GetParameters(), ref parList);

            GenerateDocComment(this.xmlDoc.getDocDomment(method), builder);
            builder.Append("\t\t");

            if (method.IsStatic) builder.Append("static "); // static keyword
            builder.Append(returnTypeTransl.NativeType + " " + method.Name + "("); // method name
            builder.Append(string.Join(", ", parList));
            builder.AppendLine(");");
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
                // delegates
                if (typeof(MulticastDelegate).IsAssignableFrom(parameter.ParameterType))
                {
                    var method = parameter.ParameterType.GetMethod("Invoke");
                    var delegateParList = new List<string>();

                    // return type translation
                    var retTypeTransl = TypeConverter.TranslateType(method.ReturnType);
                    if (retTypeTransl.IsWrapperRequired)
                    {
                        GenerateWrapperDeclaration(retTypeTransl.ManagedType, this.outHeader);
                    }

                    // get parameters of delegate
                    GenerateParametersList(method.GetParameters(), ref delegateParList);

                    // add to output
                    parList.Add(retTypeTransl.NativeType + " (*" + parameter.Name + ")(" + string.Join(", ", delegateParList) + ")");
                }                          
                else // everything else
                {
                    // translate parameter
                    var parTypeTransl = TypeConverter.TranslateType(parameter.ParameterType);
                    if (parTypeTransl.IsWrapperRequired)
                    {
                        GenerateWrapperDeclaration(parTypeTransl.ManagedType, this.outHeader);
                    }

                    // add to output
                    parList.Add(parTypeTransl.NativeType + " " + parameter.Name);
                }
            }
        }

        /// <summary>
        /// Generation of enum/class forward-declaration for found usages of diffenet types.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="builder">Output StringBuilder.</param>
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

        private void GenerateDocComment(string comment, StringBuilder builder, string spacer = "\t\t/// ")
        {
            if (comment.Length == 0) return;
            builder.AppendLine(Utils.IndentText(comment, spacer));    
        }


        #endregion



        /// <summary>
        /// Gets output filename of this generator for type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Filename</returns>
        public static string GetFileName(Type type)
        {
            return Utils.GetWrapperFileNameFor(type) + ".h";
        }

    }
}