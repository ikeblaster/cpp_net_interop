#pragma once
#pragma managed
#include <msclr\auto_gcroot.h>

#using "Arrays.dll"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Stuff.h"

namespace Wrapper {
namespace MyCorp {
namespace TheProduct {
namespace SomeModule {
namespace Utilities {

class Stuff_IL {
	public:
		msclr::auto_gcroot<::MyCorp::TheProduct::SomeModule::Utilities::Stuff^> __Impl;
};

}
}
}
}
}

template <typename TTo> 
inline Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* marshal_as(::MyCorp::TheProduct::SomeModule::Utilities::Stuff^ const from)
{
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff_IL* bridge = new Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff_IL;
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* wrapper = new Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff(bridge);
	bridge->__Impl = from;
	return wrapper;
}

template <typename TTo> 
inline ::MyCorp::TheProduct::SomeModule::Utilities::Stuff^ marshal_as(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* const from)
{
	return (::MyCorp::TheProduct::SomeModule::Utilities::Stuff^) from->__IL->__Impl.get();
}
