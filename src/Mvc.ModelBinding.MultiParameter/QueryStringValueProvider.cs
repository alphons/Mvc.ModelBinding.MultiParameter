// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

/// <summary>
/// An <see cref="IValueProvider"/> adapter for data stored in an <see cref="IQueryCollection"/>.
/// </summary>
public class QueryStringValueProvider : BindingSourceValueProvider, IEnumerableValueProvider
{
    private readonly IQueryCollection _values;
    private PrefixContainer? _prefixContainer;

    /// <summary>
    /// Creates a value provider for <see cref="IQueryCollection"/>.
    /// </summary>
    /// <param name="bindingSource">The <see cref="BindingSource"/> for the data.</param>
    /// <param name="values">The key value pairs to wrap.</param>
    /// <param name="culture">The culture to return with ValueProviderResult instances.</param>
    public QueryStringValueProvider(
        BindingSource bindingSource,
        IQueryCollection values,
        CultureInfo? culture)
        : base(bindingSource)
    {
        if (bindingSource == null)
        {
            throw new ArgumentNullException(nameof(bindingSource));
        }

        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        _values = values;
        Culture = culture;
    }

    /// <summary>
    /// The culture for the provider.
    /// </summary>
    public CultureInfo? Culture { get; }

    /// <summary>
    /// The <see cref="PrefixContainer"/>.
    /// </summary>
    protected PrefixContainer PrefixContainer
    {
        get
        {
            if (_prefixContainer == null)
            {
                _prefixContainer = new PrefixContainer(_values.Keys);
            }

            return _prefixContainer;
        }
    }

    /// <inheritdoc />
    public override bool ContainsPrefix(string prefix)
    {
        return PrefixContainer.ContainsPrefix(prefix);
    }

    /// <inheritdoc />
    public virtual IDictionary<string, string> GetKeysFromPrefix(string prefix)
    {
        if (prefix == null)
        {
            throw new ArgumentNullException(nameof(prefix));
        }

        return PrefixContainer.GetKeysFromPrefix(prefix);
    }

    /// <inheritdoc />
    public override ValueProviderResult GetValue(string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (key.Length == 0)
        {
            // Top level parameters will fall back to an empty prefix when the parameter name does not
            // appear in any value provider. This would result in the parameter binding to a query string
            // parameter with a empty key (e.g. /User?=test) which isn't a scenario we want to support.
            // Return a "None" result in this event.
            return ValueProviderResult.None;
        }

        var values = _values[key];
        if (values.Count == 0)
        {
            return ValueProviderResult.None;
        }
        else
        {
            return new ValueProviderResult(values, Culture);
        }
    }

    public override object? GetModel(string key, Type t)
    {
        var val = GetValue(key).FirstOrDefault();
        if (val == null)
            return null;

        var model = System.ComponentModel.TypeDescriptor.GetConverter(t).ConvertFrom(
                   context: null,
                   culture: CultureInfo.InvariantCulture,
                   value: val); // Needs some tweaking
        return model;
    }
}
