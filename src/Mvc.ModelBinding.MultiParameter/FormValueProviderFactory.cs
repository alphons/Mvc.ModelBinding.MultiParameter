
// FormValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Version: 1.2 Date: 2022-04-10
// Version: 1.3 Date: 2024-11-23

using System.Text.Json;

using Microsoft.AspNetCore.Http;

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

/// <summary>
/// A <see cref="IValueProviderFactory"/> for <see cref="GenericValueProvider"/>.
/// </summary>
public class FormValueProviderFactory(JsonSerializerOptions? jsonSerializerOptions) : IValueProviderFactory
{
	/// <inheritdoc />
	public Task CreateValueProviderAsync(ValueProviderFactoryContext? context)
	{
		if (context?.ActionContext?.HttpContext?.Request is { HasFormContentType: true })
		{
			// Allocating a Task only when the body is form data.
			return AddValueProviderAsync(context);
		}

		return Task.CompletedTask;
	}

	private async Task AddValueProviderAsync(ValueProviderFactoryContext context)
	{
		var request = context.ActionContext.HttpContext.Request;

		IFormCollection formCollection;

		try
		{
			formCollection = await request.ReadFormAsync();
		}
		catch (InvalidDataException ex)
		{
			throw new ValueProviderException("Malformed form content detected.", ex);
		}
		catch (IOException ex)
		{
			throw new ValueProviderException("Client disconnected during form reading.", ex);
		}

		context.ValueProviders.Add(new GenericValueProvider(
			BindingSource.Form,
			jsonDocument: null,
			formCollection: formCollection,
			jsonSerializerOptions: jsonSerializerOptions));
	}
}

