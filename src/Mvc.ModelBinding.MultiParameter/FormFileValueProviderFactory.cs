// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

/// <summary>
/// A <see cref="IValueProviderFactory"/> for <see cref="FormValueProvider"/>.
/// </summary>
public sealed class FormFileValueProviderFactory : IValueProviderFactory
{
    /// <inheritdoc />
    public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var request = context.ActionContext.HttpContext.Request;
        if (request.HasFormContentType)
        {
            // Allocating a Task only when the body is multipart form.
            return AddValueProviderAsync(context, request);
        }

        return Task.CompletedTask;
    }

    private static async Task AddValueProviderAsync(ValueProviderFactoryContext context, HttpRequest request)
    {
        IFormCollection form;

        try
        {
            form = await request.ReadFormAsync();
        }
        catch (InvalidDataException ex)
        {
            // ReadFormAsync can throw InvalidDataException if the form content is malformed.
            // Wrap it in a ValueProviderException that the CompositeValueProvider special cases.
            throw new ValueProviderException(Resources.FormatFailedToReadRequestForm(ex.Message), ex);
        }
        catch (IOException ex)
        {
            // ReadFormAsync can throw IOException if the client disconnects.
            // Wrap it in a ValueProviderException that the CompositeValueProvider special cases.
            throw new ValueProviderException(Resources.FormatFailedToReadRequestForm(ex.Message), ex);
        }

        if (form.Files.Count > 0)
        {
            var valueProvider = new FormFileValueProvider(BindingSource.FormFile, form.Files);
            context.ValueProviders.Add(valueProvider);
        }
    }
}
