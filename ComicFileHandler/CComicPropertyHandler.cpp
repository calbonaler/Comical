#include "Dll.h"
#include "CComicPropertyHandler.h"
#include <propkey.h>
#include <propvarutil.h>
#include <sstream>

using namespace std;

struct VERSION
{
	UINT8 bMajor;
	UINT8 bMinor;
	UINT8 bRevision;
	UINT8 bRebuild;
};

struct PROPERTYMAP
{
	const PROPERTYKEY* pKey;
	HRESULT(*getter)(IStream*, PROPVARIANT*, VERSION*);
};

HRESULT Read7BitEncodedInt(IStream* pStream, UINT* puiValue)
{
	*puiValue = 0;
	UINT uiOffset = 0;
	UINT8 bCur = 0;
	do
	{
		if (uiOffset == 35)
			return E_FAIL;
		TEST(pStream->Read(&bCur, sizeof(bCur), nullptr));
		*puiValue |= (bCur & 0x7F) << (uiOffset & 0x1F);
		uiOffset += 7;
	} while ((bCur & 0x80) != 0);
	return S_OK;
}

HRESULT ReadString(IStream* pStream, wstring* pwstr)
{
	UINT uiLength;
	TEST(Read7BitEncodedInt(pStream, &uiLength));
	if (uiLength == 0)
	{
		if (pwstr)
			*pwstr = L"";
		return S_OK;
	}
	UINT8 charBytes[0x80] = { 0 };
	ULONG ulRead = 0;
	for (UINT uiOffset = 0; uiOffset < uiLength; uiOffset += ulRead)
	{
		TEST(pStream->Read(charBytes, min(ARRAYSIZE(charBytes), uiLength - uiOffset), &ulRead));
		if (ulRead == 0)
			return E_FAIL;
		if (uiOffset == 0 && ulRead == uiLength)
		{
			if (pwstr)
				*pwstr = wstring(pointer_cast<PWSTR>(charBytes));
			return S_OK;
		}
		if (pwstr)
			pwstr->append(pointer_cast<PWSTR>(charBytes));
	}
	return S_OK;
}

HRESULT ReadThumbnail(IStream* pStream, PROPVARIANT*, VERSION*)
{
	BITMAPFILEHEADER bmp;
	TEST(pStream->Read(&bmp, sizeof(bmp), nullptr));
	LARGE_INTEGER li;
	li.QuadPart = bmp.bfType == *(pointer_cast<WORD*>("BM")) ? bmp.bfSize : 0;
	TEST(pStream->Seek(li, STREAM_SEEK_SET, nullptr));
	return S_FALSE;
}

HRESULT ReadFileIdentifier(IStream* pStream, PROPVARIANT*, VERSION*)
{
	CHAR identifier[3];
	TEST(pStream->Read(identifier, sizeof(identifier), nullptr));
	if (identifier[0] != 'C' || identifier[1] != 'I' || identifier[2] != 'C')
		return E_INVALIDARG;
	return S_FALSE;
}

HRESULT ReadFileVersion(IStream* pStream, PROPVARIANT* pvar, VERSION* version)
{
	wstringstream wss;
	TEST(pStream->Read(&version->bMajor, sizeof(version->bMajor), nullptr));
	wss << version->bMajor;
	if (version->bMajor >= 4)
	{
		TEST(pStream->Read(&version->bMinor, sizeof(version->bMinor), nullptr));
		wss << "." << version->bMinor;
	}
	return InitPropVariantFromString(wss.str().c_str(), pvar);
}

HRESULT ReadHashData(IStream* pStream, PROPVARIANT*, VERSION*)
{
	UINT8 bDecodeLen = 0;
	TEST(pStream->Read(&bDecodeLen, sizeof(bDecodeLen), nullptr));
	LARGE_INTEGER li;
	li.QuadPart = bDecodeLen;
	TEST(pStream->Seek(li, STREAM_SEEK_CUR, nullptr));
	return S_FALSE;
}

HRESULT ReadSingleString(IStream* pStream, PROPVARIANT* pvar, VERSION*)
{
	wstring wstr;
	TEST(ReadString(pStream, &wstr));
	return InitPropVariantFromString(wstr.c_str(), pvar);
}

HRESULT ReadDateOfPublication(IStream* pStream, PROPVARIANT* pvar, VERSION*)
{
	UINT8 bDate[4];
	TEST(pStream->Read(bDate, ARRAYSIZE(bDate), nullptr));
	SYSTEMTIME st = { };
	st.wYear = static_cast<WORD>(bDate[0] + (bDate[1] << 8));
	st.wMonth = bDate[2];
	st.wDay = bDate[3];
	if (st.wYear <= 1)
		return S_FALSE;
	FILETIME ftlocal = { };
	if (!SystemTimeToFileTime(&st, &ftlocal))
		return HRESULT_FROM_WIN32(GetLastError());
	FILETIME ft = { };
	if (!LocalFileTimeToFileTime(&ftlocal, &ft))
		return HRESULT_FROM_WIN32(GetLastError());
	return InitPropVariantFromFileTime(&ft, pvar);
}

HRESULT ReadBoundSide(IStream* pStream, PROPVARIANT*, VERSION* version)
{
	UINT8 boundSide;
	if (version->bMajor >= 4)
		TEST(pStream->Read(&boundSide, sizeof(boundSide), nullptr));
	return S_FALSE;
}

HRESULT ReadBookmarks(IStream* pStream, PROPVARIANT* pvar, VERSION*)
{
	UINT32 uiBookmarks = 0;
	TEST(pStream->Read(&uiBookmarks, sizeof(uiBookmarks), nullptr));
	wstring wstrStringAsVector;
	wstring wstrItem;
	for (UINT32 i = 0; i < uiBookmarks; ++i)
	{
		TEST(ReadString(pStream, &wstrItem));
		UINT32 uiTarget;
		TEST(pStream->Read(&uiTarget, sizeof(uiTarget), nullptr));
		if (i > 0)
			wstrStringAsVector.append(L";");
		wstrStringAsVector.append(wstrItem);
	}
	return InitPropVariantFromStringAsVector(wstrStringAsVector.c_str(), pvar);
}

IFACEMETHODIMP CComicPropertyHandler::Initialize(_In_ IStream* pStream, _In_ DWORD)
{
	if (m_pCache.p)
		return HRESULT_FROM_WIN32(ERROR_ALREADY_INITIALIZED);
	static const PROPERTYMAP mapping[] =
	{
		{ nullptr, ReadThumbnail },
		{ nullptr, ReadFileIdentifier },
		{ &PKEY_FileVersion, ReadFileVersion },
		{ nullptr, ReadHashData },
		{ &PKEY_Title, ReadSingleString },
		{ &PKEY_Author, ReadSingleString },
		{ &PKEY_Document_DateCreated, ReadDateOfPublication },
		{ nullptr, ReadBoundSide },
		{ &PKEY_Keywords, ReadBookmarks },
	};
	TEST(PSCreateMemoryPropertyStore(IID_PPV_ARGS(&m_pCache)));
	VERSION version = { };
	PROPVARIANT prop = { };
	for (size_t i = 0; i < ARRAYSIZE(mapping); ++i)
	{
		auto hres = mapping[i].getter(pStream, &prop, &version);
		if (FAILED(hres))
			return hres;
		if (hres == S_OK)
		{
			if (mapping[i].pKey)
			{
				TEST(PSCoerceToCanonicalValue(*mapping[i].pKey, &prop));
				TEST(m_pCache->SetValueAndState(*mapping[i].pKey, &prop, PSC_NORMAL));
			}
			TEST(PropVariantClear(&prop));
		}
	}
	return S_OK;
}