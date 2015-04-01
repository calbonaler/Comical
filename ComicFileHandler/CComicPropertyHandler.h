#pragma once

#include <ShObjIdl.h>

class _declspec(uuid("001823E8-247E-4685-BD84-350347B0460C")) CComicPropertyHandler : public IInitializeWithStream, public IPropertyStore, public IPropertyStoreCapabilities
{
public:
	CComicPropertyHandler();

	IFACEMETHODIMP QueryInterface(REFIID, void**);
	IFACEMETHODIMP_(ULONG) AddRef();
	IFACEMETHODIMP_(ULONG) Release();
	
	IFACEMETHODIMP Initialize(_In_ IStream*, _In_ DWORD);

	IFACEMETHODIMP GetCount(__RPC__out DWORD*);
	IFACEMETHODIMP GetAt(DWORD, __RPC__out PROPERTYKEY*);
	IFACEMETHODIMP GetValue(__RPC__in REFPROPERTYKEY, __RPC__out PROPVARIANT*);
	IFACEMETHODIMP SetValue(__RPC__in REFPROPERTYKEY, __RPC__in REFPROPVARIANT);
	IFACEMETHODIMP Commit();
	IFACEMETHODIMP IsPropertyWritable(__RPC__in REFPROPERTYKEY);

protected:
	virtual ~CComicPropertyHandler();

private:
	ULONG m_cRef;
	CComPtr<IPropertyStoreCache> m_pCache;
};