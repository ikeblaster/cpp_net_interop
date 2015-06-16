#pragma once
#pragma managed
#include <msclr\auto_gcroot.h>

#using "TestSuite.dll"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Genericclass__System_String.h"

namespace Wrapper {
namespace MyCorp {
namespace TheProduct {
namespace SomeModule {
namespace Utilities {

class Genericclass__System_String_IL {
	public:
		msclr::auto_gcroot<::MyCorp::TheProduct::SomeModule::Utilities::Genericclass<::System::String^>^> __Impl;
};

}
}
}
}
}

template <typename TTo> 
inline Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Genericclass__System_String* marshal_as(::MyCorp::TheProduct::SomeModule::Utilities::Genericclass<::System::String^>^ const from)
{
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Genericclass__System_String_IL* bridge = new Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Genericclass__System_String_IL;
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Genericclass__System_String* wrapper = new Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Genericclass__System_String(bridge);
	bridge->__Impl = from;
	return wrapper;
}

template <typename TTo> 
inline ::MyCorp::TheProduct::SomeModule::Utilities::Genericclass<::System::String^>^ marshal_as(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Genericclass__System_String* const from)
{
	return (::MyCorp::TheProduct::SomeModule::Utilities::Genericclass<::System::String^>^) from->__IL->__Impl.get();
}
