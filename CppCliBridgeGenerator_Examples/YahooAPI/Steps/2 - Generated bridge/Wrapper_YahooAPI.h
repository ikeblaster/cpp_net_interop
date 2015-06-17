#pragma once
#include <string>
#include <vector>

#ifndef _LNK
#define _LNK __declspec(dllimport)
#endif

namespace Wrapper {

class YahooAPI_IL;

class _LNK YahooAPI {

	public:
		YahooAPI_IL* __IL;

		YahooAPI();

		YahooAPI(YahooAPI_IL* IL);

		~YahooAPI();

		double GetAsk(const std::wstring& symbol);

		double GetBid(const std::wstring& symbol);

		std::wstring GetCapitalization(const std::wstring& symbol);

		static int GetStringLength(const std::wstring& str);

		std::vector<std::wstring> GetValues(const std::wstring& symbol, const std::wstring& fields);

		static std::wstring test();

		static void test(const std::wstring& mode, void (*cb)(const std::wstring& str));
};

}
