#pragma once

#include <ShObjIdl.h>
#include "Dll.h"

class CClassFactory final : public IClassFactory
{
public:
	CClassFactory(PFNCREATEINSTANCE pfnCreate) : m_pfnCreate(pfnCreate) { DllAddRef(); }

	BEGIN_COM_INTERFACE_MAPPPING
		QITABENT(CClassFactory, IClassFactory)
	END_COM_INTER_FACE_MAPPING

	IFACEMETHODIMP CreateInstance(_In_opt_ IUnknown* punkOuter, _In_ REFIID riid, _COM_Outptr_ void** ppv)
	{
		*ppv = nullptr;
		if (punkOuter)
			return CLASS_E_NOAGGREGATION;
		return m_pfnCreate(riid, ppv);
	}
	IFACEMETHODIMP LockServer(BOOL fLock)
	{
		if (fLock)
			DllAddRef();
		else
			DllRelease();
		return S_OK;
	}

private:
	PFNCREATEINSTANCE m_pfnCreate;
	
	~CClassFactory() { DllRelease(); }
};
