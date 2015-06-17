#pragma once
#pragma managed

#include "marshaller_ext.h"

#define _LNK __declspec(dllexport)
#using "Ionic.Zip.dll"
#include "Wrapper_Ionic_Zip_ZipEntry_IL.h"

namespace Wrapper {
namespace Ionic {
namespace Zip {

ZipEntry::ZipEntry(ZipEntry_IL* IL) {
	__IL = IL;
}

ZipEntry::~ZipEntry() {
	delete __IL;
}

}
}
}
