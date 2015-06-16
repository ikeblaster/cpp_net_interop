#pragma once
#pragma managed
#include <msclr\auto_gcroot.h>

#using "TestSuite.dll"

namespace Wrapper {
namespace MyCorp {
namespace TheProduct {
namespace SomeModule {
namespace Utilities {

ref class PrintCallback_IL {
	public:

		std::wstring (*cb)(const std::wstring& str, int i);

		PrintCallback_IL(std::wstring (*cb)(const std::wstring& str, int i)) {
			this->cb = cb;
		}

		::System::String^ Invoke(::System::String^ str, ::System::Int32 i) {
			std::wstring __Param_str = _marshal_as<std::wstring>(str);
			int __Param_i = i;
			std::wstring __ReturnVal = this->cb(__Param_str, __Param_i);
			::System::String^ __ReturnValMarshaled = _marshal_as<::System::String^>(__ReturnVal);
			return __ReturnValMarshaled;
		}

};

}
}
}
}
}

template <>
struct _marshal_helper<::MyCorp::TheProduct::SomeModule::Utilities::Arrays::PrintCallback^, std::wstring (*)(const std::wstring& str, int i)>
{
	inline static ::MyCorp::TheProduct::SomeModule::Utilities::Arrays::PrintCallback^ marshal(std::wstring (*from)(const std::wstring& str, int i))
	{
		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::PrintCallback_IL^ bridge = gcnew Wrapper::MyCorp::TheProduct::SomeModule::Utilities::PrintCallback_IL(from);
		return gcnew ::MyCorp::TheProduct::SomeModule::Utilities::Arrays::PrintCallback(bridge, &Wrapper::MyCorp::TheProduct::SomeModule::Utilities::PrintCallback_IL::Invoke);
	}
};
