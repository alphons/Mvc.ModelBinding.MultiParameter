
// BindingSourceValueProvider
// (C) 2022 Alphons van der Heijden
// Version: 1.2 Date: 2022-04-10
// Version: 1.3 Date: 2024-11-23

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public abstract class BindingSourceValueProvider : IBindingSourceValueProvider
{
	/// <summary>
	/// Creates a new <see cref="BindingGetModelProvider"/>.
	/// </summary>
	/// <param name="bindingSource">
	/// The <see cref="ModelBinding.BindingSource"/>. Must be a single-source (non-composite) with
	/// <see cref="BindingSource.IsGreedy"/> equal to <c>false</c>.
	/// </param>
	public BindingSourceValueProvider(BindingSource bindingSource)
	{
		ArgumentNullException.ThrowIfNull(bindingSource);

		BindingSource = bindingSource;
	}

	/// <summary>
	/// Gets the corresponding <see cref="ModelBinding.BindingSource"/>.
	/// </summary>
	protected BindingSource BindingSource { get; }

	/// <inheritdoc />
	public abstract bool ContainsPrefix(string prefix);

	/// <inheritdoc />
	public virtual ValueProviderResult GetValue(string key)
	{
		return ValueProviderResult.None;
	}

	/// <inheritdoc />
	public virtual IValueProvider? Filter(BindingSource bindingSource)
	{
		ArgumentNullException.ThrowIfNull(bindingSource);

		if (bindingSource.CanAcceptDataFrom(BindingSource))
			return this;
		else
			return null;
	}

	public virtual object? GetModel(string key, Type t)
	{
		return null;
	}

}

