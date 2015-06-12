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

namespace TestEnum { enum TestEnumType; }

}
}
}
}
}

namespace Wrapper {
namespace MyCorp {
namespace TheProduct {
namespace SomeModule {
namespace Utilities {

class Stuff_IL;

class _LNK Stuff {

	public:
		Stuff_IL* __IL;

		Stuff();

		Stuff(Stuff_IL* IL);

		~Stuff();

		std::vector<int> get_IntsField();

		void set_IntsField(std::vector<int> value);

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::TestEnum::TestEnumType get_lineending();

		void set_lineending(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::TestEnum::TestEnumType value);

		std::wstring get_Name();

		void set_Name(std::wstring value);

		void set_IntsProperty(std::vector<int> value);
};

}
}
}
}
}
