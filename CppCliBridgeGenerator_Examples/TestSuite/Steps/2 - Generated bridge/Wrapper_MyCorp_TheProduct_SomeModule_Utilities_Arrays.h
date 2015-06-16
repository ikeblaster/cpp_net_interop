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
/// Class description
/// </summary>
class _LNK Arrays {

	public:
		Arrays_IL* __IL;

		Arrays(const std::wstring& name);

		Arrays(const std::wstring& name, const std::wstring& password);

		Arrays(const std::vector<std::wstring>& names, int password);

		Arrays(bool cond, const std::vector<int>& ints);

		Arrays(Arrays_IL* IL);

		~Arrays();

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* GetS();

		void SetS(Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Simple* value);

		/// <summary>
		/// Property description
		/// </summary>
		std::wstring GetName();

		/// <summary>
		/// Property description
		/// </summary>
		void SetName(const std::wstring& value);

		void callbackInvoke();

		void callbackSet(const std::wstring& mode, std::wstring (*cb)(const std::wstring& str, int i));

		/// <summary>
		/// Public implementation of Dispose pattern callable by consumers.
		/// </summary>
		void Dispose();

		static Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Genericclass__System_String* GetGenericWithString();

		int GetInt();

		static std::vector<int> GetInts1();

		std::vector<std::vector<int>> GetInts2();

		Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff* GetObject();

		std::vector<Wrapper::MyCorp::TheProduct::SomeModule::Utilities::Stuff*> GetObjects();

		/// <summary>
		/// Method description
		/// </summary>
		std::wstring GetStrings1();

		std::vector<std::wstring> GetStrings2();

		std::vector<std::vector<std::wstring>> GetStrings3();

		std::vector<std::vector<std::wstring>> GetStrings4();

		static std::vector<int> listGetInts();

		static std::vector<std::wstring> listGetString();

		static void listSetInts(const std::vector<int>& list);

		static void listSetStrings(const std::vector<std::wstring>& list);

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
