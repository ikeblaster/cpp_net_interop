#pragma once
#pragma managed
#include <msclr\auto_gcroot.h>

#using "Arrays.dll"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Arrays.h"

namespace Wrapper {
namespace MyCorp {
namespace TheProduct {
namespace SomeModule {
namespace Utilities {

class Arrays_IL {
	public:
		msclr::auto_gcroot<::MyCorp::TheProduct::SomeModule::Utilities::Arrays^> __Impl;
};

}
}
}
}
}

template <typename TTo> 
inline Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Arrays* marshal_as(::MyCorp::TheProduct::SomeModule::Utilities::Arrays^ const from)
{
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Arrays_IL* bridge = new Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Arrays_IL;
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Arrays* wrapper = new Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Arrays(bridge);
	bridge->__Impl = from;
	return wrapper;
}

template <typename TTo> 
inline ::MyCorp::TheProduct::SomeModule::Utilities::Arrays^ marshal_as(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Arrays* const from)
{
	return (::MyCorp::TheProduct::SomeModule::Utilities::Arrays^) from->__IL->__Impl.get();
}
