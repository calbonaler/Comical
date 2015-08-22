#pragma once

#include <thumbcache.h>
#include <wincodec.h>
#include "Dll.h"

class _declspec(uuid("{4423CDF9-0C1B-4F23-8CC4-BA634252CD6A}")) CComicThumbnailProvider final : public IInitializeWithStream, public IThumbnailProvider
{
public:
	CComicThumbnailProvider() { DllAddRef(); }

#pragma warning (push)
#pragma warning (disable: 4838)
	BEGIN_COM_INTERFACE_MAPPPING
		QITABENT(CComicThumbnailProvider, IInitializeWithStream),
		QITABENT(CComicThumbnailProvider, IThumbnailProvider)
	END_COM_INTER_FACE_MAPPING
#pragma warning (pop)

	IFACEMETHODIMP Initialize(_In_ IStream* pStream, _In_ DWORD) { return m_pStream ? HRESULT_FROM_WIN32(ERROR_ALREADY_INITIALIZED) : pStream->QueryInterface(&m_pStream); }

	IFACEMETHODIMP GetThumbnail(UINT, __RPC__deref_out_opt HBITMAP* phbmp, __RPC__out WTS_ALPHATYPE* pdwAlpha)
	{
		*phbmp = nullptr;
		CComPtr<IWICImagingFactory> pImagingFactory;
		TEST(pImagingFactory.CoCreateInstance(CLSID_WICImagingFactory, nullptr, CLSCTX_INPROC_SERVER));
		CComPtr<IWICBitmapDecoder> pDecoder;
		TEST(pImagingFactory->CreateDecoderFromStream(m_pStream, &GUID_VendorMicrosoft, WICDecodeMetadataCacheOnDemand, &pDecoder));
		CComPtr<IWICBitmapFrameDecode> pBitmapFrameDecode;
		TEST(pDecoder->GetFrame(0, &pBitmapFrameDecode));
		WICPixelFormatGUID guidPixelFormatSource;
		TEST(pBitmapFrameDecode->GetPixelFormat(&guidPixelFormatSource));
		CComPtr<IWICBitmapSource> pBitmapSourceConverted = nullptr;
		if (guidPixelFormatSource != GUID_WICPixelFormat32bppBGRA)
		{
			CComPtr<IWICFormatConverter> pFormatConverter;
			TEST(pImagingFactory->CreateFormatConverter(&pFormatConverter));
			TEST(pFormatConverter->Initialize(pBitmapFrameDecode, GUID_WICPixelFormat32bppBGRA, WICBitmapDitherTypeNone, nullptr, 0, WICBitmapPaletteTypeCustom));
			TEST(pFormatConverter->QueryInterface(&pBitmapSourceConverted));
		}
		else
			TEST(pBitmapFrameDecode->QueryInterface(&pBitmapSourceConverted));
		UINT nWidth, nHeight;
		TEST(pBitmapSourceConverted->GetSize(&nWidth, &nHeight));
		BITMAPINFO bmi = { sizeof(bmi.bmiHeader) };
		bmi.bmiHeader.biWidth = nWidth;
		bmi.bmiHeader.biHeight = -static_cast<LONG>(nHeight);
		bmi.bmiHeader.biPlanes = 1;
		bmi.bmiHeader.biBitCount = 32;
		BYTE* pBits;
		auto hbmp = CreateDIBSection(nullptr, &bmi, DIB_RGB_COLORS, pointer_cast<void**>(&pBits), nullptr, 0);
		if (!hbmp)
			return E_OUTOFMEMORY;
		auto hr = pBitmapSourceConverted->CopyPixels(nullptr, nWidth * 4, nWidth * nHeight * 4, pBits);
		if (SUCCEEDED(hr))
		{
			*pdwAlpha = WTSAT_ARGB;
			*phbmp = hbmp;
		}
		else
			DeleteObject(hbmp);
		return hr;
	}

private:
	CComPtr<IStream> m_pStream;
	
	~CComicThumbnailProvider() { DllRelease(); }
};
