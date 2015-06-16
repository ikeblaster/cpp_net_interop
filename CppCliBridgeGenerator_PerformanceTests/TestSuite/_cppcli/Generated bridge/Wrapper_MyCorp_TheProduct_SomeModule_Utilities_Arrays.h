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

class Simple;

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

class Genericclass__System_String;

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

class Stuff;

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

class Arrays_IL;

class _LNK Arrays {

	public:
		Arrays_IL* __IL;

		Arrays(const std::wstring& name);

		Arrays();

		Arrays(const std::wstring& name, const std::wstring& password);

		Arrays(const std::vector<std::wstring>& names, int password);

		Arrays(bool cond, const std::vector<int>& ints);

		Arrays(Arrays_IL* IL);

		~Arrays();

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* GetS();

		void SetS(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* value);

		std::wstring GetName();

		void SetName(const std::wstring& value);

		void callbackInvoke();

		void callbackSet(const std::wstring& mode, std::wstring (*cb)(const std::wstring& str, int i));

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Genericclass__System_String* GetGenericWithString();

		int GetInt();

		std::vector<int> GetInts1();

		std::vector<std::vector<int>> GetInts2();

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* GetObject();

		std::vector<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff*> GetObjects();

		std::wstring GetStrings1();

		std::vector<std::wstring> GetStrings2();

		std::vector<std::vector<std::wstring>> GetStrings3();

		std::vector<std::vector<std::wstring>> GetStrings4();

		std::vector<int> listGetInts();

		std::vector<std::wstring> listGetString();

		void listSetInts(const std::vector<int>& list);

		void listSetStrings(const std::vector<std::wstring>& list);

		void SetInt(int i);

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* SetObject(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* s);

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* SetObjects(std::vector<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff*> s);

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* structGet();

		int structGetPosition();

		int structGetPosition(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* s);

		void structSet(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* s);

		void structSetPosition(int i);

		void structSetPosition(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* s, int i);

		void test(const std::vector<std::vector<std::wstring>>& strs);
};

}
}
}
}
}
