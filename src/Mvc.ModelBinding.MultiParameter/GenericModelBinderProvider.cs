using System.Runtime.ExceptionServices;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;


/// <summary>
/// An <see cref="IGetModelProvider"/> sufficient for most objects.
/// </summary>
public class GenericModelBinder(Type type) : IModelBinder
{
	public Task BindModelAsync(ModelBindingContext? bindingContext)
	{
		ArgumentNullException.ThrowIfNull(bindingContext);

		var defaultContext = bindingContext as DefaultModelBindingContext;

		ArgumentNullException.ThrowIfNull(defaultContext);

		var compositeValueProvider = defaultContext.OriginalValueProvider as CompositeValueProvider;

		ArgumentNullException.ThrowIfNull(compositeValueProvider);

		var bindingSource = bindingContext.BindingSource == BindingSource.FormFile ? BindingSource.Form :
			bindingContext.BindingSource;

		var iBindingGetModelProviders = compositeValueProvider
			.Where(x => x is IBindingSourceValueProvider provider &&
			(bindingSource == null || provider.Filter(bindingSource) != null))
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
			var model = getModelProvider.GetModel(defaultContext.OriginalModelName, type);

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
	public IModelBinder? GetBinder(ModelBinderProviderContext? context)
	{
		ArgumentNullException.ThrowIfNull(context);

		return new GenericModelBinder(context.Metadata.ModelType);
	}
}
