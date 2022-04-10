
// BindingSourceValueProvider
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-10
// Version: 1.2

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

#nullable enable
public abstract class BindingSourceValueProvider : IBindingSourceValueProvider
{
	/// <summary>
	/// Creates a new <see cref="BindingGetModelProvider"/>.
	/// </summary>
	/// <param name="bindingSource">
	/// The <see cref="ModelBinding.BindingSource"/>. Must be a single-source (non-composite) with
	/// <see cref="ModelBinding.BindingSource.IsGreedy"/> equal to <c>false</c>.
	/// </param>
	public BindingSourceValueProvider(BindingSource bindingSource)
	{
		if (bindingSource == null)
		{
			throw new ArgumentNullException(nameof(bindingSource));
		}

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
		if (bindingSource == null)
		{
			throw new ArgumentNullException(nameof(bindingSource));
		}

		if (bindingSource.CanAcceptDataFrom(BindingSource))
		{
			return this;
		}
		else
		{
			return null;
		}
	}

	public virtual object? GetModel(string key, Type t)
	{
		return null;
	}

}

