#pragma once
#include <string>
#include <vector>

#ifndef _LNK
#define _LNK __declspec(dllimport)
#endif

namespace Wrapper {
namespace Ionic {
namespace Zip {

class ZipEntry_IL;

class _LNK ZipEntry {

	public:
		ZipEntry_IL* __IL;

		ZipEntry(ZipEntry_IL* IL);

		~ZipEntry();
};

}
}
}
