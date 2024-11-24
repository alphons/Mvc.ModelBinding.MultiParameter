
// BindingSourceValueProvider
// (C) 2022 Alphons van der Heijden
// Version: 1.2 Date: 2022-04-10
// Version: 1.3 Date: 2024-11-23

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public abstract class BindingSourceValueProvider : IBindingSourceValueProvider
{
	/// <summary>
	/// Initializes a new instance of the <see cref="BindingSourceValueProvider"/> class.
	/// </summary>
	/// <param name="bindingSource">
	/// The <see cref="BindingSource"/> associated with this provider. 
	/// Must be a single-source (non-composite) with <see cref="BindingSource.IsGreedy"/> set to <c>false</c>.
	/// </param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="bindingSource"/> is null.</exception>
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

		return bindingSource.CanAcceptDataFrom(BindingSource) ? this : null;
	}

	public virtual object? GetModel(string key, Type t)
	{
		return null;
	}

}

