using System;
using System.Collections.Generic;

namespace CppCliBridgeGenerator
{
    /// <summary>
    /// TypeConverter utils. Can convert/translate types between managed and unmanaged "forms". 
    /// </summary>
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
        /// It is designed to hold information about types and their translation in managed (c++/cli) and unmanaged (c++) "forms".
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
        /// Set of collections supported directly by marshaller
        /// </summary>
        static HashSet<Type> _marshallableCollections = new HashSet<Type>();


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

            _marshallableCollections.Add(typeof(ICollection<>));
            _marshallableCollections.Add(typeof(List<>));
            _marshallableCollections.Add(typeof(LinkedList<>));
            _marshallableCollections.Add(typeof(Queue<>));
            _marshallableCollections.Add(typeof(HashSet<>));
            _marshallableCollections.Add(typeof(SortedSet<>));
            _marshallableCollections.Add(typeof(Stack<>));
        }

        public static bool IsMarshalledCollection(Type type)
        {
            return type.IsGenericType && _marshallableCollections.Contains(type.GetGenericTypeDefinition());
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
            if ((type.IsArray && type.HasElementType) || (IsMarshalledCollection(type)))
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
                    tt = TranslateType(type.GetGenericArguments()[0]); // INFO: after all, seems to be safe (these collections always have one "argument")
                }

                TFlags flags = TFlags.MarshalingRequired;
                if (tt.IsILObject) flags |= TFlags.WrapperRequired | TFlags.ILObject;

                return new TypeTranslation(type, string.Format(format, tt.NativeType), flags);
            }

            // Enums
            if (type.IsEnum)
            {
                return new TypeTranslation(type, Utils.GetWrapperTypeFullNameFor(type) + "::" + type.Name + "Type", TFlags.CastRequired | TFlags.WrapperRequired);
            }

            // Classes / objects
            return new TypeTranslation(type, TFlags.MarshalingRequired | TFlags.WrapperRequired | TFlags.ILObject);
        }
    }
}