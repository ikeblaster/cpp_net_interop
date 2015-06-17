#pragma once
#pragma managed

#include "marshaller_ext.h"

#define _LNK __declspec(dllexport)
#using "Ionic.Zip.dll"
#include "Wrapper_Ionic_Zip_ZipFile_IL.h"
#include "Wrapper_Ionic_Zip_ZipEntry_IL.h"

namespace Wrapper {
namespace Ionic {
namespace Zip {

ZipFile::ZipFile() {
	__IL = new ZipFile_IL;
	__IL->__Impl = gcnew ::Ionic::Zip::ZipFile();
}

ZipFile::ZipFile(ZipFile_IL* IL) {
	__IL = IL;
}

ZipFile::~ZipFile() {
	delete __IL;
}

Wrapper::Ionic::Zip::ZipEntry* ZipFile::AddFile(const std::wstring& fileName) {
	::System::String^ __Param_fileName = _marshal_as<::System::String^>(fileName);
	::Ionic::Zip::ZipEntry^ __ReturnVal = __IL->__Impl->AddFile(__Param_fileName);
	Wrapper::Ionic::Zip::ZipEntry* __ReturnValMarshaled = _marshal_as<Wrapper::Ionic::Zip::ZipEntry*>(__ReturnVal);
	return __ReturnValMarshaled;
}

void ZipFile::Save(const std::wstring& fileName) {
	::System::String^ __Param_fileName = _marshal_as<::System::String^>(fileName);
	__IL->__Impl->Save(__Param_fileName);
}

}
}
}
