#pragma once
#pragma managed
#include <msclr\auto_gcroot.h>

#using "YahooAPI.dll"

namespace Wrapper {

ref class PrintCallback_IL {
	public:

		void (*cb)(const std::wstring& str);

		PrintCallback_IL(void (*cb)(const std::wstring& str)) {
			this->cb = cb;
		}

		::System::Void Invoke(::System::String^ str) {
			std::wstring __Param_str = _marshal_as<std::wstring>(str);
			this->cb(__Param_str);
		}

};

}

template <>
struct _marshal_helper<::YahooAPI::PrintCallback^, void (*)(const std::wstring& str)>
{
	inline static ::YahooAPI::PrintCallback^ marshal(void (*from)(const std::wstring& str))
	{
		Wrapper::PrintCallback_IL^ bridge = gcnew Wrapper::PrintCallback_IL(from);
		return gcnew ::YahooAPI::PrintCallback(bridge, &Wrapper::PrintCallback_IL::Invoke);
	}
};
