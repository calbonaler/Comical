#pragma once

#include <Shlwapi.h>
#include <thumbcache.h>

class _declspec(uuid("{4423CDF9-0C1B-4F23-8CC4-BA634252CD6A}")) CComicThumbnailProvider final : public IInitializeWithStream, public IThumbnailProvider
{
public:
	CComicThumbnailProvider() { DllAddRef(); }

	BEGIN_COM_INTERFACE_MAPPPING
		QITABENT(CComicThumbnailProvider, IInitializeWithStream),
		QITABENT(CComicThumbnailProvider, IThumbnailProvider),
	END_COM_INTER_FACE_MAPPING

	IFACEMETHODIMP Initialize(_In_ IStream* pStream, _In_ DWORD) { return m_pStream ? HRESULT_FROM_WIN32(ERROR_ALREADY_INITIALIZED) : pStream->QueryInterface(&m_pStream); }

	IFACEMETHODIMP GetThumbnail(UINT, __RPC__deref_out_opt HBITMAP*, __RPC__out WTS_ALPHATYPE*);

private:
	CComPtr<IStream> m_pStream;
	
	~CComicThumbnailProvider() { DllRelease(); }
};
