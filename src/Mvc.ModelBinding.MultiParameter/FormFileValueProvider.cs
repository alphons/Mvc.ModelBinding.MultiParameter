// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable


using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

/// <summary>
/// An <see cref="IValueProvider"/> adapter for data stored in an <see cref="IFormFileCollection"/>.
/// </summary>
/// <remarks>
/// Unlike most <see cref="IValueProvider"/> instances, <see cref="FormFileValueProvider"/> does not provide any values, but
/// specifically responds to <see cref="ContainsPrefix(string)"/> queries. This allows the model binding system to
/// recurse in to deeply nested object graphs with only values for form files.
/// </remarks>
public sealed class FormFileValueProvider : BindingSourceValueProvider
{
    private readonly IFormFileCollection _files;
    private PrefixContainer? _prefixContainer;

    /// <summary>
    /// Creates a value provider for <see cref="IFormFileCollection"/>.
    /// </summary>
    /// <param name="files">The <see cref="IFormFileCollection"/>.</param>
    public FormFileValueProvider(BindingSource bindingsource, IFormFileCollection files) : base(bindingsource)
    {
        _files = files ?? throw new ArgumentNullException(nameof(files));
    }

    private PrefixContainer PrefixContainer
    {
        get
        {
            _prefixContainer ??= CreatePrefixContainer(_files);
            return _prefixContainer;
        }
    }

    private static PrefixContainer CreatePrefixContainer(IFormFileCollection formFiles)
    {
        var fileNames = new List<string>();
        var count = formFiles.Count;
        for (var i = 0; i < count; i++)
        {
            var file = formFiles[i];

            // If there is an <input type="file" ... /> in the form and is left blank.
            // This matches the filtering behavior from FormFileModelBinder
            if (file.Length == 0 && string.IsNullOrEmpty(file.FileName))
            {
                continue;
            }

            fileNames.Add(file.Name);
        }

        return new PrefixContainer(fileNames);
    }

    /// <inheritdoc />
    public override bool ContainsPrefix(string prefix) => PrefixContainer.ContainsPrefix(prefix);

    /// <inheritdoc />
    public override ValueProviderResult GetValue(string key) => ValueProviderResult.None;

    public override object? GetModel(string key, Type t)
    {
        if (_files != null && t == typeof(IFormFile))
           return _files.FirstOrDefault(x => x.Name == key);

        return null;
    }

}
