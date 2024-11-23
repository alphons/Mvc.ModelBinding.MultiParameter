
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
public class RouteValueProviderFactory : IValueProviderFactory
{
    private readonly JsonSerializerOptions? jsonSerializerOptions;

    public RouteValueProviderFactory(JsonSerializerOptions? Options) : base()
    {
        this.jsonSerializerOptions = Options;
    }
    public RouteValueProviderFactory()
    {
        this.jsonSerializerOptions = null;
    }

    /// <inheritdoc />
    public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
		ArgumentNullException.ThrowIfNull(context);

        var request = context.ActionContext.HttpContext.Request;
        var jsonString = JsonSerializer.Serialize(request.RouteValues);
        var jsonDocument = JsonDocument.Parse(jsonString, options: default);

        var valueProvider = new GenericValueProvider(
            BindingSource.Path,
            jsonDocument,
            null,
            this.jsonSerializerOptions);

        context.ValueProviders.Add(valueProvider);

        return Task.CompletedTask;
    }
}
