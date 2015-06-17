#pragma once
#pragma managed
#include <msclr\auto_gcroot.h>

#using "Ionic.Zip.dll"
#include "Wrapper_Ionic_Zip_ZipEntry.h"

namespace Wrapper {
namespace Ionic {
namespace Zip {

class ZipEntry_IL {
	public:
		msclr::auto_gcroot<::Ionic::Zip::ZipEntry^> __Impl;
};

}
}
}

template <typename TTo> 
inline Wrapper::Ionic::Zip::ZipEntry* marshal_as(::Ionic::Zip::ZipEntry^ const from)
{
	Wrapper::Ionic::Zip::ZipEntry_IL* bridge = new Wrapper::Ionic::Zip::ZipEntry_IL;
	Wrapper::Ionic::Zip::ZipEntry* wrapper = new Wrapper::Ionic::Zip::ZipEntry(bridge);
	bridge->__Impl = from;
	return wrapper;
}

template <typename TTo> 
inline ::Ionic::Zip::ZipEntry^ marshal_as(Wrapper::Ionic::Zip::ZipEntry* const from)
{
	return (::Ionic::Zip::ZipEntry^) from->__IL->__Impl.get();
}
