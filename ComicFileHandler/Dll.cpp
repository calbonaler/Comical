#include "CComicPropertyHandler.h"
#include "CComicThumbnailProvider.h"

ULONG g_cRefModule = 0;

BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, void*) noexcept
{
	if (dwReason == DLL_PROCESS_ATTACH)
		DisableThreadLibraryCalls(hInstance);
	return true;
}

__control_entrypoint(DllExport) STDAPI DllCanUnloadNow() { return !g_cRefModule ? S_OK : S_FALSE; }

void DllAddRef() noexcept
{
#pragma warning (push)
#pragma warning (disable: 26447) // declared noexcept but maybe throw (InterlockedIncrement)
	InterlockedIncrement(&g_cRefModule);
#pragma warning (pop)
}

void DllRelease() noexcept
{
#pragma warning (push)
#pragma warning (disable: 26447) // declared noexcept but maybe throw (InterlockedIncrement)
	InterlockedDecrement(&g_cRefModule);
#pragma warning (pop)
}

class CClassFactoryBase : public CCoclassBase<IClassFactory>
{
public:
#pragma warning (push)
#pragma warning (disable: 26429) // nullability is not tested (ppv)
	IFACEMETHODIMP CreateInstance(_In_opt_ IUnknown* punkOuter, _In_ const IID& riid, _COM_Outptr_ void** ppv) noexcept override
#pragma warning (pop)
	{
		*ppv = nullptr;
		if (punkOuter)
			return CLASS_E_NOAGGREGATION;
		return CreateInstanceCore(riid, ppv);
	}
	IFACEMETHODIMP LockServer(BOOL fLock) noexcept override
	{
		if (fLock)
			DllAddRef();
		else
			DllRelease();
		return S_OK;
	}

protected:
	IFACEMETHOD(CreateInstanceCore)(_In_ const IID& riid, _COM_Outptr_ void** ppv) noexcept = 0;
};

template <typename T> class CClassFactory : public CClassFactoryBase
{
protected:
#pragma warning (push)
#pragma warning (disable: 26429) // nullability is not tested (ppv)
	IFACEMETHODIMP CreateInstanceCore(_In_ const IID& riid, _COM_Outptr_ void** ppv) noexcept override
#pragma warning (pop)
	{
		*ppv = nullptr;
		CComPtr<T> pNew;
		pNew.Attach(new (std::nothrow) T());
		if (!pNew)
			return E_OUTOFMEMORY;
		return pNew->QueryInterface(riid, ppv);
	}
};

#define ASSIGN_CLASS(cls, clsid, riid, ppv) \
do { \
	if (clsid == __uuidof(cls)) \
	{ \
		CComPtr<IClassFactory> pClassFactory; \
		pClassFactory.Attach(new (std::nothrow) CClassFactory<cls>()); \
		return pClassFactory ? pClassFactory->QueryInterface(riid, ppv) : E_OUTOFMEMORY; \
	} \
} while (false)

_Check_return_ STDAPI DllGetClassObject(_In_ const CLSID& clsid, _In_ const IID& riid, _Outptr_ void** ppv)
{
	ASSIGN_CLASS(CComicThumbnailProvider, clsid, riid, ppv);
	ASSIGN_CLASS(CComicPropertyHandler, clsid, riid, ppv);
	return CLASS_E_CLASSNOTAVAILABLE;
}