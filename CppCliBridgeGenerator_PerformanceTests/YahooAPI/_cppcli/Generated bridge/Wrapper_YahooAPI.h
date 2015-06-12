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

		double GetAsk(std::wstring symbol);

		double GetBid(std::wstring symbol);

		std::wstring GetCapitalization(std::wstring symbol);

		int GetStringLength(std::wstring str);

		std::vector<std::wstring> GetValues(std::wstring symbol, std::wstring fields);

		std::wstring test();

		void test(std::wstring mode, void (*cb)(std::wstring str));
};

}
