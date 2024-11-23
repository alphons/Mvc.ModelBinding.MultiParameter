
// QueryStringValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Version: 1.2 Date: 2022-04-10
// Version: 1.3 Date: 2024-11-23

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Mvc.ModelBinding.MultiParameter;
using System.Net;
using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;


/// <summary>
/// A <see cref="IValueProviderFactory"/> that creates <see cref="GenericValueProvider"/> instances that
/// read values from the request query-string.
/// </summary>
public class QueryStringValueProviderFactory : IValueProviderFactory
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;
	public QueryStringValueProviderFactory(JsonSerializerOptions? Options) : base()
	{
		this.jsonSerializerOptions = Options;
	}

	public QueryStringValueProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}

	/// <inheritdoc />
	public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		var query = context.ActionContext.HttpContext.Request.Query;
		if (query != null && query.Count > 0)
		{
			var list = query
				.Select(x => $"\"{Helper.EscapeForJson(x.Key)}\": \"{Helper.EscapeForJson(x.Value)}\"")
				.ToArray();
			var json = $"{{{string.Join(',', list)}}}";
			var jsonDocument = JsonDocument.Parse(json, options: default);

			var valueProvider = new GenericValueProvider(
				BindingSource.Query,
				jsonDocument,
				null,
				this.jsonSerializerOptions);

			context.ValueProviders.Add(valueProvider);
		}

		return Task.CompletedTask;
	}
}
