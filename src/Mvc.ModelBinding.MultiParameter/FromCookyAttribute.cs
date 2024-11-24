using Microsoft.AspNetCore.Http.Metadata;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

//
// Summary:
//     Specifies that a parameter or property should be bound using the request cookies.

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class FromCookyAttribute : Attribute, IBindingSourceMetadata, IModelNameProvider, IFromHeaderMetadata
{
	public BindingSource BindingSource
	{
		get
		{
			return BindingSource.Special;
		}
	}

	public string? Name { get; set; }

	public FromCookyAttribute()
	{
	}
}