#pragma once
#pragma managed

#include "marshaller_ext.h"

#define _LNK __declspec(dllexport)
#using "YahooAPI.dll"
#include "Wrapper_YahooAPI_IL.h"
#include "Wrapper_PrintCallback_IL.h"

namespace Wrapper {

YahooAPI::YahooAPI() {
	__IL = new YahooAPI_IL;
	__IL->__Impl = gcnew ::YahooAPI();
}

YahooAPI::YahooAPI(YahooAPI_IL* IL) {
	__IL = IL;
}

YahooAPI::~YahooAPI() {
	delete __IL;
}

double YahooAPI::GetAsk(const std::wstring& symbol) {
	::System::String^ __Param_symbol = _marshal_as<::System::String^>(symbol);
	::System::Double __ReturnVal = __IL->__Impl->GetAsk(__Param_symbol);
	return __ReturnVal;
}

double YahooAPI::GetBid(const std::wstring& symbol) {
	::System::String^ __Param_symbol = _marshal_as<::System::String^>(symbol);
	::System::Double __ReturnVal = __IL->__Impl->GetBid(__Param_symbol);
	return __ReturnVal;
}

std::wstring YahooAPI::GetCapitalization(const std::wstring& symbol) {
	::System::String^ __Param_symbol = _marshal_as<::System::String^>(symbol);
	::System::String^ __ReturnVal = __IL->__Impl->GetCapitalization(__Param_symbol);
	std::wstring __ReturnValMarshaled = _marshal_as<std::wstring>(__ReturnVal);
	return __ReturnValMarshaled;
}

int YahooAPI::GetStringLength(const std::wstring& str) {
	::System::String^ __Param_str = _marshal_as<::System::String^>(str);
	::System::Int32 __ReturnVal = __IL->__Impl->GetStringLength(__Param_str);
	return __ReturnVal;
}

std::vector<std::wstring> YahooAPI::GetValues(const std::wstring& symbol, const std::wstring& fields) {
	::System::String^ __Param_symbol = _marshal_as<::System::String^>(symbol);
	::System::String^ __Param_fields = _marshal_as<::System::String^>(fields);
	array<::System::String^>^ __ReturnVal = __IL->__Impl->GetValues(__Param_symbol, __Param_fields);
	std::vector<std::wstring> __ReturnValMarshaled = _marshal_as<std::vector<std::wstring>>(__ReturnVal);
	return __ReturnValMarshaled;
}

std::wstring YahooAPI::test() {
	::System::String^ __ReturnVal = __IL->__Impl->test();
	std::wstring __ReturnValMarshaled = _marshal_as<std::wstring>(__ReturnVal);
	return __ReturnValMarshaled;
}

void YahooAPI::test(const std::wstring& mode, void (*cb)(const std::wstring& str)) {
	::System::String^ __Param_mode = _marshal_as<::System::String^>(mode);
	::YahooAPI::PrintCallback^ __Param_cb = _marshal_as<::YahooAPI::PrintCallback^>(cb);
	__IL->__Impl->test(__Param_mode, __Param_cb);
}

}
