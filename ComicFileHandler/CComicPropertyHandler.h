#pragma once

#include <ShObjIdl.h>

class _declspec(uuid("001823E8-247E-4685-BD84-350347B0460C")) CComicPropertyHandler final : public IInitializeWithStream, public IPropertyStore, public IPropertyStoreCapabilities
{
public:
	CComicPropertyHandler() { DllAddRef(); }

	BEGIN_COM_INTERFACE_MAPPPING
		QITABENT(CComicPropertyHandler, IPropertyStore),
		QITABENT(CComicPropertyHandler, IPropertyStoreCapabilities),
		QITABENT(CComicPropertyHandler, IInitializeWithStream)
	END_COM_INTER_FACE_MAPPING

	IFACEMETHODIMP Initialize(_In_ IStream*, _In_ DWORD);

	IFACEMETHODIMP GetCount(__RPC__out DWORD* pcProps) { return m_pCache.p ? m_pCache->GetCount(pcProps) : E_UNEXPECTED; }
	IFACEMETHODIMP GetAt(DWORD iProp, __RPC__out PROPERTYKEY* pKey) { return m_pCache.p ? m_pCache->GetAt(iProp, pKey) : E_UNEXPECTED; }
	IFACEMETHODIMP GetValue(__RPC__in REFPROPERTYKEY key, __RPC__out PROPVARIANT* pPropVar) { return m_pCache.p ? m_pCache->GetValue(key, pPropVar) : E_UNEXPECTED; }
	IFACEMETHODIMP SetValue(__RPC__in REFPROPERTYKEY, __RPC__in REFPROPVARIANT) { return E_NOTIMPL; }
	IFACEMETHODIMP Commit() { return E_NOTIMPL; }
	IFACEMETHODIMP IsPropertyWritable(__RPC__in REFPROPERTYKEY) { return S_FALSE; }

private:
	CComPtr<IPropertyStoreCache> m_pCache;

	virtual ~CComicPropertyHandler() { DllRelease(); }
};