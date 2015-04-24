using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ManagedToNativeWrapperGenerator
{
    public static class TypeConverter
    {

        [Flags]
        public enum TFlag
        {
            none = 0,
            MarshalingRequired = 1,
            MarshalingInContext = 2,
            WrapperRequired = 4,
            CastRequired = 8,
            ILObject = 16
        }

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


            public TypeTranslation(Type managedType, string nativeType, TFlag ti = TFlag.none)
            {
                _managedType = managedType;
                _nativeType = nativeType;
                _marshalingRequired = ti.HasFlag(TFlag.MarshalingRequired);
                _marshalingInContext = ti.HasFlag(TFlag.MarshalingInContext);
                _wrapperRequired = ti.HasFlag(TFlag.WrapperRequired);
                _castRequired = ti.HasFlag(TFlag.CastRequired);
                _ilObject = ti.HasFlag(TFlag.ILObject);

                if (_nativeType == null) _nativeType = Utils.GetWrapperTypeFullNameFor(managedType) + "*";
                if (_wrapperRequired) _ilbridgeType = Utils.GetWrapperILBridgeTypeFullNameFor(managedType) + "*";
            }

            public TypeTranslation(Type managedType, TFlag ti = TFlag.none)
                : this(managedType, null, ti)
            {
                
            }



            public Type ManagedType
            {
                get { return _managedType; }
            }

            public string NativeType
            {
                get { return _nativeType; }
            }

            public string ILBridgeType
            {
                get { return _ilbridgeType; }
            }

            public bool isMarshalingRequired
            {
                get { return _marshalingRequired; }
            }

            public bool isMarshalingInContext
            {
                get { return _marshalingInContext; }
            }

            public bool isWrapperRequired
            {
                get { return _wrapperRequired; }
            }

            public bool isCastRequired
            {
                get { return _castRequired; }
            }

            public bool isILObject
            {
                get { return _ilObject; }
            }

        }

        static Dictionary<Type, TypeTranslation> _standardTranslations = new Dictionary<Type, TypeTranslation>();

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
            translations.Add(new TypeTranslation(typeof(string), "std::wstring", TFlag.MarshalingRequired));
            translations.Add(new TypeTranslation(typeof(void), "void"));
            // TODO: More translations (decimal?)

            foreach (TypeTranslation translation in translations)
            {
                _standardTranslations.Add(translation.ManagedType, translation);
            }
        }

        public static TypeTranslation TranslateParameterType(Type parameterType)
        {
            TypeTranslation translation;
            if (_standardTranslations.TryGetValue(parameterType, out translation))
                return translation;

            if (parameterType.IsArray)
            {
                Type t = parameterType;

                string pointers = "";
                while (t.IsArray && t.HasElementType)
                {
                    for (int i = 0; i < t.GetArrayRank(); ++i)
                        pointers += "*";

                    t = t.GetElementType();
                }

                if (_standardTranslations.TryGetValue(t, out translation))
                {
                    // Array of something simple
                    return new TypeTranslation(parameterType, translation.NativeType + pointers, TFlag.MarshalingRequired);
                }
                else
                {
                    // Array of objects
                    return new TypeTranslation(parameterType, Utils.GetWrapperTypeFullNameFor(t) + pointers + "*", TFlag.MarshalingRequired | TFlag.WrapperRequired | TFlag.ILObject);
                }
            }

            //TODO: Try more things, like collections
            //TODO: Convert structures recursively?

            // Objects
            if (!parameterType.IsValueType)
            {
                return new TypeTranslation(parameterType, TFlag.MarshalingRequired | TFlag.WrapperRequired | TFlag.ILObject);
            }

            // e.g. Enum values
            return new TypeTranslation(parameterType, Utils.GetWrapperTypeFullNameFor(parameterType), TFlag.CastRequired | TFlag.WrapperRequired);
        }
    }

    public static class Utils
    {
        public static string GetCppCliTypeNameFor(Type type)
        {
            return type.FullName.Replace(".", "::").Replace("+","::");
        }

        public static string GetCppCliNamespaceFor(Type type)
        {
            if (type.Namespace == null) return "";
            return type.Namespace.Replace(".", "::");
        }

        public static string GetCppCliNamespacePrefixFor(Type type)
        {
            string ns = GetCppCliNamespaceFor(type);
            return ns.Length == 0 ? ns : ns + "::";
        }


        public static string GetCppCliTypeFor(Type type)
        {
            string bestEffort;

            if (type.IsArray)
            {
                bestEffort = "array<" 
                                + GetCppCliTypeFor(type.GetElementType()) 
                                + (type.GetArrayRank() > 1 ? ("," + type.GetArrayRank()) : "") 
                                + ">";
            }
            else 
            {
                bestEffort = GetCppCliTypeNameFor(type);
            }

            if (!type.IsValueType)
            {
                return bestEffort + "^";
            }

            return bestEffort;
        }



        public static string GetLocalTempNameFor(ParameterInfo parameter)
        {
            return GetLocalTempNameFor(parameter.Name);
        }

        public static string GetLocalTempNameFor(String name)
        {
            return "__Param_" + name;
        }

        public static string GetLocalTempNameForReturn()
        {
            return "__ReturnVal";
        }



        public static string GetWrapperProjectName()
        {
            return "Wrapper";
        }

        public static string GetWrapperTypeNameFor(Type type)
        {
            return "Wrapper_" + type.Name.Replace('.', '_');
        }

        public static string GetWrapperILBridgeTypeNameFor(Type type)
        {
            return GetWrapperTypeNameFor(type) + "_IL";
        }



        public static string GetWrapperTypeFullNameFor(Type type)
        {
            return GetCppCliNamespacePrefixFor(type) + GetWrapperTypeNameFor(type);
        }

        public static string GetWrapperILBridgeTypeFullNameFor(Type type)
        {
            return GetCppCliNamespacePrefixFor(type) + GetWrapperILBridgeTypeNameFor(type);
        }


    }
}