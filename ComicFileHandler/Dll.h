#pragma once

#include <Windows.h>
#include <sal.h>

void DllAddRef() noexcept;
void DllRelease() noexcept;

template <class ...TBase>
class CCoclassBase : public TBase...
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
#pragma warning (suppress: 26447) // declared noexcept but maybe throw (InterlockedIncrement)
		return InterlockedIncrement(&m_cRef);
	}
	IFACEMETHODIMP_(ULONG) Release() noexcept override
	{
#pragma warning (suppress: 26447) // declared noexcept but maybe throw (InterlockedDecrement)
		const auto cRef = InterlockedDecrement(&m_cRef);
		if (cRef == 0)
			delete this;
		return cRef;
	}
#pragma warning (suppress: 26429) // nullability is not tested (ppv)
	IFACEMETHODIMP QueryInterface(_In_ const IID& riid, _COM_Outptr_ void** ppv) noexcept override
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
			return static_cast<T*>(this);
		if constexpr (sizeof...(Rest) == 0)
			return nullptr;
		else
			return QueryInterfaceCore<Rest...>(riid);
	}
};
