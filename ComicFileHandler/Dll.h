#pragma once

#include <Shlwapi.h>
#include <new>
#include <atlbase.h>

template <typename T> inline T pointer_cast(void* pv) { return static_cast<T>(pv); }

#define CLSID_CREATOR(type) { &__uuidof(type), CoInstanceCreator<type> }

template <typename T> HRESULT CoInstanceCreator(IUnknown* punkOuter, REFIID riid, void** ppv)
{
	if (punkOuter)
		return CLASS_E_NOAGGREGATION;
	T* pNew = new (std::nothrow) T();
	if (!pNew)
		return E_OUTOFMEMORY;
	HRESULT hr = pNew->QueryInterface(riid, ppv);
	pNew->Release();
	return hr;
}

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

#pragma warning (disable : 4127)

typedef HRESULT (*PFNCREATEINSTANCE)(IUnknown* punkOuter, REFIID riid, void** ppv);

struct CLASS_OBJECT_INIT
{
	const CLSID* pClsid;
	PFNCREATEINSTANCE pfnCreate;
};

void DllAddRef();
void DllRelease();