#include "CComicPropertyHandler.h"
#include "CComicThumbnailProvider.h"
#include <combaseapi.h>
#include <ShObjIdl.h>

ULONG g_cRefModule = 0;

BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, void*)
{
	if (dwReason == DLL_PROCESS_ATTACH)
		DisableThreadLibraryCalls(hInstance);
	return true;
}

STDAPI DllCanUnloadNow() { return !g_cRefModule ? S_OK : S_FALSE; }

void DllAddRef() { InterlockedIncrement(&g_cRefModule); }

void DllRelease() { InterlockedDecrement(&g_cRefModule); }

typedef HRESULT(*PFNCREATEINSTANCE)(_In_ REFIID riid, _COM_Outptr_ void** ppv);

template <typename T> HRESULT CreateInstance(_In_ REFIID riid, _COM_Outptr_ void** ppv)
{
	T* pNew = new (std::nothrow) T();
	if (!pNew)
		return E_OUTOFMEMORY;
	auto hr = pNew->QueryInterface(riid, ppv);
	pNew->Release();
	return hr;
}

_Check_return_ STDAPI DllGetClassObject(_In_ REFCLSID clsid, _In_ REFIID riid, _Outptr_ LPVOID FAR* ppv)
{
	class CClassFactory final : public IClassFactory
	{
	public:
		CClassFactory(PFNCREATEINSTANCE pfnCreate) : m_pfnCreate(pfnCreate) { DllAddRef(); }

#pragma warning (push)
#pragma warning (disable: 4838)
		BEGIN_COM_INTERFACE_MAPPPING
			QITABENT(CClassFactory, IClassFactory)
		END_COM_INTER_FACE_MAPPING
#pragma warning (pop)

		IFACEMETHODIMP CreateInstance(_In_opt_ IUnknown* punkOuter, _In_ REFIID riid, _COM_Outptr_ void** ppv)
		{
			*ppv = nullptr;
			if (punkOuter)
				return CLASS_E_NOAGGREGATION;
			return m_pfnCreate(riid, ppv);
		}
		IFACEMETHODIMP LockServer(BOOL fLock)
		{
			if (fLock)
				DllAddRef();
			else
				DllRelease();
			return S_OK;
		}

	private:
		PFNCREATEINSTANCE m_pfnCreate;

		~CClassFactory() { DllRelease(); }
	};

	static const struct
	{
		const CLSID& pClsid;
		PFNCREATEINSTANCE pfnCreate;
	}
	classObjectInit[] =
	{
		{ __uuidof(CComicThumbnailProvider), CreateInstance<CComicThumbnailProvider> },
		{ __uuidof(CComicPropertyHandler), CreateInstance<CComicPropertyHandler> },
	};
	for (size_t i = 0; i < ARRAYSIZE(classObjectInit); i++)
	{
		if (clsid != classObjectInit[i].pClsid)
			continue;
		CComPtr<IClassFactory> pClassFactory;
		pClassFactory.Attach(new (std::nothrow) CClassFactory(classObjectInit[i].pfnCreate));
		return pClassFactory ? pClassFactory->QueryInterface(riid, ppv) : E_OUTOFMEMORY;
	}
	return CLASS_E_CLASSNOTAVAILABLE;
}