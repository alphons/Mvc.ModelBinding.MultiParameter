
// QueryStringValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Version: 1.2 Date: 2022-04-10
// Version: 1.3 Date: 2024-11-23

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;


/// <summary>
/// A <see cref="IValueProviderFactory"/> that creates <see cref="GenericValueProvider"/> instances that
/// read values from the request query-string.
/// </summary>
public class QueryStringValueProviderFactory(JsonSerializerOptions? jsonSerializerOptions) : IValueProviderFactory
{
	/// <inheritdoc />
	public Task CreateValueProviderAsync(ValueProviderFactoryContext? context)
	{
		if (context?.ActionContext?.HttpContext?.Request?.Query is { Count: > 0 } query)
		{
			var queryDictionary = query.ToDictionary(
			pair => pair.Key,
			pair => pair.Value.Count > 1
				? pair.Value.ToArray()              // Convert multiple values to an array
				: (object)pair.Value.ToString());   // Use a string for a single value


			var jsonDocument = JsonDocument.Parse(
				JsonSerializer.Serialize(
					queryDictionary, 
					jsonSerializerOptions));

			context.ValueProviders.Add(new GenericValueProvider(
				BindingSource.Query,
				jsonDocument,
				formCollection: null,
				jsonSerializerOptions: jsonSerializerOptions
			));
		}

		return Task.CompletedTask;
	}
}
