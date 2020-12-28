#pragma once

#include "Dll.h"

class _declspec(uuid("001823E8-247E-4685-BD84-350347B0460C")) CComicPropertyHandler: public CCoclassBase<IInitializeWithStream, IPropertyStore, IPropertyStoreCapabilities>
{
public:
#pragma warning (push)
#pragma warning (disable: 26429) // not null
	IFACEMETHODIMP Initialize(_In_ IStream* pStream, _In_ DWORD) override
#pragma warning (pop)
	{
		if (m_pCache)
			return HRESULT_FROM_WIN32(ERROR_ALREADY_INITIALIZED);
		static const struct
		{
			const PROPERTYKEY* pKey;
			HRESULT(*getter)(IStream&, PROPVARIANT*, UINT32&);
		} mappings[] {
			{ nullptr, ReadThumbnail },
			{ nullptr, ReadFileIdentifier },
			{ &PKEY_FileVersion, ReadFileVersion },
			{ nullptr, ReadHashData },
			{ &PKEY_Title, ReadSingleString },
			{ &PKEY_Author, ReadSingleString },
			{ &PKEY_Document_DateCreated, ReadPublished },
			{ nullptr, ReadBindingSide },
			{ &PKEY_Keywords, ReadBookmarks },
		};
		TEST(PSCreateMemoryPropertyStore(IID_PPV_ARGS(&m_pCache)));
#pragma warning (push)
#pragma warning (disable: 26494) // object must be initialized
		UINT32 version;
#pragma warning (pop)
		for (const auto& mapping : mappings)
		{
#pragma warning (push)
#pragma warning (disable: 26476) // avoid naked union
			PROPVARIANT prop;
#pragma warning (pop)
			PropVariantInit(&prop);
			auto hres = mapping.getter(*pStream, &prop, version);
			if (hres == S_OK && mapping.pKey)
			{
				hres = PSCoerceToCanonicalValue(*mapping.pKey, &prop);
				if (SUCCEEDED(hres))
					hres = m_pCache->SetValueAndState(*mapping.pKey, &prop, PSC_NORMAL);
			}
			const auto hrClear = PropVariantClear(&prop);
			const auto hrClearSuccess = SUCCEEDED(hrClear);
			if (FAILED(hres))
				return hres;
			if (!hrClearSuccess)
				return hrClear;
		}
		return S_OK;
	}

	IFACEMETHODIMP GetCount(__RPC__out DWORD* pcProps) override { return m_pCache ? m_pCache->GetCount(pcProps) : E_UNEXPECTED; }
	IFACEMETHODIMP GetAt(DWORD iProp, __RPC__out PROPERTYKEY* pKey) override { return m_pCache ? m_pCache->GetAt(iProp, pKey) : E_UNEXPECTED; }
	IFACEMETHODIMP GetValue(__RPC__in REFPROPERTYKEY key, __RPC__out PROPVARIANT* pPropVar) override { return m_pCache ? m_pCache->GetValue(key, pPropVar) : E_UNEXPECTED; }
	IFACEMETHODIMP SetValue(__RPC__in REFPROPERTYKEY, __RPC__in REFPROPVARIANT) noexcept override { return E_NOTIMPL; }
	IFACEMETHODIMP Commit() noexcept override { return E_NOTIMPL; }
	IFACEMETHODIMP IsPropertyWritable(__RPC__in REFPROPERTYKEY) noexcept override { return S_FALSE; }

private:
	CComPtr<IPropertyStoreCache> m_pCache;

	inline static constexpr UINT32 MakeVersion(UINT8 major, UINT8 minor, UINT8 revision = 0, UINT8 rebuild = 0) { return (major << 24) | (minor << 16) | (revision << 8) | rebuild; }

	static HRESULT Read7BitEncodedInt(IStream& stream, UINT32& value)
	{
		value = 0;
		for (UINT32 offset = 0; offset < 35; offset += 7)
		{
			UINT8 b;
			TEST(stream.Read(&b, sizeof(b), nullptr));
			value |= (b & 0x7f) << offset;
			if (!(b & 0x80))
				return S_OK;
		}
		return E_FAIL;
	}

	static HRESULT ReadString(IStream& stream, std::wstring& value)
	{
		UINT32 lengthInBytes;
		TEST(Read7BitEncodedInt(stream, lengthInBytes));
		value.clear();
#pragma warning (push)
#pragma warning (disable: 26494) // object must be initialized
		WCHAR buffer[64];
#pragma warning (pop)
		ULONG bytesRead = 0;
		for (UINT32 bytesReadSoFar = 0; bytesReadSoFar < lengthInBytes; bytesReadSoFar += bytesRead)
		{
#pragma warning (push)
#pragma warning (disable: 26485) // do not decay array to pointer
			TEST(stream.Read(buffer, std::min(lengthInBytes - bytesReadSoFar, static_cast<UINT32>(sizeof(buffer))), &bytesRead));
#pragma warning (pop)
			if (bytesRead == 0)
				return HRESULT_FROM_WIN32(ERROR_HANDLE_EOF);
#pragma warning (push)
#pragma warning (disable: 26485) // do not decay array to pointer
			value.append(buffer, (static_cast<size_t>(bytesRead) + 1) / 2);
#pragma warning (pop)
		}
		return S_OK;
	}

	static HRESULT ReadThumbnail(IStream& stream, PROPVARIANT*, UINT32&)
	{
		BITMAPFILEHEADER bmp;
		TEST(stream.Read(&bmp, sizeof(bmp), nullptr));
		TEST(stream.Seek(LARGE_INTEGER { bmp.bfType == 0x4D42 ? bmp.bfSize : 0 }, STREAM_SEEK_SET, nullptr));
		return S_FALSE;
	}

	static HRESULT ReadFileIdentifier(IStream& stream, PROPVARIANT*, UINT32&)
	{
		CHAR identifier[3];
#pragma warning (push)
#pragma warning (disable: 26485) // do not decay array to pointer
		TEST(stream.Read(identifier, sizeof(identifier), nullptr));
#pragma warning (pop)
		if (identifier[0] != 'C' || identifier[1] != 'I' || identifier[2] != 'C')
			return E_INVALIDARG;
		return S_FALSE;
	}

	static HRESULT ReadFileVersion(IStream& stream, PROPVARIANT* var, UINT32& version)
	{
		std::wstringstream wss;
		UINT8 major = 0, minor = 0;
		TEST(stream.Read(&major, sizeof(major), nullptr));
		wss << major;
		if (major >= 4)
		{
			TEST(stream.Read(&minor, sizeof(minor), nullptr));
			wss << "." << minor;
		}
		version = MakeVersion(major, minor);
		return InitPropVariantFromString(wss.str().c_str(), var);
	}

	static HRESULT ReadHashData(IStream& stream, PROPVARIANT*, UINT32&)
	{
		UINT8 decodeLen = 0;
		TEST(stream.Read(&decodeLen, sizeof(decodeLen), nullptr));
		TEST(stream.Seek(LARGE_INTEGER { decodeLen }, STREAM_SEEK_CUR, nullptr));
		return S_FALSE;
	}

	static HRESULT ReadSingleString(IStream& stream, PROPVARIANT* var, UINT32&)
	{
		std::wstring wstr;
		TEST(ReadString(stream, wstr));
		return InitPropVariantFromString(wstr.c_str(), var);
	}

	static HRESULT ReadPublished(IStream& stream, PROPVARIANT* var, UINT32&)
	{
		UINT8 date[4];
#pragma warning (push)
#pragma warning (disable: 26485) // do not decay array to pointer
		TEST(stream.Read(date, ARRAYSIZE(date), nullptr));
#pragma warning (pop)
		SYSTEMTIME st = { };
		st.wYear = gsl::narrow_cast<WORD>(date[0] + (date[1] << 8));
		st.wMonth = date[2];
		st.wDay = date[3];
		if (st.wYear <= 1)
			return S_FALSE;
		FILETIME ftlocal = { };
		if (!SystemTimeToFileTime(&st, &ftlocal))
			return HRESULT_FROM_WIN32(GetLastError());
		FILETIME ft = { };
		if (!LocalFileTimeToFileTime(&ftlocal, &ft))
			return HRESULT_FROM_WIN32(GetLastError());
		return InitPropVariantFromFileTime(&ft, var);
	}

#pragma warning (push)
#pragma warning (disable: 26460) // mark reference parameter as const
	static HRESULT ReadBindingSide(IStream& stream, PROPVARIANT*, UINT32& version)
#pragma warning (pop)
	{
#pragma warning (push)
#pragma warning (disable: 26494) // object must be initialized
		UINT8 boundSide;
#pragma warning (pop)
		if (version >= MakeVersion(4, 0))
			TEST(stream.Read(&boundSide, sizeof(boundSide), nullptr));
		return S_FALSE;
	}

	static HRESULT ReadBookmarks(IStream& stream, PROPVARIANT* var, UINT32&)
	{
		UINT32 bookmarks = 0;
		TEST(stream.Read(&bookmarks, sizeof(bookmarks), nullptr));
		std::wstring stringAsVector;
		for (UINT32 i = 0; i < bookmarks; ++i)
		{
			std::wstring item;
			TEST(ReadString(stream, item));
			UINT32 target;
			TEST(stream.Read(&target, sizeof(target), nullptr));
			if (i > 0)
				stringAsVector.append(L";");
			stringAsVector.append(item);
		}
		return InitPropVariantFromStringAsVector(stringAsVector.c_str(), var);
	}
};