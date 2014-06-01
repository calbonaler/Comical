#pragma once

#include <Shlwapi.h>
#include <new>

template <typename T> inline T pointer_cast(void* pv) { return static_cast<T>(pv); }

template <typename T> inline void SafeRelease(T*& rpT)
{
	if (rpT)
	{
		rpT->Release();
		rpT = nullptr;
	}
}

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

#define TEST(x) do { auto hr = x; if (FAILED(hr)) return hr; } while (false)
#pragma warning (disable : 4127)

typedef HRESULT (*PFNCREATEINSTANCE)(IUnknown* punkOuter, REFIID riid, void** ppv);

struct CLASS_OBJECT_INIT
{
	const CLSID* pClsid;
	PFNCREATEINSTANCE pfnCreate;
};

void DllAddRef();
void DllRelease();