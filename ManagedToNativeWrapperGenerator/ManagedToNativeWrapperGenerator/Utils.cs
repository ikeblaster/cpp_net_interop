using System;
using System.Collections.Generic;
using System.Reflection;

namespace ManagedToNativeWrapperGenerator
{
    public static class TypeConverter
    {

        [Flags]
        public enum TFlag
        {
            None = 0,
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


            public TypeTranslation(Type managedType, string nativeType, TFlag ti = TFlag.None)
            {
                this._managedType = managedType;
                this._nativeType = nativeType;
                this._marshalingRequired = ti.HasFlag(TFlag.MarshalingRequired);
                this._marshalingInContext = ti.HasFlag(TFlag.MarshalingInContext);
                this._wrapperRequired = ti.HasFlag(TFlag.WrapperRequired);
                this._castRequired = ti.HasFlag(TFlag.CastRequired);
                this._ilObject = ti.HasFlag(TFlag.ILObject);

                if (this._nativeType == null) this._nativeType = Utils.GetWrapperTypeFullNameFor(managedType) + "*";
                if (this._wrapperRequired) this._ilbridgeType = Utils.GetWrapperILBridgeTypeFullNameFor(managedType) + "*";
            }

            public TypeTranslation(Type managedType, TFlag ti = TFlag.None)
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
                string format = @"{0}";

                while (t.IsArray && t.HasElementType)
                {
                    for (int i = 0; i < t.GetArrayRank(); ++i)
                        format = "std::vector<" + format + ">";

                    t = t.GetElementType();
                }

                if (_standardTranslations.TryGetValue(t, out translation))
                {
                    return new TypeTranslation(parameterType, string.Format(format, translation.NativeType), TFlag.MarshalingRequired); // Array of something simple
                }
                else
                {
                    return new TypeTranslation(parameterType, string.Format(format, Utils.GetWrapperTypeFullNameFor(t) + "*"), TFlag.MarshalingRequired | TFlag.WrapperRequired | TFlag.ILObject); // Array of objects
                }
            }

            // TODO: Try more things, like collections
            // TODO: Convert structures recursively?

            // Enums
            if (parameterType.IsEnum)
            {
                return new TypeTranslation(parameterType, Utils.GetWrapperTypeFullNameFor(parameterType), TFlag.CastRequired | TFlag.WrapperRequired);
            }

            // Objects
            return new TypeTranslation(parameterType, TFlag.MarshalingRequired | TFlag.WrapperRequired | TFlag.ILObject);
        }
    }

    public static class Utils
    {
        public static string GetCppCliTypeFullNameFor(Type type)
        {
            return "::" + type.FullName.Replace(".", "::").Replace("+","::");
        }

        public static string GetCppCliNamespaceFor(Type type)
        {
            if (type.Namespace == null) return "";
            return type.Namespace.Replace(".", "::");
        }

        public static string GetCppCliNamespacePrefixFor(Type type)
        {
            if (type.Namespace == null) return "";
            return GetCppCliNamespaceFor(type) + "::";
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
                bestEffort = GetCppCliTypeFullNameFor(type);
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
            return type.Name.Replace('.', '_');
        }

        public static string GetWrapperILBridgeTypeNameFor(Type type)
        {
            return GetWrapperTypeNameFor(type) + "_IL";
        }



        public static string GetWrapperTypeFullNameFor(Type type)
        {
            return GetWrapperProjectName() + "::" + GetCppCliNamespacePrefixFor(type) + GetWrapperTypeNameFor(type);
        }

        public static string GetWrapperILBridgeTypeFullNameFor(Type type)
        {
            return GetWrapperProjectName() + "::" + GetCppCliNamespacePrefixFor(type) + GetWrapperILBridgeTypeNameFor(type);
        }


        public static string GetWrapperFileNameFor(Type type)
        {
            return GetWrapperProjectName() + "_" + GetWrapperTypeNameFor(type);
        }
        public static string GetWrapperILBridgeFileNameFor(Type type)
        {
            return GetWrapperProjectName() + "_" + GetWrapperILBridgeTypeNameFor(type);
        }
    }
}