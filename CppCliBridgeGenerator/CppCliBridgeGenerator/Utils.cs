using System;
using System.Linq;
using System.Reflection;

namespace CppCliBridgeGenerator
{
    /// <summary>
    /// Collection of static utils mainly for manipulation with element names.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Get raw full name for type usable in C++/CLI.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Formatted fullname</returns>
        public static string GetCppCliTypeFullNameFor(Type type)
        {
            string fullname = "::" + type.FullName.Replace(".", "::").Replace("+", "::").Split('`')[0];
            if (type.IsGenericType) fullname += "<" + string.Join(", ", type.GetGenericArguments().Select(arg => GetCppCliTypeFor(arg))) + ">";

            return fullname;
        }

        /// <summary>
        /// Get raw namespace (inline) for type usable in C++/CLI
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Formatted namespace</returns>
        public static string GetCppCliNamespaceFor(Type type)
        {
            if (type.Namespace == null) return "";
            return type.Namespace.Replace(".", "::");
        }

        /// <summary>
        /// Get raw namespace (inline) prefix for type usable in C++/CLI.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Formatted namespace prefix - ie. "NS::" or ""</returns>
        public static string GetCppCliNamespacePrefixFor(Type type)
        {
            if (type.Namespace == null) return "";
            return GetCppCliNamespaceFor(type) + "::";
        }


        /// <summary>
        /// Get type usable in C++/CLI.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Formatted type</returns>
        public static string GetCppCliTypeFor(Type type)
        {
            string cppcli;

            // Generic types, but not collections (which are translated)
            if (type.IsArray) // Arrays
            {
                cppcli = "array<"
                                + GetCppCliTypeFor(type.GetElementType())
                                + (type.GetArrayRank() > 1 ? ("," + type.GetArrayRank()) : "")
                                + ">";
            }
            else // Everything else
            {
                cppcli = GetCppCliTypeFullNameFor(type);
            }

            if (!type.IsValueType)
            {
                return cppcli + "^"; // add "hat" for objects
            }

            return cppcli;
        }


        /// <summary>
        /// Get name for temporary local variable - parameter.
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>Variable name</returns>
        public static string GetLocalTempNameFor(ParameterInfo parameter)
        {
            return GetLocalTempNameFor(parameter.Name);
        }

        /// <summary>
        /// Get name for temporary local variable.
        /// </summary>
        /// <param name="name">Original variable name</param>
        /// <returns>Variable name</returns>
        public static string GetLocalTempNameFor(String name)
        {
            return "__Param_" + name;
        }

        /// <summary>
        /// Get name for temporary local variable for return value.
        /// </summary>
        /// <returns>Variable name</returns>
        public static string GetLocalTempNameForReturn()
        {
            return "__ReturnVal";
        }



        /// <summary>
        /// Get wrapper name (can be used for project name, namespace wrapper, prefixes/postfixes, ...).
        /// </summary>
        public static string GetWrapperProjectName()
        {
            return "Wrapper";
        }

        /// <summary>
        /// Get bridge name for type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Formatted name</returns>
        public static string GetWrapperTypeNameFor(Type type)
        {
            string name = type.Name.Replace("+", "_").Split('`')[0];

            if(type.IsGenericType)
                name += "_" + string.Join("_", type.GetGenericArguments().Select(arg => GetCppCliTypeFullNameFor(arg).Replace("::", "_")));

            return name;
        }

        /// <summary>
        /// Get IL bridge name for type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Formatted name</returns>
        public static string GetWrapperILBridgeTypeNameFor(Type type)
        {
            return GetWrapperTypeNameFor(type) + "_IL";
        }


        /// <summary>
        /// Get bridge fullname for type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Formatted name</returns>
        public static string GetWrapperTypeFullNameFor(Type type)
        {
            return GetWrapperProjectName() + "::" + GetCppCliNamespacePrefixFor(type) + GetWrapperTypeNameFor(type);
        }

        /// <summary>
        /// Get IL bridge fullname for type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Formatted name</returns>
        public static string GetWrapperILBridgeTypeFullNameFor(Type type)
        {
            return GetWrapperProjectName() + "::" + GetCppCliNamespacePrefixFor(type) + GetWrapperILBridgeTypeNameFor(type);
        }


        /// <summary>
        /// Get bridge filename for type without extension.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Filename</returns>
        public static string GetWrapperFileNameFor(Type type)
        {
            return GetWrapperProjectName() + "_" + GetCppCliNamespacePrefixFor(type).Replace("::","_") + GetWrapperTypeNameFor(type);
        }
        /// <summary>
        /// Get IL bridge filename for type without extension.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Filename</returns>
        public static string GetWrapperILBridgeFileNameFor(Type type)
        {
            return GetWrapperProjectName() + "_" + GetCppCliNamespacePrefixFor(type).Replace("::", "_") + GetWrapperILBridgeTypeNameFor(type);
        }


        /// <summary>
        /// Indent all lines with some text.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="spacer">Text used as indentation</param>
        /// <returns>Indented text</returns>
        public static string IndentText(string text, string spacer)
        {
            if (text.Length == 0) return text;
            return spacer + text.Replace(Environment.NewLine, Environment.NewLine + spacer);
        }
    }
}