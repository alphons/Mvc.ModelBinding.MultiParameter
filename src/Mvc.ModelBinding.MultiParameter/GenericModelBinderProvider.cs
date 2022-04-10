
// GenericModelBinder
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-10
// Version: 1.2

using System.Runtime.ExceptionServices;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

#nullable enable

/// <summary>
/// An <see cref="IGetModelProvider"/> sufficient for most objects.
/// </summary>
public class GenericModelBinder : IModelBinder
{
	private readonly Type type;

	/// <summary>
	/// Initializes a new instance of <see cref="GenericModelBinder"/>.
	/// </summary>
	/// <param name="type">The type to create binder for.</param>
	public GenericModelBinder(Type type)
	{
		this.type = type ?? throw new ArgumentNullException(nameof(type));
	}

	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		if (bindingContext == null)
			throw new ArgumentNullException(nameof(ModelBindingContext));

		var defaultContext = bindingContext as DefaultModelBindingContext;
		if (defaultContext == null)
			throw new ArgumentNullException(nameof(DefaultModelBindingContext));

		var compositeValueProvider = defaultContext.OriginalValueProvider as CompositeValueProvider;
		if (compositeValueProvider == null)
			throw new ArgumentNullException(nameof(CompositeValueProvider));

		var iBindingGetModelProviders = compositeValueProvider
			.Where(x => x is IBindingSourceValueProvider provider &&
			(bindingContext.BindingSource == null ||
			provider.Filter(bindingContext.BindingSource) != null))
			.Select(x => x as IBindingSourceValueProvider)
			.ToList();

		if (iBindingGetModelProviders.FirstOrDefault(x => x != null &&
		x.ContainsPrefix(defaultContext.OriginalModelName)) is not IBindingSourceValueProvider getModelProvider)
		{
			//System.Diagnostics.Debug.WriteLine($"Bind failed on: {defaultContext.OriginalModelName}");
			return Task.CompletedTask; // Failed
		}

		try
		{
			var model = getModelProvider.GetModel(defaultContext.OriginalModelName, this.type);

			bindingContext.Result = ModelBindingResult.Success(model);

			return Task.CompletedTask;
		}
		catch (Exception exception)
		{
			var isFormatException = exception is FormatException;
			if (!isFormatException && exception.InnerException != null)
			{
				// TypeConverter throws System.Exception wrapping the FormatException,
				// so we capture the inner exception.
				exception = ExceptionDispatchInfo.Capture(exception.InnerException).SourceException;
			}

			bindingContext.ModelState.TryAddModelError(
				bindingContext.ModelName,
				exception,
				bindingContext.ModelMetadata);

			// Were able to find a converter for the type but conversion failed.
			return Task.CompletedTask;
		}
	}

}

/// <summary>
/// An <see cref="IModelBinderProvider"/> a GenericModelBinderProvider for the famous GenericModelBinder.
/// </summary>
public class GenericModelBinderProvider : IModelBinderProvider
{
	public IModelBinder? GetBinder(ModelBinderProviderContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		return new GenericModelBinder(context.Metadata.ModelType);
	}
}
