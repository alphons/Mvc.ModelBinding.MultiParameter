
// RouteValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Version: 1.2 Date: 2022-04-10
// Version: 1.3 Date: 2024-11-23

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

/// <summary>
/// A <see cref="IValueProviderFactory"/> for creating <see cref="GenericValueProvider"/> instances.
/// </summary>
public class RouteValueProviderFactory(JsonSerializerOptions? jsonSerializerOptions) : IValueProviderFactory
{
	/// <inheritdoc />
	public Task CreateValueProviderAsync(ValueProviderFactoryContext? context)
	{
		if (context?.ActionContext?.HttpContext?.Request?.RouteValues is { Count: > 0 } routeValues)
		{
			var jsonString = JsonSerializer.Serialize(routeValues, jsonSerializerOptions);

			var jsonDocument = JsonDocument.Parse(jsonString);

			context.ValueProviders.Add(new GenericValueProvider(
				BindingSource.Path,
				jsonDocument: jsonDocument,
				formCollection: null,
				jsonSerializerOptions: jsonSerializerOptions));
		}

		return Task.CompletedTask;
	}
}
