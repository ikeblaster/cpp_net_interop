#pragma once
#pragma managed
#include <msclr\marshal_cppstd.h>
#include <vector>

using namespace msclr::interop;


//--------------------------------------------------------------------------------
// Helper structs
//--------------------------------------------------------------------------------

template<typename T> struct underlying_type_of { typedef T type; };
template<typename T> struct underlying_type_of<std::vector<T>> { typedef T type; };
//template<typename T> struct underlying_type_of<cli::array<T>^> { typedef T type; };
//template<typename T> struct underlying_type_of<cli::array<T,2>^> { typedef T type; };
//template<typename T> struct remove_pointer_ex { typedef T type; };
//template<typename T> struct remove_pointer_ex<T*> { typedef T type; };
template<typename T> struct remove_hat { typedef T type; };
template<typename T> struct remove_hat<T^> { typedef T type; };



//--------------------------------------------------------------------------------
// Helper classes
//--------------------------------------------------------------------------------

/// marshal_as - helper class
template<typename TTo, typename TFrom>
struct _marshal_helper {
    inline static TTo marshal(const TFrom& from) 
    {
        return marshal_as<TTo>(from);
    }    
};

/// cli::array<> -> std::vector<> helpers
template<typename TTo, typename TFrom>
struct _marshal_helper<cli::array<TTo>^, TFrom> {
    inline static cli::array<TTo>^ marshal(const TFrom& from)
    {
        return marshal_as_1d<TTo>(from);
    }    
};
template<typename TTo, typename TFrom>
struct _marshal_helper<cli::array<TTo,2>^, TFrom> {
    inline static cli::array<TTo,2>^ marshal(const TFrom& from)
    {
        return marshal_as_2d<TTo>(from);
    }    
}; 

/// std::vector<> -> CLR Collections helpers
template<typename TTo, typename TFrom>
struct _marshal_helper<System::Collections::Generic::ICollection<TTo>^, TFrom> {
    inline static System::Collections::Generic::ICollection<TTo>^ marshal(const TFrom& from)
    {
        return marshal_as_clr<System::Collections::Generic::List<TTo>^>(from); // use List as default ICollection
    }    
}; 
template<typename TTo, typename TFrom>
struct _marshal_helper<System::Collections::Generic::List<TTo>^, TFrom> {
    inline static System::Collections::Generic::List<TTo>^ marshal(const TFrom& from)
    {
        return marshal_as_clr<System::Collections::Generic::List<TTo>^>(from);
    }    
}; 
template<typename TTo, typename TFrom>
struct _marshal_helper<System::Collections::Generic::LinkedList<TTo>^, TFrom> {
    inline static System::Collections::Generic::LinkedList<TTo>^ marshal(const TFrom& from)
    {
        return marshal_as_clr<System::Collections::Generic::LinkedList<TTo>^>(from);
    }    
}; 
template<typename TTo, typename TFrom>
struct _marshal_helper<System::Collections::Generic::Queue<TTo>^, TFrom> {
    inline static System::Collections::Generic::Queue<TTo>^ marshal(const TFrom& from)
    {
        return marshal_as_clr<System::Collections::Generic::Queue<TTo>^>(from);
    }    
}; 
template<typename TTo, typename TFrom>
struct _marshal_helper<System::Collections::Generic::HashSet<TTo>^, TFrom> {
    inline static System::Collections::Generic::HashSet<TTo>^ marshal(const TFrom& from)
    {
        return marshal_as_clr<System::Collections::Generic::HashSet<TTo>^>(from);
    }    
}; 
template<typename TTo, typename TFrom>
struct _marshal_helper<System::Collections::Generic::SortedSet<TTo>^, TFrom> {
    inline static System::Collections::Generic::SortedSet<TTo>^ marshal(const TFrom& from)
    {
        return marshal_as_clr<System::Collections::Generic::SortedSet<TTo>^>(from);
    }    
}; 
template<typename TTo, typename TFrom>
struct _marshal_helper<System::Collections::Generic::Stack<TTo>^, TFrom> {
    inline static System::Collections::Generic::Stack<TTo>^ marshal(const TFrom& from)
    {
        return marshal_as_clr<System::Collections::Generic::Stack<TTo>^>(from);
    }    
}; 




/// same-type copy - helper class
template<typename TTo>
struct _marshal_helper<TTo, TTo> {
    inline static TTo marshal(const TTo& from)
    {
        return from;
    }    
};



//--------------------------------------------------------------------------------
// Entry function for marshalling
//--------------------------------------------------------------------------------

template <typename TTo, typename TFrom>
inline TTo _marshal_as(const TFrom& from)
{
	return _marshal_helper<TTo, TFrom>::marshal(from);
}



//--------------------------------------------------------------------------------
// Conversions templates for arrays
//--------------------------------------------------------------------------------

/// cli::array<> -> std::vector<> 
template <typename TTo, typename TFrom> inline TTo marshal_as(cli::array<TFrom>^ const from)
{
    typedef underlying_type_of<TTo>::type TTo2; 
    size_t len = from->Length;
    TTo __ReturnVal(len);
    
    for(size_t i = 0; i < len; i++) {
        TFrom s = from[i];
        __ReturnVal[i] = _marshal_as<TTo2>(s);
    }
    
    return __ReturnVal;
} 


/// cli::array<,2> -> std::vector<std::vector<>> 
template <typename TTo, typename TFrom> inline TTo marshal_as(cli::array<TFrom,2>^ const from)
{
    typedef underlying_type_of<TTo>::type TTo2; 
    typedef underlying_type_of<TTo2>::type TTo3;
    size_t len = from->GetLength(0);
    size_t len2 = from->GetLength(1);
    TTo __ReturnVal(len, TTo2(len2));
    
    for(size_t i = 0; i < len; i++) {
        for(size_t j = 0; j < len2; j++) {
            TFrom s = from[i,j];
            __ReturnVal[i][j] = _marshal_as<TTo3>(s);
        }
    }  
    
    return __ReturnVal;
}


/// std::vector<> -> cli::array<> 
template<typename TTo, typename TFrom> inline cli::array<TTo>^ marshal_as_1d(std::vector<TFrom> const from)
{
    size_t len = from.size();
    cli::array<TTo>^ __ReturnVal = gcnew cli::array<TTo>(len);

    for(size_t i = 0; i < len; i++) {
        __ReturnVal[i] = _marshal_as<TTo>(from[i]);
    }

    return __ReturnVal;
}

/// std::vector<std::vector<>> -> cli::array<,2> 
template <typename TTo, typename TFrom> inline cli::array<TTo,2>^ marshal_as_2d(std::vector<std::vector<TFrom>> const from)
{
    size_t len = from.size();
    size_t len2 = len > 0 ? from[0].size() : 0;
    cli::array<TTo,2>^ __ReturnVal = gcnew cli::array<TTo,2>(len, len2);

    for(int i = 0; i < len; i++) {
        for(int j = 0; j < len2; j++) {
            TFrom s = from[i][j];
            __ReturnVal[i,j] = _marshal_as<TTo>(s);
        }
    }
    
    return __ReturnVal;
}




/// ICollection<> -> std::vector<>
template <typename TTo, typename TFrom> inline TTo marshal_as(System::Collections::Generic::ICollection<TFrom>^ const from)
{
    typedef underlying_type_of<TTo>::type TTo2;
    
    TTo __ReturnVal;
    __ReturnVal.reserve(from->Count);
    
    for each (TFrom elem in from) {
        __ReturnVal.push_back(_marshal_as<TTo2>(elem));
    }

    return __ReturnVal;
}
template <typename TTo, typename TFrom> inline TTo marshal_as(System::Collections::Generic::List<TFrom>^ const from) {
    return marshal_as<TTo>((System::Collections::Generic::ICollection<TFrom>^) from);
}
template <typename TTo, typename TFrom> inline TTo marshal_as(System::Collections::Generic::LinkedList<TFrom>^ const from) {
    return marshal_as<TTo>((System::Collections::Generic::ICollection<TFrom>^) from);
}
template <typename TTo, typename TFrom> inline TTo marshal_as(System::Collections::Generic::Queue<TFrom>^ const from) {
    return marshal_as<TTo>((System::Collections::Generic::ICollection<TFrom>^) from);
}
template <typename TTo, typename TFrom> inline TTo marshal_as(System::Collections::Generic::HashSet<TFrom>^ const from) {
    return marshal_as<TTo>((System::Collections::Generic::ICollection<TFrom>^) from);
}
template <typename TTo, typename TFrom> inline TTo marshal_as(System::Collections::Generic::SortedSet<TFrom>^ const from) {
    return marshal_as<TTo>((System::Collections::Generic::ICollection<TFrom>^) from);
}
template <typename TTo, typename TFrom> inline TTo marshal_as(System::Collections::Generic::Stack<TFrom>^ const from) {
    return marshal_as<TTo>((System::Collections::Generic::ICollection<TFrom>^) from);
}


/// std::vector<> -> ICollection<>
template <typename TTo, typename TFrom> inline TTo marshal_as_clr(std::vector<TFrom> const from)
{
    typedef remove_hat<TTo>::type TTo2;
    
	TTo result = gcnew TTo2();
	typedef decltype(System::Linq::Enumerable::First(result)) TTo3;
    
    for each (TFrom elem in from) {
        result->Add(_marshal_as<TTo3>(elem));
    }
    
	return result;
}

