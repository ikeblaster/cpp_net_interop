using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace CppCliBridgeGenerator
{
    class XmlDocHelper
    {
        private XmlDocument xmlDoc;


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
                }
            }
        }

        public string getDocDomment(MethodInfo method)
        {
            if (method.IsSpecialName)
            {
                var property = method.DeclaringType.GetProperty(method.Name.Substring(4));
                return getDocCommentFor("P:" + method.DeclaringType.FullName + "." + property.Name);
            }

            return getDocCommentFor("M:" + method.DeclaringType.FullName + "." + method.Name + getParameters(method.GetParameters()));  
        }

        public string getDocDomment(ConstructorInfo ctor)
        {
            return getDocCommentFor("M:" + ctor.DeclaringType.FullName + ".#ctor" + getParameters(ctor.GetParameters()));
        }

        public string getDocDomment(FieldInfo field)
        {
            return getDocCommentFor("F:" + field.DeclaringType.FullName + field.Name);
        }

        public string getDocDomment(Type type)
        {
            return getDocCommentFor("T:" + type.FullName);
        }



        private string getDocCommentFor(string nodeName)
        {
            if (this.xmlDoc == null) return "";
            XmlNode node = this.xmlDoc.SelectSingleNode("//member[@name = '" + nodeName + "']");
            if (node == null) return "";
            return Regex.Replace(node.InnerXml, Environment.NewLine + @"\s+", Environment.NewLine).Trim();
        }

        private string getParameters(ParameterInfo[] parameters)
        {
            string outstr = "";
            foreach (ParameterInfo parameterInfo in parameters)
            {
                if (outstr.Length > 0)
                {
                    outstr += ",";
                }

                outstr += parameterInfo.ParameterType.FullName;
            }

            if (outstr.Length > 0)
            {
                outstr = "(" + outstr + ")";
            }

            return outstr;
        }



    }
}
