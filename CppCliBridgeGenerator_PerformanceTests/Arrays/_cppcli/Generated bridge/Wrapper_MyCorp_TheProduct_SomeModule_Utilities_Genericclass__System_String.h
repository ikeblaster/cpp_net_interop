#pragma once
#include <string>
#include <vector>

#ifndef _LNK
#define _LNK __declspec(dllimport)
#endif

namespace Wrapper {
namespace MyCorp {
namespace TheProduct {
namespace SomeModule {
namespace Utilities {

class Genericclass__System_String_IL;

class _LNK Genericclass__System_String {

	public:
		Genericclass__System_String_IL* __IL;

		Genericclass__System_String(Genericclass__System_String_IL* IL);

		~Genericclass__System_String();

		void print();

		void print2();
};

}
}
}
}
}
