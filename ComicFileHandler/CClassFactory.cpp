#include "CClassFactory.h"

CClassFactory::CClassFactory(PFNCREATEINSTANCE pfnCreate) : m_cRef(1), m_pfnCreate(pfnCreate) { DllAddRef(); }

CClassFactory::~CClassFactory() { DllRelease(); }

IFACEMETHODIMP CClassFactory::QueryInterface(REFIID riid, void** ppv)
{
	static const QITAB qit[] =
	{
		QITABENT(CClassFactory, IClassFactory),
		{ 0 }
	};
	return QISearch(this, qit, riid, ppv);
}

IFACEMETHODIMP_(ULONG) CClassFactory::AddRef() { return static_cast<ULONG>(InterlockedIncrement(&m_cRef)); }

IFACEMETHODIMP_(ULONG) CClassFactory::Release()
{
	auto cRef = InterlockedDecrement(&m_cRef);
	if (cRef == 0)
		delete this;
	return static_cast<ULONG>(cRef);
}

#pragma warning(suppress: 6388)
IFACEMETHODIMP CClassFactory::CreateInstance(_In_opt_ IUnknown* punkOuter, _In_ REFIID riid, _COM_Outptr_ void** ppv) { return m_pfnCreate(punkOuter, riid, ppv); }

IFACEMETHODIMP CClassFactory::LockServer(BOOL fLock)
{
	if (fLock)
		DllAddRef();
	else
		DllRelease();
	return S_OK;
}