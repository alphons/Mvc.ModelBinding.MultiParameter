namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

public interface IBindingSourceValueProvider : IValueProvider
{
	IValueProvider? Filter(BindingSource bindingSource);

	object? GetModel(string key, Type t);
}
