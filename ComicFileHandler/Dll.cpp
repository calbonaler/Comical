#include "CClassFactory.h"
#include "CComicPropertyHandler.h"
#include "CComicThumbnailProvider.h"
#include <combaseapi.h>

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

_Check_return_ STDAPI DllGetClassObject(_In_ REFCLSID clsid, _In_ REFIID riid, _Outptr_ LPVOID FAR* ppv)
{
	static const CLASS_OBJECT_INIT classObjectInit[] =
	{
		CLSID_CREATOR(CComicThumbnailProvider),
		CLSID_CREATOR(CComicPropertyHandler)
	};
	for (size_t i = 0; i < ARRAYSIZE(classObjectInit); i++)
	{
		if (clsid != *classObjectInit[i].pClsid)
			continue;
		CComPtr<IClassFactory> pClassFactory;
		pClassFactory.Attach(new (std::nothrow) CClassFactory(classObjectInit[i].pfnCreate));
		return pClassFactory ? pClassFactory->QueryInterface(riid, ppv) : E_OUTOFMEMORY;
	}
	return CLASS_E_CLASSNOTAVAILABLE;
}