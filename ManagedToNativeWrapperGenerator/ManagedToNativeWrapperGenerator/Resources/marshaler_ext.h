#pragma once
#pragma managed
#include <msclr\marshal_cppstd.h>

using namespace msclr::interop;


//--------------------------------------------------------------------------------
// Helper structs
//--------------------------------------------------------------------------------

template<typename T> struct remove_pointer_ex { typedef T type; };
template<typename T> struct remove_pointer_ex<T*> { typedef T type; };


//--------------------------------------------------------------------------------
// Helper classes
//--------------------------------------------------------------------------------

/// marshal_as - helper class
template<typename TTo, typename TFrom>
class _marshal_helper {
	public:
		static TTo marshal(const TFrom& from)
		{
			return marshal_as<TTo>(from);
		}    
};
/// same-type copy - helper class
template<typename TTo>
class _marshal_helper<TTo, TTo> {
	public:
		static TTo marshal(const TTo& from)
		{
			return from;
		}    
};



//--------------------------------------------------------------------------------
// Conversions templates for arrays
//--------------------------------------------------------------------------------

/// cli::array<> -> array*
template <typename TTo, typename TFrom> inline TTo marshal_as(cli::array<TFrom>^ const from)
{
    TTo __ReturnVal = new remove_pointer_ex<TTo>::type[from->Length]; 

    for(int i = 0; i < from->Length; i++) {
        TFrom s = from[i];
        __ReturnVal[i] = _marshal_as<remove_pointer_ex<TTo>::type, TFrom>(s);
    }
    
    return __ReturnVal;
}

/// cli::array<,2> -> array*
template <typename TTo, typename TFrom> inline TTo marshal_as(cli::array<TFrom,2>^ const from)
{
    int len = from->GetLength(0);
    int len2 = from->GetLength(1);
    
    TTo __ReturnVal = new remove_pointer_ex<TTo>::type[len]; 

    for(int i = 0; i < len; i++) {
        __ReturnVal[i] = new remove_pointer_ex<remove_pointer_ex<TTo>::type>::type[len2]; 
        
        for(int j = 0; j < len2; j++) {
            TFrom s = from[i,j];
            __ReturnVal[i][j] = _marshal_as<remove_pointer_ex<remove_pointer_ex<TTo>::type>::type, TFrom>(s);
        }
    } 
    
    return __ReturnVal;
}



/// cli::array<> -> vector<> 
/*template <typename TTo, typename TFrom> inline TTo marshal_as(cli::array<TFrom>^ const from)
{
    TTo __ReturnVal;

    for(int i = 0; i < from->Length; i++) {
        TFrom _elm = from[i];
        __ReturnVal.push_back(_marshal_as<TTo::value_type>(_elm));
    }
    
    return __ReturnVal;
}
*/



/// array* -> cli::array<> 
template <typename TTo, typename TFrom> inline cli::array<TTo>^ marshal_as(TFrom* const from, size_t len)
{
    cli::array<TTo>^ __ReturnVal = gcnew cli::array<TTo>(len);

    for(int i = 0; i < len; i++) {
        TFrom s = from[i];
        __ReturnVal[i] = _marshal_as<TTo>(s);
    }
    
    return __ReturnVal;
}


/// array* -> cli::array<,2> 
template <typename TTo, typename TFrom> inline cli::array<TTo,2>^ marshal_as(TFrom** const from, size_t len, size_t len2)
{
    cli::array<TTo,2>^ __ReturnVal = gcnew cli::array<TTo,2>(len, len2);

    for(int i = 0; i < len; i++) {
        for(int j = 0; j < len2; j++) {
            TFrom s = from[i][j];
            __ReturnVal[i,j] = _marshal_as<TTo>(s);
        }
    }
    
    return __ReturnVal;
}


/*

template <typename T> struct wr_array { 
    size_t length; 
    T* values; 
}; 

*/




//--------------------------------------------------------------------------------
// Entry function for marshalling
//--------------------------------------------------------------------------------
template <typename TTo, typename TFrom>
inline TTo _marshal_as(const TFrom& from)
{
	return _marshal_helper<TTo, TFrom>::marshal(from);
}

template <typename TTo, typename TFrom>
inline cli::array<TTo>^ _marshal_as(const TFrom& from, size_t len)
{
	return marshal_as<TTo>(from, len);
}

template <typename TTo, typename TFrom>
inline cli::array<TTo,2>^ _marshal_as(const TFrom& from, size_t len, size_t len2)
{
	return marshal_as<TTo>(from, len, len2);
}
