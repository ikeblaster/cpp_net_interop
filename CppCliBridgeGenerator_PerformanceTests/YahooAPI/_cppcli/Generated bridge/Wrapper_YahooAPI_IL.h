#pragma once
#pragma managed
#include <msclr\auto_gcroot.h>

#using "YahooAPI.dll"
#include "Wrapper_YahooAPI.h"

namespace Wrapper {

class YahooAPI_IL {
	public:
		msclr::auto_gcroot<::YahooAPI^> __Impl;
};

}

template <typename TTo> 
inline Wrapper::YahooAPI* marshal_as(::YahooAPI^ const from)
{
	Wrapper::YahooAPI_IL* bridge = new Wrapper::YahooAPI_IL;
	Wrapper::YahooAPI* wrapper = new Wrapper::YahooAPI(bridge);
	bridge->__Impl = from;
	return wrapper;
}

template <typename TTo> 
inline ::YahooAPI^ marshal_as(Wrapper::YahooAPI* const from)
{
	return (::YahooAPI^) from->__IL->__Impl.get();
}
