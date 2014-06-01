#pragma once

#include <ShObjIdl.h>
#include <string>

class CRegisterExtension
{
public:
    CRegisterExtension(REFCLSID clsid = CLSID_NULL);
    ~CRegisterExtension();

    HRESULT RegisterInProcServer(const std::wstring& wstrFriendlyName, const std::wstring& wstrThreadingModel);
    HRESULT RegisterInProcServerAttribute(const std::wstring& wstrAttribute, DWORD dwValue);

    // remove a COM object registration
    HRESULT UnRegisterObject();

    HRESULT RegisterPropertyHandler(const std::wstring& wstrExtension);
    HRESULT UnRegisterPropertyHandler(const std::wstring& wstrExtension);

	HRESULT RegisterThumbnailHandler(const std::wstring& wstrExtension);
	HRESULT UnRegisterThumbnailHandler(const std::wstring& wstrExtension);

    HRESULT UnRegisterProgID(const std::wstring& wstrProgID, const std::wstring& wstrFileExtension);
    HRESULT RegisterExtensionWithProgID(const std::wstring& wstrFileExtension, const std::wstring& wstrProgID);

    HRESULT RegisterProgIDValue(const std::wstring& wstrProgID, const std::wstring& wstrValueName, const std::wstring& wstrValue);

private:
    HRESULT SetRegValue(HKEY hkey, const std::wstring& wstrKeyName, const std::wstring& wstrValueName, const std::wstring& wstrValue);
    HRESULT SetRegValue(HKEY hkey, const std::wstring& wstrKeyName, const std::wstring& wstrValueName, DWORD dwValue);

    HRESULT DeleteRegKey(HKEY hkey, const std::wstring& wstrKeyName);

    void UpdateAssocChanged(HKEY hkey, const std::wstring& wstrKeyName);

    std::wstring m_wstrCLSID;
    WCHAR m_szModule[MAX_PATH];
    bool m_fAssocChanged;
};
