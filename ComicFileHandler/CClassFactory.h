#pragma once

#include <ShObjIdl.h>
#include "Dll.h"

class CClassFactory : public IClassFactory
{
public:
	CClassFactory(PFNCREATEINSTANCE);

	IFACEMETHODIMP QueryInterface(REFIID, void**);
	IFACEMETHODIMP_(ULONG) AddRef();
	IFACEMETHODIMP_(ULONG) Release();

	IFACEMETHODIMP CreateInstance(_In_opt_ IUnknown*, _In_ REFIID, _COM_Outptr_ void**);
	IFACEMETHODIMP LockServer(BOOL);

protected:
	virtual ~CClassFactory();

private:
	long m_cRef;
	PFNCREATEINSTANCE m_pfnCreate;
};
