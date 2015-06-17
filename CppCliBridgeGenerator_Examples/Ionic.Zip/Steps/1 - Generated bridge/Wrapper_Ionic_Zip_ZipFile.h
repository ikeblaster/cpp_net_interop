#pragma once
#include <string>
#include <vector>

#ifndef _LNK
#define _LNK __declspec(dllimport)
#endif

namespace Wrapper {
namespace Ionic {
namespace Zip {

class ZipEntry;

}
}
}

namespace Wrapper {
namespace Ionic {
namespace Zip {

class ZipFile_IL;

class _LNK ZipFile {

	public:
		ZipFile_IL* __IL;

		ZipFile();

		ZipFile(ZipFile_IL* IL);

		~ZipFile();

		Wrapper::Ionic::Zip::ZipEntry* AddFile(const std::wstring& fileName);

		void Save(const std::wstring& fileName);
};

}
}
}
