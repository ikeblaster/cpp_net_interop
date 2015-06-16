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

		std::vector<int> GetIntsField();

		void SetIntsField(const std::vector<int>& value);

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::TestEnum::TestEnumType GetLineending();

		void SetLineending(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::TestEnum::TestEnumType value);

		std::wstring GetName();

		void SetName(const std::wstring& value);

		std::vector<int> GetIntsProperty();

		void SetIntsProperty(const std::vector<int>& value);
};

}
}
}
}
}
