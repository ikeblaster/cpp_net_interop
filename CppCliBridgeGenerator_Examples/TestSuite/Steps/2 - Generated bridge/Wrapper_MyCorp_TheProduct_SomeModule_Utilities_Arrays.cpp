#pragma once
#pragma managed

#include "marshaller_ext.h"

#define _LNK __declspec(dllexport)
#using "TestSuite.dll"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Arrays_IL.h"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Simple_IL.h"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_PrintCallback_IL.h"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Genericclass__System_String_IL.h"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Stuff_IL.h"

namespace Wrapper {
namespace MyCorp {
namespace TheProduct {
namespace SomeModule {
namespace Utilities {

Arrays::Arrays(const std::wstring& name) {
	::System::String^ __Param_name = _marshal_as<::System::String^>(name);
	__IL = new Arrays_IL;
	__IL->__Impl = gcnew ::MyCorp::TheProduct::SomeModule::Utilities::Arrays(__Param_name);
}

Arrays::Arrays(const std::wstring& name, const std::wstring& password) {
	::System::String^ __Param_name = _marshal_as<::System::String^>(name);
	::System::String^ __Param_password = _marshal_as<::System::String^>(password);
	__IL = new Arrays_IL;
	__IL->__Impl = gcnew ::MyCorp::TheProduct::SomeModule::Utilities::Arrays(__Param_name, __Param_password);
}

Arrays::Arrays(const std::vector<std::wstring>& names, int password) {
	array<::System::String^>^ __Param_names = _marshal_as<array<::System::String^>^>(names);
	::System::Int32 __Param_password = password;
	__IL = new Arrays_IL;
	__IL->__Impl = gcnew ::MyCorp::TheProduct::SomeModule::Utilities::Arrays(__Param_names, __Param_password);
}

Arrays::Arrays(bool cond, const std::vector<int>& ints) {
	::System::Boolean __Param_cond = cond;
	array<::System::Int32>^ __Param_ints = _marshal_as<array<::System::Int32>^>(ints);
	__IL = new Arrays_IL;
	__IL->__Impl = gcnew ::MyCorp::TheProduct::SomeModule::Utilities::Arrays(__Param_cond, __Param_ints);
}

Arrays::Arrays(Arrays_IL* IL) {
	__IL = IL;
}

Arrays::~Arrays() {
	delete __IL;
}

Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* Arrays::GetS() {
	::MyCorp::TheProduct::SomeModule::Utilities::Simple __ReturnVal = __IL->__Impl->s;
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* __ReturnValMarshaled = _marshal_as<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple*>(__ReturnVal);
	return __ReturnValMarshaled;
}

void Arrays::SetS(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* value) {
	::MyCorp::TheProduct::SomeModule::Utilities::Simple __Param_value = _marshal_as<::MyCorp::TheProduct::SomeModule::Utilities::Simple>(value);
	__IL->__Impl->s = __Param_value;
}

std::wstring Arrays::GetName() {
	::System::String^ __ReturnVal = __IL->__Impl->Name;
	std::wstring __ReturnValMarshaled = _marshal_as<std::wstring>(__ReturnVal);
	return __ReturnValMarshaled;
}

void Arrays::SetName(const std::wstring& value) {
	::System::String^ __Param_value = _marshal_as<::System::String^>(value);
	__IL->__Impl->Name = __Param_value;
}

void Arrays::callbackInvoke() {
	__IL->__Impl->callbackInvoke();
}

void Arrays::callbackSet(const std::wstring& mode, std::wstring (*cb)(const std::wstring& str, int i)) {
	::System::String^ __Param_mode = _marshal_as<::System::String^>(mode);
	::MyCorp::TheProduct::SomeModule::Utilities::Arrays::PrintCallback^ __Param_cb = _marshal_as<::MyCorp::TheProduct::SomeModule::Utilities::Arrays::PrintCallback^>(cb);
	__IL->__Impl->callbackSet(__Param_mode, __Param_cb);
}

void Arrays::Dispose() {
	__IL->__Impl->~Arrays();
}

Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Genericclass__System_String* Arrays::GetGenericWithString() {
	::MyCorp::TheProduct::SomeModule::Utilities::Genericclass<::System::String^>^ __ReturnVal = ::MyCorp::TheProduct::SomeModule::Utilities::Arrays::GetGenericWithString();
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Genericclass__System_String* __ReturnValMarshaled = _marshal_as<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Genericclass__System_String*>(__ReturnVal);
	return __ReturnValMarshaled;
}

int Arrays::GetInt() {
	::System::Int32 __ReturnVal = __IL->__Impl->GetInt();
	return __ReturnVal;
}

std::vector<int> Arrays::GetInts1() {
	array<::System::Int32>^ __ReturnVal = ::MyCorp::TheProduct::SomeModule::Utilities::Arrays::GetInts1();
	std::vector<int> __ReturnValMarshaled = _marshal_as<std::vector<int>>(__ReturnVal);
	return __ReturnValMarshaled;
}

std::vector<std::vector<int>> Arrays::GetInts2() {
	array<array<::System::Int32>^>^ __ReturnVal = __IL->__Impl->GetInts2();
	std::vector<std::vector<int>> __ReturnValMarshaled = _marshal_as<std::vector<std::vector<int>>>(__ReturnVal);
	return __ReturnValMarshaled;
}

Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* Arrays::GetObject() {
	::MyCorp::TheProduct::SomeModule::Utilities::Stuff^ __ReturnVal = __IL->__Impl->GetObject();
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* __ReturnValMarshaled = _marshal_as<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff*>(__ReturnVal);
	return __ReturnValMarshaled;
}

std::vector<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff*> Arrays::GetObjects() {
	array<::MyCorp::TheProduct::SomeModule::Utilities::Stuff^>^ __ReturnVal = __IL->__Impl->GetObjects();
	std::vector<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff*> __ReturnValMarshaled = _marshal_as<std::vector<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff*>>(__ReturnVal);
	return __ReturnValMarshaled;
}

std::wstring Arrays::GetStrings1() {
	::System::String^ __ReturnVal = __IL->__Impl->GetStrings1();
	std::wstring __ReturnValMarshaled = _marshal_as<std::wstring>(__ReturnVal);
	return __ReturnValMarshaled;
}

std::vector<std::wstring> Arrays::GetStrings2() {
	array<::System::String^>^ __ReturnVal = __IL->__Impl->GetStrings2();
	std::vector<std::wstring> __ReturnValMarshaled = _marshal_as<std::vector<std::wstring>>(__ReturnVal);
	return __ReturnValMarshaled;
}

std::vector<std::vector<std::wstring>> Arrays::GetStrings3() {
	array<::System::String^,2>^ __ReturnVal = __IL->__Impl->GetStrings3();
	std::vector<std::vector<std::wstring>> __ReturnValMarshaled = _marshal_as<std::vector<std::vector<std::wstring>>>(__ReturnVal);
	return __ReturnValMarshaled;
}

std::vector<std::vector<std::wstring>> Arrays::GetStrings4() {
	array<array<::System::String^>^>^ __ReturnVal = __IL->__Impl->GetStrings4();
	std::vector<std::vector<std::wstring>> __ReturnValMarshaled = _marshal_as<std::vector<std::vector<std::wstring>>>(__ReturnVal);
	return __ReturnValMarshaled;
}

std::vector<int> Arrays::listGetInts() {
	::System::Collections::Generic::List<::System::Int32>^ __ReturnVal = ::MyCorp::TheProduct::SomeModule::Utilities::Arrays::listGetInts();
	std::vector<int> __ReturnValMarshaled = _marshal_as<std::vector<int>>(__ReturnVal);
	return __ReturnValMarshaled;
}

std::vector<std::wstring> Arrays::listGetString() {
	::System::Collections::Generic::List<::System::String^>^ __ReturnVal = ::MyCorp::TheProduct::SomeModule::Utilities::Arrays::listGetString();
	std::vector<std::wstring> __ReturnValMarshaled = _marshal_as<std::vector<std::wstring>>(__ReturnVal);
	return __ReturnValMarshaled;
}

void Arrays::listSetInts(const std::vector<int>& list) {
	::System::Collections::Generic::List<::System::Int32>^ __Param_list = _marshal_as<::System::Collections::Generic::List<::System::Int32>^>(list);
	::MyCorp::TheProduct::SomeModule::Utilities::Arrays::listSetInts(__Param_list);
}

void Arrays::listSetStrings(const std::vector<std::wstring>& list) {
	::System::Collections::Generic::List<::System::String^>^ __Param_list = _marshal_as<::System::Collections::Generic::List<::System::String^>^>(list);
	::MyCorp::TheProduct::SomeModule::Utilities::Arrays::listSetStrings(__Param_list);
}

void Arrays::SetInt(int i) {
	::System::Int32 __Param_i = i;
	__IL->__Impl->SetInt(__Param_i);
}

Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* Arrays::SetObject(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* s) {
	::MyCorp::TheProduct::SomeModule::Utilities::Stuff^ __Param_s = _marshal_as<::MyCorp::TheProduct::SomeModule::Utilities::Stuff^>(s);
	::MyCorp::TheProduct::SomeModule::Utilities::Stuff^ __ReturnVal = __IL->__Impl->SetObject(__Param_s);
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* __ReturnValMarshaled = _marshal_as<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff*>(__ReturnVal);
	return __ReturnValMarshaled;
}

Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* Arrays::SetObjects(std::vector<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff*> s) {
	array<::MyCorp::TheProduct::SomeModule::Utilities::Stuff^>^ __Param_s = _marshal_as<array<::MyCorp::TheProduct::SomeModule::Utilities::Stuff^>^>(s);
	::MyCorp::TheProduct::SomeModule::Utilities::Stuff^ __ReturnVal = __IL->__Impl->SetObjects(__Param_s);
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* __ReturnValMarshaled = _marshal_as<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff*>(__ReturnVal);
	return __ReturnValMarshaled;
}

Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* Arrays::structGet() {
	::MyCorp::TheProduct::SomeModule::Utilities::Simple __ReturnVal = __IL->__Impl->structGet();
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* __ReturnValMarshaled = _marshal_as<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple*>(__ReturnVal);
	return __ReturnValMarshaled;
}

int Arrays::structGetPosition() {
	::System::Int32 __ReturnVal = __IL->__Impl->structGetPosition();
	return __ReturnVal;
}

int Arrays::structGetPosition(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* s) {
	::MyCorp::TheProduct::SomeModule::Utilities::Simple __Param_s = _marshal_as<::MyCorp::TheProduct::SomeModule::Utilities::Simple>(s);
	::System::Int32 __ReturnVal = __IL->__Impl->structGetPosition(__Param_s);
	return __ReturnVal;
}

void Arrays::structSet(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* s) {
	::MyCorp::TheProduct::SomeModule::Utilities::Simple __Param_s = _marshal_as<::MyCorp::TheProduct::SomeModule::Utilities::Simple>(s);
	__IL->__Impl->structSet(__Param_s);
}

void Arrays::structSetPosition(int i) {
	::System::Int32 __Param_i = i;
	__IL->__Impl->structSetPosition(__Param_i);
}

void Arrays::structSetPosition(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* s, int i) {
	::MyCorp::TheProduct::SomeModule::Utilities::Simple __Param_s = _marshal_as<::MyCorp::TheProduct::SomeModule::Utilities::Simple>(s);
	::System::Int32 __Param_i = i;
	__IL->__Impl->structSetPosition(__Param_s, __Param_i);
}

void Arrays::test(const std::vector<std::vector<std::wstring>>& strs) {
	array<::System::String^,2>^ __Param_strs = _marshal_as<array<::System::String^,2>^>(strs);
	__IL->__Impl->test(__Param_strs);
}

}
}
}
}
}
