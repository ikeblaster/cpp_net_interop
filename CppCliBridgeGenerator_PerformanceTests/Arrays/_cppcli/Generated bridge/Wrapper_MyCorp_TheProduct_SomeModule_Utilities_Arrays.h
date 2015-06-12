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

/// <summary>
/// Popis tridy
/// </summary>
class _LNK Arrays {

	public:
		Arrays_IL* __IL;

		Arrays(std::wstring name);

		Arrays(std::wstring name, std::wstring password);

		Arrays(std::vector<std::wstring> names, int password);

		Arrays(bool cond, std::vector<int> ints);

		Arrays(Arrays_IL* IL);

		~Arrays();

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* get_s();

		void set_s(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* value);

		/// <summary>
		/// Popis property
		/// </summary>
		void set_Name(std::wstring value);

		void callbackInvoke();

		void callbackSet(std::wstring mode, std::wstring (*cb)(std::wstring str, int i));

		static Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Genericclass__System_String* GetGenericWithString();

		int GetInt();

		static std::vector<int> GetInts1();

		std::vector<std::vector<int>> GetInts2();

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* GetObject();

		std::vector<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff*> GetObjects();

		/// <summary>
		/// Popis metody
		/// </summary>
		std::wstring GetStrings1();

		std::vector<std::wstring> GetStrings2();

		std::vector<std::vector<std::wstring>> GetStrings3();

		std::vector<std::vector<std::wstring>> GetStrings4();

		static std::vector<int> listGetInts();

		static std::vector<std::wstring> listGetString();

		static void listSetInts(std::vector<int> list);

		static void listSetStrings(std::vector<std::wstring> list);

		void SetInt(int i);

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* SetObject(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* s);

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* SetObjects(std::vector<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff*> s);

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* structGet();

		int structGetPosition();

		int structGetPosition(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* s);

		void structSet(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* s);

		void structSetPosition(int i);

		void structSetPosition(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* s, int i);

		void test(std::vector<std::vector<std::wstring>> strs);
};

}
}
}
}
}
