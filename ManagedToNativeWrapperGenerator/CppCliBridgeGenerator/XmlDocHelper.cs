using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace CppCliBridgeGenerator
{
    /// <summary>
    /// Tool for working with XML annotations for .NET assemblies.
    /// </summary>
    class XmlDocHelper
    {
        private XmlDocument xmlDoc;


        /// <summary>
        /// Tool for working with XML annotations for .NET assemblies.
        /// </summary>
        /// <param name="docuPath">Path to XML with annotations for .NET assembly.</param>
        public XmlDocHelper(string docuPath)
        {
            if (File.Exists(docuPath))
            {
                try
                {
                    this.xmlDoc = new XmlDocument();
                    this.xmlDoc.PreserveWhitespace = true;
                    this.xmlDoc.Load(docuPath);
                }
                catch (Exception)
                {
                    this.xmlDoc = null;
                }
            }
        }

        /// <summary>
        /// Get documentation comment for method.
        /// </summary>
        /// <param name="method">Method.</param>
        /// <returns>Documentation comment or empty string.</returns>
        public string getDocDomment(MethodInfo method)
        {
            if (method.IsSpecialName)
            {
                var property = method.DeclaringType.GetProperty(method.Name.Substring(4));
                return getDocCommentFor("P:" + method.DeclaringType.FullName + "." + property.Name);
            }

            return getDocCommentFor("M:" + method.DeclaringType.FullName + "." + method.Name + getParameters(method.GetParameters()));  
        }

        /// <summary>
        /// Get documentation comment for constructor.
        /// </summary>
        /// <param name="ctor">Constructor.</param>
        /// <returns>Documentation comment or empty string.</returns>
        public string getDocDomment(ConstructorInfo ctor)
        {
            return getDocCommentFor("M:" + ctor.DeclaringType.FullName + ".#ctor" + getParameters(ctor.GetParameters()));
        }

        /// <summary>
        /// Get documentation comment for field.
        /// </summary>
        /// <param name="field">Field.</param>
        /// <returns>Documentation comment or empty string.</returns>
        public string getDocDomment(FieldInfo field)
        {
            return getDocCommentFor("F:" + field.DeclaringType.FullName + field.Name);
        }

        /// <summary>
        /// Get documentation comment for type (eg. class).
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Documentation comment or empty string.</returns>
        public string getDocDomment(Type type)
        {
            return getDocCommentFor("T:" + type.FullName);
        }


        /// <summary>
        /// Get documentation comment using raw node name.
        /// </summary>
        /// <param name="nodeName">Parameter name of node.</param>
        /// <returns>Documentation comment or empty string.</returns>
        private string getDocCommentFor(string nodeName)
        {
            if (this.xmlDoc == null) return "";
            XmlNode node = this.xmlDoc.SelectSingleNode("//member[@name = '" + nodeName + "']");
            if (node == null) return "";
            return Regex.Replace(node.InnerXml, Environment.NewLine + @"\s+", Environment.NewLine).Trim();
        }

        /// <summary>
        /// Prepare parameters for methods/constructors.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <returns>Parameters as string usable in XML node name.</returns>
        private string getParameters(ParameterInfo[] parameters)
        {
            if (parameters.Length == 0) return "";

            string outstr = "(";

            // add parameters to outstr
            foreach (ParameterInfo parameterInfo in parameters)
            {
                if (outstr.Length > 0) outstr += ",";
                outstr += parameterInfo.ParameterType.FullName;
            }

            outstr += ")";

            return outstr;
        }



    }
}
