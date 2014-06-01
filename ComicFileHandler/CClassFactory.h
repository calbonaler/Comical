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

	IFACEMETHODIMP CreateInstance(IUnknown*, REFIID, void**);
	IFACEMETHODIMP LockServer(BOOL);

protected:
	virtual ~CClassFactory();

private:
	long m_cRef;
	PFNCREATEINSTANCE m_pfnCreate;
};
