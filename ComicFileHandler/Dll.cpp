#include "CClassFactory.h"
#include "CComicPropertyHandler.h"
#include "CComicThumbnailProvider.h"
#include "CRegisterExtension.h"

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

STDAPI DllGetClassObject(const CLSID& clsid, const IID& riid, void** ppv)
{
	static const CLASS_OBJECT_INIT classObjectInit[] =
	{
		CLSID_CREATOR(CComicThumbnailProvider),
		CLSID_CREATOR(CComicPropertyHandler)
	};
	auto hr = CLASS_E_CLASSNOTAVAILABLE;
	for (size_t i = 0; i < ARRAYSIZE(classObjectInit); i++)
	{
		if (clsid == *classObjectInit[i].pClsid)
		{
			IClassFactory *pClassFactory = new (std::nothrow) CClassFactory(classObjectInit[i].pfnCreate);
			hr = pClassFactory ? S_OK : E_OUTOFMEMORY;
			if (SUCCEEDED(hr))
			{
				hr = pClassFactory->QueryInterface(riid, ppv);
				pClassFactory->Release();
			}
			break;
		}
	}
	return hr;
}

LPCWSTR c_szComicFileExtension = L".cic";
LPCWSTR c_szComicProgID= L"ComicImageCollectionFile";

STDAPI DllRegisterServer()
{
	CRegisterExtension re(__uuidof(CComicPropertyHandler));
	TEST(re.RegisterInProcServer(L"ComicPropertyHandler", L"Both"));
	TEST(re.RegisterInProcServerAttribute(L"ManualSafeSave", true));
	TEST(re.RegisterPropertyHandler(c_szComicFileExtension));
	TEST(re.RegisterProgIDValue(c_szComicProgID, L"InfoTip", L"prop:System.ItemType;System.Author;System.Size;System.DateModified"));
	TEST(re.RegisterProgIDValue(c_szComicProgID, L"FullDetails", L"prop:System.PropGroup.Description;System.Title;System.Author;System.Document.DateCreated;System.Keywords;System.FileVersion;System.PropGroup.FileSystem;System.ItemNameDisplay;System.ItemType;System.ItemFolderPathDisplay;System.Size;System.DateCreated;System.DateModified;System.DateAccessed;System.FileAttributes;System.OfflineAvailability;System.OfflineStatus;System.SharedWith;System.FileOwner;System.ComputerName"));
	TEST(re.RegisterProgIDValue(c_szComicProgID, L"PreviewDetails", L"prop:System.Title;System.Author;System.Document.DateCreated;System.Keywords;System.DateModified;System.Size;System.FileVersion;System.DateCreated;System.SharedWith"));
	TEST(re.RegisterExtensionWithProgID(c_szComicFileExtension, c_szComicProgID));
	CRegisterExtension re2(__uuidof(CComicThumbnailProvider));
	TEST(re2.RegisterInProcServer(L"ComicThumbnailHandler", L"Apartment"));
	return re2.RegisterThumbnailHandler(c_szComicFileExtension);
}

STDAPI DllUnregisterServer()
{
	CRegisterExtension re(__uuidof(CComicPropertyHandler));
	TEST(re.UnRegisterObject());
	TEST(re.UnRegisterPropertyHandler(c_szComicFileExtension));
	TEST(re.UnRegisterProgID(c_szComicProgID, c_szComicFileExtension));
	CRegisterExtension re2(__uuidof(CComicThumbnailProvider));
	TEST(re2.UnRegisterObject());
	return re2.UnRegisterThumbnailHandler(c_szComicFileExtension);
}