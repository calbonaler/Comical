#pragma once

#include <Windows.h>
#include <sal.h>

void DllAddRef() noexcept;
void DllRelease() noexcept;

#define ELIPSIS ...

template <class ...TBase>
class CCoclassBase : public TBase ELIPSIS
{
public:
	CCoclassBase() noexcept { DllAddRef(); }
	virtual ~CCoclassBase() { DllRelease(); }
	CCoclassBase(CCoclassBase&) = delete;
	CCoclassBase(CCoclassBase&&) = delete;
	CCoclassBase& operator =(CCoclassBase&) = delete;
	CCoclassBase& operator =(CCoclassBase&&) = delete;

	IFACEMETHODIMP_(ULONG) AddRef() noexcept override
	{
#pragma warning (push)
#pragma warning (disable: 26447) // declared noexcept but maybe throw (InterlockedIncrement)
		return InterlockedIncrement(&m_cRef);
#pragma warning (pop)
	}
	IFACEMETHODIMP_(ULONG) Release() noexcept override
	{
#pragma warning (push)
#pragma warning (disable: 26401) // Do not delete non-marked owner<T> (delete this)
#pragma warning (disable: 26409) // Do not new/delete directly (delete this)
#pragma warning (disable: 26447) // declared noexcept but maybe throw (InterlockedIncrement)
		const auto cRef = InterlockedDecrement(&m_cRef);
		if (cRef == 0)
			delete this;
		return cRef;
#pragma warning (pop)
	}
#pragma warning (push)
#pragma warning (disable: 26429) // nullability is not tested (ppv)
	IFACEMETHODIMP QueryInterface(_In_ const IID& riid, _COM_Outptr_ void** ppv) noexcept override
#pragma warning (pop)
	{
		*ppv = nullptr;
		void* pv = QueryInterfaceCore<TBase...>(riid);
		if (pv == nullptr)
			return E_NOINTERFACE;
		*ppv = pv;
		AddRef();
		return S_OK;
	}

private:
	ULONG m_cRef = 1;

	template <typename T, typename ...Rest>
	void* QueryInterfaceCore(_In_ const IID& riid) noexcept
	{
		if (riid == __uuidof(T) || riid == __uuidof(IUnknown))
#pragma warning (push)
#pragma warning (disable: 26474) // do not cast between pointers
			return static_cast<T*>(this);
#pragma warning (pop)
		if constexpr (sizeof...(Rest) == 0)
			return nullptr;
		else
			return QueryInterfaceCore<Rest...>(riid);
	}
};
