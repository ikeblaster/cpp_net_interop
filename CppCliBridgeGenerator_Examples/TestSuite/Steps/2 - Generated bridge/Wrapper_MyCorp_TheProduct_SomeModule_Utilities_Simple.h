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

		bool GetExists();

		void SetExists(bool value);

		double GetLastValue();

		void SetLastValue(double value);

		int GetPosition();

		void SetPosition(int value);
};

}
}
}
}
}
