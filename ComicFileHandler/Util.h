#pragma once

#include <Windows.h>
#include <cstdio>

inline bool SucceededAndLogIfFailed(HRESULT hr, const char* file, int line) noexcept
{
	if (hr < 0)
	{
		char data[2048];
		sprintf_s(data, "%x, %s (%d)", hr, file, line);
#pragma warning (suppress: 26485) // do not decay array to pointer
		MessageBoxA(nullptr, data, "FAILED", MB_OK);
		return false;
	}
	return true;
}

#ifdef _DEBUG
#define XSUCCEEDED(hr) (SucceededAndLogIfFailed(hr, __FILE__, __LINE__))
#define XFAILED(hr) (!SucceededAndLogIfFailed(hr, __FILE__, __LINE__))
#else
#define XSUCCEEDED(hr) SUCCEEDED(hr)
#define XFAILED(hr) FAILED(hr)
#endif
#define TEST(x) do { const auto hr = x; if (XFAILED(hr)) return hr; } while (false)
