#pragma once

#include <Shlwapi.h>
#include <new>
#include <atlbase.h>

template <typename T> inline T pointer_cast(void* pv) { return static_cast<T>(pv); }

#ifdef _DEBUG
inline bool SUCCEEDED_DEBUG(HRESULT hr, LPSTR file, int line)
{
	if (FAILED(hr))
	{
		CHAR data[2048];
		wsprintfA(data, "%x, %s (%d)", hr, file, line);
		MessageBoxA(nullptr, data, "FAILED", MB_OK);
		return false;
	}
	return true;
}
#undef SUCCEEDED
#define SUCCEEDED(hr) SUCCEEDED_DEBUG(hr, __FILE__, __LINE__)
#define TEST(x) do { auto hr = x; if (!SUCCEEDED_DEBUG(hr, __FILE__, __LINE__)) return hr; } while (false)
#else
#define TEST(x) do { auto hr = x; if (FAILED(hr)) return hr; } while (false)
#endif

void DllAddRef();
void DllRelease();

#define BEGIN_COM_INTERFACE_MAPPPING \
IFACEMETHODIMP QueryInterface(REFIID riid, void** ppv) \
{ \
	static const QITAB qit[] = \
	{ \

#define END_COM_INTER_FACE_MAPPING \
		,{ nullptr } \
	}; \
	return QISearch(this, qit, riid, ppv); \
} \
IFACEMETHODIMP_(ULONG) AddRef() { return InterlockedIncrement(&m_cRef); } \
IFACEMETHODIMP_(ULONG) Release() \
{ \
	auto cRef = InterlockedDecrement(&m_cRef); \
	if (cRef == 0) \
		delete this; \
	return cRef; \
} \
private: \
	ULONG m_cRef = 1; \
public: