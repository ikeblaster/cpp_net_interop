using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CppCliBridgeGenerator
{
    /// <summary>
    /// Collection of static utils mainly for manipulation with element names.
    /// </summary>
    public static class Utils
    {

        private static HashSet<string> _keywords = new HashSet<string>() { "abstract", "add", "alignas", "alignof", "and", "and_eq", "array", "as", "ascending", "asm", "async", "auto", "await", "base", "bitand", "bitor", "bool", "break", "by", "byte", "case", "catch", "char", "char16_t", "char32_t", "checked", "class", "compl", "const", "constexpr", "const_cast", "continue", "decimal", "decltype", "default", "delegate", "delete", "descending", "do", "double", "dynamic", "dynamic_cast", "else", "enum", "enum ", "equals", "explicit", "export", "extern", "false", "final", "finally", "fixed", "float", "for", "foreach", "friend", "from", "get", "global", "goto", "group", "if", "implicit", "in", "inline", "int", "interface", "internal", "into", "is", "join", "let", "lock", "long", "mutable", "namespace", "new", "noexcept", "not", "not_eq", "null", "nullptr", "object", "on", "operator", "or", "orderby", "or_eq", "out", "override", "params", "partial", "private", "protected", "public", "readonly", "ref", "register", "reinterpret_cast", "remove", "restrict", "return", "return ", "sbyte", "sealed", "select", "set", "short", "signed", "sizeof", "stackalloc", "static", "static_assert", "static_cast", "string", "struct", "switch", "template", "this", "thread_local", "throw", "true", "try", "typedef", "typeid", "typename", "typeof", "uint", "ulong", "unchecked", "union", "unsafe", "unsigned", "ushort", "using", "var", "virtual", "void", "volatile", "wchar_t", "where", "while", "xor", "xor_eq", "yield", "_Bool", "_Complex", "_Imaginary" }; 


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
        /// Get name for parameter - parameter.
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>Parameter name</returns>
        public static string GetParNameFor(ParameterInfo parameter)
        {
            return GetParNameFor(parameter.Name);
        }

        /// <summary>
        /// Get name for parameter.
        /// </summary>
        /// <param name="name">Original variable name</param>
        /// <returns>Parameter name</returns>
        public static string GetParNameFor(String name)
        {
            if (_keywords.Contains(name)) return name + "_";
            return name;
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
            string name = type.Name.Replace('+', '_').Replace('[','_').Replace(']','_').Split('`')[0];

            if(type.IsGenericType)
                name += "_" + string.Join("_", type.GetGenericArguments().Select(arg => arg.Namespace.Replace(".", "_") + GetWrapperTypeNameFor(arg)));

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