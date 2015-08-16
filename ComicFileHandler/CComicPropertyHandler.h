#pragma once

#include <ShObjIdl.h>
#include <propkey.h>
#include <propvarutil.h>
#include <sstream>
#include "Dll.h"

class _declspec(uuid("001823E8-247E-4685-BD84-350347B0460C")) CComicPropertyHandler final : public IInitializeWithStream, public IPropertyStore, public IPropertyStoreCapabilities
{
public:
	CComicPropertyHandler() { DllAddRef(); }

	BEGIN_COM_INTERFACE_MAPPPING
		QITABENT(CComicPropertyHandler, IPropertyStore),
		QITABENT(CComicPropertyHandler, IPropertyStoreCapabilities),
		QITABENT(CComicPropertyHandler, IInitializeWithStream)
	END_COM_INTER_FACE_MAPPING

	IFACEMETHODIMP Initialize(_In_ IStream* pStream, _In_ DWORD)
	{
		if (m_pCache.p)
			return HRESULT_FROM_WIN32(ERROR_ALREADY_INITIALIZED);
		static const struct
		{
			const PROPERTYKEY* pKey;
			HRESULT(*getter)(IStream*, PROPVARIANT*, Version&);
		} mapping[] = {
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
		Version version;
		PROPVARIANT prop = { };
		for (size_t i = 0; i < ARRAYSIZE(mapping); ++i)
		{
			auto hres = mapping[i].getter(pStream, &prop, version);
			if (FAILED(hres))
				return hres;
			if (hres != S_OK)
				continue;
			if (mapping[i].pKey)
			{
				TEST(PSCoerceToCanonicalValue(*mapping[i].pKey, &prop));
				TEST(m_pCache->SetValueAndState(*mapping[i].pKey, &prop, PSC_NORMAL));
			}
			TEST(PropVariantClear(&prop));
		}
		return S_OK;
	}

	IFACEMETHODIMP GetCount(__RPC__out DWORD* pcProps) { return m_pCache.p ? m_pCache->GetCount(pcProps) : E_UNEXPECTED; }
	IFACEMETHODIMP GetAt(DWORD iProp, __RPC__out PROPERTYKEY* pKey) { return m_pCache.p ? m_pCache->GetAt(iProp, pKey) : E_UNEXPECTED; }
	IFACEMETHODIMP GetValue(__RPC__in REFPROPERTYKEY key, __RPC__out PROPVARIANT* pPropVar) { return m_pCache.p ? m_pCache->GetValue(key, pPropVar) : E_UNEXPECTED; }
	IFACEMETHODIMP SetValue(__RPC__in REFPROPERTYKEY, __RPC__in REFPROPVARIANT) { return E_NOTIMPL; }
	IFACEMETHODIMP Commit() { return E_NOTIMPL; }
	IFACEMETHODIMP IsPropertyWritable(__RPC__in REFPROPERTYKEY) { return S_FALSE; }

private:
	CComPtr<IPropertyStoreCache> m_pCache;

	~CComicPropertyHandler() { DllRelease(); }

	struct Version
	{
		Version() : _major(0), _minor(0), _revision(0), _rebuild(0) { }
		Version(UINT8 major, UINT8 minor, UINT8 revision, UINT8 rebuild) : _major(major), _minor(minor), _revision(revision), _rebuild(rebuild) { }
		Version(const Version& src) : _major(src._major), _minor(src._minor), _revision(src._revision), _rebuild(src._rebuild) { }
		Version& operator=(const Version& src)
		{
			_major = src._major;
			_minor = src._minor;
			_revision = src._revision;
			_rebuild = src._rebuild;
			return *this;
		}

		UINT8 Major() const { return _major; }
		UINT8 Minor() const { return _minor; }
		UINT8 Revision() const { return _revision; }
		UINT8 Rebuild() const { return _rebuild; }

		bool operator==(const Version& right) const { return ToUInt32() == right.ToUInt32(); }
		bool operator!=(const Version& right) const { return !(*this == right); }
		bool operator<(const Version& right) const { return ToUInt32() < right.ToUInt32(); }
		bool operator>(const Version& right) const { return right < *this; }
		bool operator<=(const Version& right) const { return !(right < *this); }
		bool operator>=(const Version& right) const { return !(*this < right); }

	private:
		UINT8 _major;
		UINT8 _minor;
		UINT8 _revision;
		UINT8 _rebuild;

		UINT32 ToUInt32() const { return (_major << 24) | (_minor << 16) | (_revision << 8) | _rebuild; }
	};

	static HRESULT Read7BitEncodedInt(IStream* stream, UINT& value)
	{
		value = 0;
		UINT offset = 0;
		UINT8 current = 0;
		do
		{
			if (offset == 35)
				return E_FAIL;
			TEST(stream->Read(&current, sizeof(current), nullptr));
			value |= (current & 0x7F) << (offset & 0x1F);
			offset += 7;
		} while ((current & 0x80) != 0);
		return S_OK;
	}

	static HRESULT ReadString(IStream* stream, std::wstring& value)
	{
		UINT length;
		TEST(Read7BitEncodedInt(stream, length));
		if (length == 0)
		{
			value = L"";
			return S_OK;
		}
		UINT8 charBytes[0x80] = { 0 };
		ULONG bytesRead = 0;
		for (UINT offset = 0; offset < length; offset += bytesRead)
		{
			TEST(stream->Read(charBytes, min(ARRAYSIZE(charBytes), length - offset), &bytesRead));
			if (bytesRead == 0)
				return E_FAIL;
			if (offset == 0 && bytesRead == length)
			{
				value = std::wstring(pointer_cast<PWSTR>(charBytes));
				return S_OK;
			}
			value.append(pointer_cast<PWSTR>(charBytes));
		}
		return S_OK;
	}

	static HRESULT ReadThumbnail(IStream* stream, PROPVARIANT*, Version&)
	{
		BITMAPFILEHEADER bmp;
		TEST(stream->Read(&bmp, sizeof(bmp), nullptr));
		LARGE_INTEGER li;
		li.QuadPart = bmp.bfType == *(pointer_cast<WORD*>("BM")) ? bmp.bfSize : 0;
		TEST(stream->Seek(li, STREAM_SEEK_SET, nullptr));
		return S_FALSE;
	}

	static HRESULT ReadFileIdentifier(IStream* stream, PROPVARIANT*, Version&)
	{
		CHAR identifier[3];
		TEST(stream->Read(identifier, sizeof(identifier), nullptr));
		if (identifier[0] != 'C' || identifier[1] != 'I' || identifier[2] != 'C')
			return E_INVALIDARG;
		return S_FALSE;
	}

	static HRESULT ReadFileVersion(IStream* stream, PROPVARIANT* var, Version& version)
	{
		std::wstringstream wss;
		UINT8 major = 0, minor = 0;
		TEST(stream->Read(&major, sizeof(major), nullptr));
		wss << major;
		if (major >= 4)
		{
			TEST(stream->Read(&minor, sizeof(minor), nullptr));
			wss << "." << minor;
		}
		version = Version(major, minor, 0, 0);
		return InitPropVariantFromString(wss.str().c_str(), var);
	}

	static HRESULT ReadHashData(IStream* stream, PROPVARIANT*, Version&)
	{
		UINT8 decodeLen = 0;
		TEST(stream->Read(&decodeLen, sizeof(decodeLen), nullptr));
		LARGE_INTEGER li;
		li.QuadPart = decodeLen;
		TEST(stream->Seek(li, STREAM_SEEK_CUR, nullptr));
		return S_FALSE;
	}

	static HRESULT ReadSingleString(IStream* stream, PROPVARIANT* var, Version&)
	{
		std::wstring wstr;
		TEST(ReadString(stream, wstr));
		return InitPropVariantFromString(wstr.c_str(), var);
	}

	static HRESULT ReadDateOfPublication(IStream* stream, PROPVARIANT* var, Version&)
	{
		UINT8 date[4];
		TEST(stream->Read(date, ARRAYSIZE(date), nullptr));
		SYSTEMTIME st = { };
		st.wYear = static_cast<WORD>(date[0] + (date[1] << 8));
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

	static HRESULT ReadBoundSide(IStream* stream, PROPVARIANT*, Version& version)
	{
		UINT8 boundSide;
		if (version >= Version(4, 0, 0, 0))
			TEST(stream->Read(&boundSide, sizeof(boundSide), nullptr));
		return S_FALSE;
	}

	static HRESULT ReadBookmarks(IStream* stream, PROPVARIANT* var, Version&)
	{
		UINT32 bookmarks = 0;
		TEST(stream->Read(&bookmarks, sizeof(bookmarks), nullptr));
		std::wstring stringAsVector;
		for (UINT32 i = 0; i < bookmarks; ++i)
		{
			std::wstring item;
			TEST(ReadString(stream, item));
			UINT32 target;
			TEST(stream->Read(&target, sizeof(target), nullptr));
			if (i > 0)
				stringAsVector.append(L";");
			stringAsVector.append(item);
		}
		return InitPropVariantFromStringAsVector(stringAsVector.c_str(), var);
	}
};