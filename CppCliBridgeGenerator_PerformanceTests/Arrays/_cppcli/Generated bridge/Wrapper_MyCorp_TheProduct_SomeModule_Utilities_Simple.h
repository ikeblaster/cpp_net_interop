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

class Simple_IL;

class _LNK Simple {

	public:
		Simple_IL* __IL;

		Simple();
		Simple(Simple_IL* IL);

		~Simple();

		bool get_Exists();

		void set_Exists(bool value);

		double get_LastValue();

		void set_LastValue(double value);

		int get_Position();

		void set_Position(int value);
};

}
}
}
}
}
