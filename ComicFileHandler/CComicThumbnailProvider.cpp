#include <wincodec.h>
#include "Dll.h"
#include "CComicThumbnailProvider.h"

#pragma comment(lib, "Shlwapi.lib")

CComicThumbnailProvider::CComicThumbnailProvider() : m_cRef(1), m_pStream(nullptr) { DllAddRef(); }

CComicThumbnailProvider::~CComicThumbnailProvider() { DllRelease(); }

IFACEMETHODIMP CComicThumbnailProvider::QueryInterface(REFIID riid, void** ppv)
{
	static const QITAB qit[] =
	{
		QITABENT(CComicThumbnailProvider, IInitializeWithStream),
		QITABENT(CComicThumbnailProvider, IThumbnailProvider),
		{ 0 },
	};
	return QISearch(this, qit, riid, ppv);
}

IFACEMETHODIMP_(ULONG) CComicThumbnailProvider::AddRef() { return InterlockedIncrement(&m_cRef); }

IFACEMETHODIMP_(ULONG) CComicThumbnailProvider::Release()
{
	auto cRef = InterlockedDecrement(&m_cRef);
	if (!cRef)
		delete this;
	return cRef;
}

IFACEMETHODIMP CComicThumbnailProvider::Initialize(IStream* pStream, DWORD) { return m_pStream ? HRESULT_FROM_WIN32(ERROR_ALREADY_INITIALIZED) : pStream->QueryInterface(&m_pStream); }

IFACEMETHODIMP CComicThumbnailProvider::GetThumbnail(UINT, HBITMAP* phbmp, WTS_ALPHATYPE* pdwAlpha)
{
	*phbmp = nullptr;
	IWICImagingFactory* pImagingFactory;
	auto hr = CoCreateInstance(CLSID_WICImagingFactory, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&pImagingFactory));
	if (SUCCEEDED(hr))
	{
		IWICBitmapDecoder* pDecoder;
		if (SUCCEEDED(hr = pImagingFactory->CreateDecoderFromStream(m_pStream, &GUID_VendorMicrosoft, WICDecodeMetadataCacheOnDemand, &pDecoder)))
		{
			IWICBitmapFrameDecode* pBitmapFrameDecode;
			if (SUCCEEDED(hr = pDecoder->GetFrame(0, &pBitmapFrameDecode)))
			{
				IWICBitmapSource* pBitmapSourceConverted = nullptr;
				WICPixelFormatGUID guidPixelFormatSource;
				if (SUCCEEDED(hr = pBitmapFrameDecode->GetPixelFormat(&guidPixelFormatSource)))
				{
					if (guidPixelFormatSource != GUID_WICPixelFormat32bppBGRA)
					{
						IWICFormatConverter* pFormatConverter;
						if (SUCCEEDED(hr = pImagingFactory->CreateFormatConverter(&pFormatConverter)))
						{
							if (SUCCEEDED(hr = pFormatConverter->Initialize(pBitmapFrameDecode, GUID_WICPixelFormat32bppBGRA, WICBitmapDitherTypeNone, nullptr, 0, WICBitmapPaletteTypeCustom)))
								hr = pFormatConverter->QueryInterface(&pBitmapSourceConverted);
							pFormatConverter->Release();
						}
					}
					else
						hr = pBitmapFrameDecode->QueryInterface(&pBitmapSourceConverted);
				}
				if (SUCCEEDED(hr))
				{
					UINT nWidth, nHeight;
					if (SUCCEEDED(hr = pBitmapSourceConverted->GetSize(&nWidth, &nHeight)))
					{
						BITMAPINFO bmi = { sizeof(bmi.bmiHeader) };
						bmi.bmiHeader.biWidth = nWidth;
						bmi.bmiHeader.biHeight = -static_cast<LONG>(nHeight);
						bmi.bmiHeader.biPlanes = 1;
						bmi.bmiHeader.biBitCount = 32;
						BYTE* pBits;
						auto hbmp = CreateDIBSection(nullptr, &bmi, DIB_RGB_COLORS, pointer_cast<void**>(&pBits), nullptr, 0);
						if (SUCCEEDED(hr = hbmp ? S_OK : E_OUTOFMEMORY))
						{
							if (SUCCEEDED(hr = pBitmapSourceConverted->CopyPixels(nullptr, nWidth * 4, nWidth * nHeight * 4, pBits)))
							{
								*pdwAlpha = WTSAT_ARGB;
								*phbmp = hbmp;
							}
							else
								DeleteObject(hbmp);
						}
					}
					pBitmapSourceConverted->Release();
				}
				pBitmapFrameDecode->Release();
			}
			pDecoder->Release();
		}
		pImagingFactory->Release();
	}
	return hr;
}