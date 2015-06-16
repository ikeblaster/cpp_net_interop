#pragma once
#pragma managed

#include "marshaller_ext.h"

#define _LNK __declspec(dllexport)
#using "TestSuite.dll"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Simple_IL.h"

namespace Wrapper {
namespace MyCorp {
namespace TheProduct {
namespace SomeModule {
namespace Utilities {

Simple::Simple() {
	__IL = new Simple_IL;
	__IL->__Impl = gcnew ::MyCorp::TheProduct::SomeModule::Utilities::Simple;
}

Simple::Simple(Simple_IL* IL) {
	__IL = IL;
}

Simple::~Simple() {
	delete __IL;
}

bool Simple::GetExists() {
	::System::Boolean __ReturnVal = __IL->__Impl->Exists;
	return __ReturnVal;
}

void Simple::SetExists(bool value) {
	::System::Boolean __Param_value = value;
	__IL->__Impl->Exists = __Param_value;
}

double Simple::GetLastValue() {
	::System::Double __ReturnVal = __IL->__Impl->LastValue;
	return __ReturnVal;
}

void Simple::SetLastValue(double value) {
	::System::Double __Param_value = value;
	__IL->__Impl->LastValue = __Param_value;
}

int Simple::GetPosition() {
	::System::Int32 __ReturnVal = __IL->__Impl->Position;
	return __ReturnVal;
}

void Simple::SetPosition(int value) {
	::System::Int32 __Param_value = value;
	__IL->__Impl->Position = __Param_value;
}

}
}
}
}
}
