
// QueryStringValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-10
// Version: 1.2

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

#nullable enable

/// <summary>
/// A <see cref="IValueProviderFactory"/> that creates <see cref="GenericValueProvider"/> instances that
/// read values from the request query-string.
/// </summary>
public class QueryStringValueProviderFactory : IValueProviderFactory
{
    private readonly JsonSerializerOptions? jsonSerializerOptions;
    public QueryStringValueProviderFactory(JsonSerializerOptions Options) : base()
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
        if (context == null)
        {
            throw new ArgumentNullException(nameof(ValueProviderFactoryContext));
        }

        var query = context.ActionContext.HttpContext.Request.Query;
        if (query != null && query.Count > 0)
        {
            var list = query.Select(x => $"\"{x.Key}\": \"{x.Value}\"").ToArray();
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
