#pragma once

#include <Shlwapi.h>
#include <thumbcache.h>

class _declspec(uuid("{4423CDF9-0C1B-4F23-8CC4-BA634252CD6A}")) CComicThumbnailProvider : public IInitializeWithStream, public IThumbnailProvider
{
public:
	CComicThumbnailProvider();

	IFACEMETHODIMP QueryInterface(REFIID, void**);
	IFACEMETHODIMP_(ULONG) AddRef();
	IFACEMETHODIMP_(ULONG) Release();

	IFACEMETHODIMP Initialize(IStream*, DWORD);

	IFACEMETHODIMP GetThumbnail(UINT, HBITMAP*, WTS_ALPHATYPE*);

protected:
	virtual ~CComicThumbnailProvider();

private:
	ULONG m_cRef;
	IStream* m_pStream;
};
