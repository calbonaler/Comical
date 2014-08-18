#include "CRegisterExtension.h"
#include <shlobj.h>
#include <algorithm>
#include "Dll.h"

#pragma comment(lib, "shlwapi.lib")     // link to this

using std::wstring;

extern "C" IMAGE_DOS_HEADER __ImageBase;

CRegisterExtension::CRegisterExtension(REFCLSID clsid) : m_fAssocChanged(false)
{
	WCHAR szClsid[39];
    StringFromGUID2(clsid, szClsid, ARRAYSIZE(szClsid));
	m_wstrCLSID = szClsid;
	GetModuleFileNameW(pointer_cast<HMODULE>(&__ImageBase), m_szModule, ARRAYSIZE(m_szModule));
}

CRegisterExtension::~CRegisterExtension() { if (m_fAssocChanged) SHChangeNotify(SHCNE_ASSOCCHANGED, 0, 0, 0); }

HRESULT CRegisterExtension::RegisterInProcServer(const wstring& wstrFriendlyName, const wstring& wstrThreadingModel)
{
	if (!m_szModule[0])
		return E_FAIL;
	TEST(SetRegValue(HKEY_CLASSES_ROOT, L"CLSID\\" + m_wstrCLSID, L"", wstrFriendlyName));
	TEST(SetRegValue(HKEY_CLASSES_ROOT, L"CLSID\\"+ m_wstrCLSID + L"\\InProcServer32", L"", m_szModule));
	return SetRegValue(HKEY_CLASSES_ROOT, L"CLSID\\" + m_wstrCLSID + L"\\InProcServer32", L"ThreadingModel", wstrThreadingModel);
}

HRESULT CRegisterExtension::RegisterInProcServerAttribute(const wstring& wstrAttribute, DWORD dwValue) { return SetRegValue(HKEY_CLASSES_ROOT, L"CLSID\\" + m_wstrCLSID, wstrAttribute, dwValue); }

HRESULT CRegisterExtension::UnRegisterObject()
{
    // might have an AppID value, try that
	TEST(DeleteRegKey(HKEY_CLASSES_ROOT, L"AppID\\" + m_wstrCLSID));
	return DeleteRegKey(HKEY_CLASSES_ROOT, L"CLSID\\" + m_wstrCLSID);
}

HRESULT CRegisterExtension::RegisterPropertyHandler(const wstring& wstrExtension) { return SetRegValue(HKEY_LOCAL_MACHINE, L"Software\\Microsoft\\Windows\\CurrentVersion\\PropertySystem\\PropertyHandlers\\" + wstrExtension, L"", m_wstrCLSID); }

HRESULT CRegisterExtension::UnRegisterPropertyHandler(const wstring& wstrExtension) { return DeleteRegKey(HKEY_LOCAL_MACHINE, L"Software\\Microsoft\\Windows\\CurrentVersion\\PropertySystem\\PropertyHandlers\\" + wstrExtension); }

HRESULT CRegisterExtension::RegisterThumbnailHandler(const wstring& wstrExtension) { return SetRegValue(HKEY_CLASSES_ROOT, wstrExtension + L"\\ShellEx\\{e357fccd-a995-4576-b01f-234630154e96}", L"", m_wstrCLSID); }

HRESULT CRegisterExtension::UnRegisterThumbnailHandler(const wstring& wstrExtension) { return DeleteRegKey(HKEY_CLASSES_ROOT, wstrExtension + L"\\ShellEx\\{e357fccd-a995-4576-b01f-234630154e96}"); }

HRESULT CRegisterExtension::UnRegisterProgID(const wstring& wstrProgID, const wstring& wstrFileExtension)
{
	TEST(DeleteRegKey(HKEY_CLASSES_ROOT, wstrProgID));
	if (!wstrFileExtension.empty())
		return SetRegValue(HKEY_CLASSES_ROOT, wstrFileExtension, L"", L"");
    return S_OK;
}

HRESULT CRegisterExtension::RegisterProgIDValue(const wstring& wstrProgID, const wstring& wstrValueName, const wstring& wstrValue) { return SetRegValue(HKEY_CLASSES_ROOT, wstrProgID, wstrValueName, wstrValue); }

HRESULT CRegisterExtension::RegisterExtensionWithProgID(const wstring& wstrFileExtension, const wstring& wstrProgID) { return SetRegValue(HKEY_CLASSES_ROOT, wstrFileExtension, L"", wstrProgID); }

HRESULT CRegisterExtension::SetRegValue(HKEY hkey, const wstring& wstrKeyName, const wstring& wstrValueName, const wstring& wstrValue)
{
	TEST(HRESULT_FROM_WIN32(static_cast<unsigned long>(RegSetKeyValueW(hkey, wstrKeyName.c_str(), wstrValueName.c_str(), REG_SZ, wstrValue.c_str(), (static_cast<DWORD>((wstrValue.length() + 1) * sizeof(WCHAR)))))));
	UpdateAssocChanged(hkey, wstrKeyName);
	return S_OK;
}

HRESULT CRegisterExtension::SetRegValue(HKEY hkey, const wstring& wstrKeyName, const wstring& wstrValueName, DWORD dwValue)
{
	TEST(HRESULT_FROM_WIN32(static_cast<unsigned long>(RegSetKeyValueW(hkey, wstrKeyName.c_str(), wstrValueName.c_str(), REG_DWORD, &dwValue, sizeof(dwValue)))));
	UpdateAssocChanged(hkey, wstrKeyName);
	return S_OK;
}

HRESULT CRegisterExtension::DeleteRegKey(HKEY hkey, const wstring& wstrKeyName)
{
	auto ls = RegDeleteTreeW(hkey, wstrKeyName.c_str());
	auto hr = HRESULT_FROM_WIN32(static_cast<unsigned long>(ls));
	if (SUCCEEDED(hr))
		UpdateAssocChanged(hkey, wstrKeyName);
	return ls == ERROR_FILE_NOT_FOUND ? S_OK : hr;
}

void CRegisterExtension::UpdateAssocChanged(HKEY hkey, const wstring& wstrKeyName)
{
	if (!m_fAssocChanged)
	{
		if (hkey == HKEY_CLASSES_ROOT)
			m_fAssocChanged = true;
		else
		{
			wstring wstrKeyNameCopy(wstrKeyName);
			std::transform(wstrKeyNameCopy.begin(), wstrKeyNameCopy.end(), wstrKeyNameCopy.begin(), towlower);
			if (wstrKeyNameCopy.find(L"propertyhandlers") != wstring::npos || wstrKeyNameCopy.find(L"kindmap") != wstring::npos)
				m_fAssocChanged = true;
		}
	}
}
