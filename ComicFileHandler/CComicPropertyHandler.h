#pragma once

#include <ShObjIdl.h>

class _declspec(uuid("001823E8-247E-4685-BD84-350347B0460C")) CComicPropertyHandler : public IInitializeWithStream, public IPropertyStore, public IPropertyStoreCapabilities
{
public:
	CComicPropertyHandler();

	IFACEMETHODIMP QueryInterface(REFIID, void**);
	IFACEMETHODIMP_(ULONG) AddRef();
	IFACEMETHODIMP_(ULONG) Release();
	
	IFACEMETHODIMP Initialize(IStream*, DWORD);

	IFACEMETHODIMP GetCount(DWORD*);
	IFACEMETHODIMP GetAt(DWORD, PROPERTYKEY*);
	IFACEMETHODIMP GetValue(REFPROPERTYKEY, PROPVARIANT*);
	IFACEMETHODIMP SetValue(REFPROPERTYKEY, REFPROPVARIANT);
	IFACEMETHODIMP Commit();
	IFACEMETHODIMP IsPropertyWritable(REFPROPERTYKEY);

protected:
	virtual ~CComicPropertyHandler();

private:
	ULONG m_cRef;
	IPropertyStoreCache* m_pCache;
};