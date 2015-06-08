using System;
using System.Collections.Generic;
using System.Reflection;

namespace CppCliBridgeGenerator
{
    public static class TypeConverter
    {

        /// <summary>
        /// Flags for different types of working with "types" (classes, structs, ...)
        /// </summary>
        [Flags]
        public enum TFlags
        {
            None = 0,
            MarshalingRequired = 1,
            MarshalingInContext = 2,
            WrapperRequired = 4,
            CastRequired = 8,
            ILObject = 16
        }

        /// <summary>
        /// Type translation helper - wrapper class.
        /// It is designed to hold information about types and their translation in managed (c++/cli) and unmanaged (c++) "versions".
        /// </summary>
        public class TypeTranslation
        {
            private readonly Type _managedType;
            private readonly string _nativeType;
            private readonly string _ilbridgeType;
            private readonly bool _marshalingRequired;
            private readonly bool _marshalingInContext;
            private readonly bool _wrapperRequired;
            private readonly bool _castRequired;
            private readonly bool _ilObject;


            public TypeTranslation(Type managedType, string nativeType, TFlags ti = TFlags.None)
            {
                this._managedType = managedType;
                this._nativeType = nativeType;
                this._marshalingRequired = ti.HasFlag(TFlags.MarshalingRequired);
                this._marshalingInContext = ti.HasFlag(TFlags.MarshalingInContext);
                this._wrapperRequired = ti.HasFlag(TFlags.WrapperRequired);
                this._castRequired = ti.HasFlag(TFlags.CastRequired);
                this._ilObject = ti.HasFlag(TFlags.ILObject);

                if (this._nativeType == null) this._nativeType = Utils.GetWrapperTypeFullNameFor(managedType) + "*";
                if (this._wrapperRequired) this._ilbridgeType = Utils.GetWrapperILBridgeTypeFullNameFor(managedType) + "*";
            }

            public TypeTranslation(Type managedType, TFlags ti = TFlags.None)
                : this(managedType, null, ti)
            {
            }


            public Type ManagedType
            {
                get { return this._managedType; }
            }

            public string NativeType
            {
                get { return this._nativeType; }
            }

            public string ILBridgeType
            {
                get { return this._ilbridgeType; }
            }

            public bool IsMarshalingRequired
            {
                get { return this._marshalingRequired; }
            }

            public bool IsMarshalingInContext
            {
                get { return this._marshalingInContext; }
            }

            public bool IsWrapperRequired
            {
                get { return this._wrapperRequired; }
            }

            public bool IsCastRequired
            {
                get { return this._castRequired; }
            }

            public bool IsILObject
            {
                get { return this._ilObject; }
            }

        }

        /// <summary>
        /// Dictionary with blittable types translations.
        /// </summary>
        static Dictionary<Type, TypeTranslation> _standardTranslations = new Dictionary<Type, TypeTranslation>();

        /// <summary>
        /// Type converter/translator static initializer.
        /// Initializes all known blittable types (ints/double/...).
        /// </summary>
        static TypeConverter()
        {
            List<TypeTranslation> translations = new List<TypeTranslation>();
            translations.Add(new TypeTranslation(typeof(byte), "unsigned __int8"));
            translations.Add(new TypeTranslation(typeof(sbyte), "__int8"));
            translations.Add(new TypeTranslation(typeof(int), "int"));
            translations.Add(new TypeTranslation(typeof(uint), "unsigned int"));
            translations.Add(new TypeTranslation(typeof(short), "short"));
            translations.Add(new TypeTranslation(typeof(ushort), "unsigned short"));
            translations.Add(new TypeTranslation(typeof(long), "__int64"));
            translations.Add(new TypeTranslation(typeof(ulong), "unsigned __int64"));
            translations.Add(new TypeTranslation(typeof(float), "float"));
            translations.Add(new TypeTranslation(typeof(double), "double"));
            translations.Add(new TypeTranslation(typeof(char), "wchar_t"));
            translations.Add(new TypeTranslation(typeof(bool), "bool"));
            translations.Add(new TypeTranslation(typeof(string), "std::wstring", TFlags.MarshalingRequired));
            translations.Add(new TypeTranslation(typeof(void), "void"));
            // INFO: More translations (decimal?)

            foreach (TypeTranslation translation in translations)
            {
                _standardTranslations.Add(translation.ManagedType, translation);
            }
        }

        /// <summary>
        /// Translates type from managed to unmanaged version.
        /// </summary>
        /// <param name="type">Type (eg. class) to be translated.</param>
        /// <returns>TypeTranslation wrapper class, containing translated version.</returns>
        public static TypeTranslation TranslateType(Type type)
        {
            TypeTranslation translation;

            // look in dictionary with blittable types.
            if (_standardTranslations.TryGetValue(type, out translation))
                return translation;

            // Arrays or collections
            if ((type.IsArray && type.HasElementType) || (type.IsGenericType && type.GetInterface("ICollection") != null))
            {
                TypeTranslation tt;

                string format = @"std::vector<{0}>";

                if (type.IsArray)
                {
                    tt = TranslateType(type.GetElementType());

                    // nested vectors
                    for (int i = 1; i < type.GetArrayRank(); ++i)
                        format = @"std::vector<" + format + @">";
                }
                else
                {
                    tt = TranslateType(type.GetGenericArguments()[0]); // INFO: after all, seems to be safe (collection always have at least one "argument")
                }

                TFlags flags = TFlags.MarshalingRequired;
                if (tt.IsILObject) flags |= TFlags.WrapperRequired | TFlags.ILObject;

                return new TypeTranslation(type, string.Format(format, tt.NativeType), flags);
            }

            // Enums
            if (type.IsEnum)
            {
                return new TypeTranslation(type, Utils.GetWrapperTypeFullNameFor(type), TFlags.CastRequired | TFlags.WrapperRequired);
            }

            // Classes / objects
            return new TypeTranslation(type, TFlags.MarshalingRequired | TFlags.WrapperRequired | TFlags.ILObject);
        }
    }

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
            return "::" + type.FullName.Replace(".", "::").Replace("+","::").Split('`')[0]; // TODO: generic classes usage
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
            if (type.IsGenericType && type.GetInterface("ICollection") != null)
            {
                cppcli = GetCppCliTypeFullNameFor(type) + "<" + GetCppCliTypeFor(type.GetGenericArguments()[0]) + ">";
            }
            else if (type.IsArray) // Arrays
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
            return type.Name.Replace('.', '_');
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
        ///  Get bridge fullname for type.
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
            return GetWrapperProjectName() + "_" + GetWrapperTypeNameFor(type);
        }
        /// <summary>
        /// Get IL bridge filename for type without extension.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Filename</returns>
        public static string GetWrapperILBridgeFileNameFor(Type type)
        {
            return GetWrapperProjectName() + "_" + GetWrapperILBridgeTypeNameFor(type);
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