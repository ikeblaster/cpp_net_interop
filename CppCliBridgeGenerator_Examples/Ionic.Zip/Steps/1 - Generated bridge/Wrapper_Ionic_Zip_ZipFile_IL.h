#pragma once
#pragma managed
#include <msclr\auto_gcroot.h>

#using "Ionic.Zip.dll"
#include "Wrapper_Ionic_Zip_ZipFile.h"

namespace Wrapper {
namespace Ionic {
namespace Zip {

class ZipFile_IL {
	public:
		msclr::auto_gcroot<::Ionic::Zip::ZipFile^> __Impl;
};

}
}
}

template <typename TTo> 
inline Wrapper::Ionic::Zip::ZipFile* marshal_as(::Ionic::Zip::ZipFile^ const from)
{
	Wrapper::Ionic::Zip::ZipFile_IL* bridge = new Wrapper::Ionic::Zip::ZipFile_IL;
	Wrapper::Ionic::Zip::ZipFile* wrapper = new Wrapper::Ionic::Zip::ZipFile(bridge);
	bridge->__Impl = from;
	return wrapper;
}

template <typename TTo> 
inline ::Ionic::Zip::ZipFile^ marshal_as(Wrapper::Ionic::Zip::ZipFile* const from)
{
	return (::Ionic::Zip::ZipFile^) from->__IL->__Impl.get();
}
