#pragma once
#pragma managed
#include <msclr\auto_gcroot.h>

#using "TestSuite.dll"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Simple.h"

namespace Wrapper {
namespace MyCorp {
namespace TheProduct {
namespace SomeModule {
namespace Utilities {

class Simple_IL {
	public:
		msclr::auto_gcroot<::MyCorp::TheProduct::SomeModule::Utilities::Simple^> __Impl;
};

}
}
}
}
}

template <typename TTo> 
inline Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* marshal_as(::MyCorp::TheProduct::SomeModule::Utilities::Simple const from)
{
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple_IL* bridge = new Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple_IL;
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* wrapper = new Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple(bridge);
	bridge->__Impl = from;
	return wrapper;
}

template <typename TTo> 
inline ::MyCorp::TheProduct::SomeModule::Utilities::Simple marshal_as(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* const from)
{
	return (::MyCorp::TheProduct::SomeModule::Utilities::Simple) from->__IL->__Impl.get();
}
