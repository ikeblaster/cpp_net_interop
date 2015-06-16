#pragma once
#pragma managed

#include "marshaller_ext.h"

#define _LNK __declspec(dllexport)
#using "TestSuite.dll"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Stuff_IL.h"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_TestEnum_IL.h"

namespace Wrapper {
namespace MyCorp {
namespace TheProduct {
namespace SomeModule {
namespace Utilities {

Stuff::Stuff() {
	__IL = new Stuff_IL;
	__IL->__Impl = gcnew ::MyCorp::TheProduct::SomeModule::Utilities::Stuff();
}

Stuff::Stuff(Stuff_IL* IL) {
	__IL = IL;
}

Stuff::~Stuff() {
	delete __IL;
}

std::vector<int> Stuff::GetIntsField() {
	array<::System::Int32>^ __ReturnVal = __IL->__Impl->IntsField;
	std::vector<int> __ReturnValMarshaled = _marshal_as<std::vector<int>>(__ReturnVal);
	return __ReturnValMarshaled;
}

void Stuff::SetIntsField(const std::vector<int>& value) {
	array<::System::Int32>^ __Param_value = _marshal_as<array<::System::Int32>^>(value);
	__IL->__Impl->IntsField = __Param_value;
}

Wrapper::MyCorp::TheProduct::SomeModule::Utilities::TestEnum::TestEnumType Stuff::GetLineending() {
	::MyCorp::TheProduct::SomeModule::Utilities::TestEnum __ReturnVal = __IL->__Impl->lineending;
	Wrapper::MyCorp::TheProduct::SomeModule::Utilities::TestEnum::TestEnumType __ReturnValCast = static_cast<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::TestEnum::TestEnumType>(__ReturnVal);
	return __ReturnValCast;
}

void Stuff::SetLineending(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::TestEnum::TestEnumType value) {
	::MyCorp::TheProduct::SomeModule::Utilities::TestEnum __Param_value = static_cast<::MyCorp::TheProduct::SomeModule::Utilities::TestEnum>(value);
	__IL->__Impl->lineending = __Param_value;
}

std::wstring Stuff::GetName() {
	::System::String^ __ReturnVal = __IL->__Impl->Name;
	std::wstring __ReturnValMarshaled = _marshal_as<std::wstring>(__ReturnVal);
	return __ReturnValMarshaled;
}

void Stuff::SetName(const std::wstring& value) {
	::System::String^ __Param_value = _marshal_as<::System::String^>(value);
	__IL->__Impl->Name = __Param_value;
}

std::vector<int> Stuff::GetIntsProperty() {
	array<::System::Int32>^ __ReturnVal = __IL->__Impl->IntsProperty;
	std::vector<int> __ReturnValMarshaled = _marshal_as<std::vector<int>>(__ReturnVal);
	return __ReturnValMarshaled;
}

void Stuff::SetIntsProperty(const std::vector<int>& value) {
	array<::System::Int32>^ __Param_value = _marshal_as<array<::System::Int32>^>(value);
	__IL->__Impl->IntsProperty = __Param_value;
}

}
}
}
}
}
