#pragma once
#pragma managed

#include "marshaller_ext.h"

#define _LNK __declspec(dllexport)
#using "Arrays.dll"
#include "Wrapper_MyCorp_TheProduct_SomeModule_Utilities_Genericclass__System_String_IL.h"

namespace Wrapper {
namespace MyCorp {
namespace TheProduct {
namespace SomeModule {
namespace Utilities {

Genericclass__System_String::Genericclass__System_String(Genericclass__System_String_IL* IL) {
	__IL = IL;
}

Genericclass__System_String::~Genericclass__System_String() {
	delete __IL;
}

void Genericclass__System_String::print() {
	__IL->__Impl->print();
}

void Genericclass__System_String::print2() {
	__IL->__Impl->print2();
}

}
}
}
}
}
