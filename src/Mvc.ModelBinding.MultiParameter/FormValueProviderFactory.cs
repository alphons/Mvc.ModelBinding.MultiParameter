
// FormValueProviderFactory
// (C) 2022 Alphons van der Heijden
// Date: 2022-04-10
// Version: 1.2

using System.Text.Json;

using Microsoft.AspNetCore.Http;

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Mvc.ModelBinding.MultiParameter;

#nullable enable

/// <summary>
/// A <see cref="IValueProviderFactory"/> for <see cref="GenericValueProvider"/>.
/// </summary>
public class FormValueProviderFactory : IValueProviderFactory
{
	private readonly JsonSerializerOptions? jsonSerializerOptions;

	public FormValueProviderFactory(JsonSerializerOptions? Options) : base()
	{
		this.jsonSerializerOptions = Options;
	}
	public FormValueProviderFactory()
	{
		this.jsonSerializerOptions = null;
	}

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
			// Allocating a Task only when the body is form data.
			return AddValueProviderAsync(context);
		}

		return Task.CompletedTask;
	}

	private async Task AddValueProviderAsync(ValueProviderFactoryContext context)
	{
		var request = context.ActionContext.HttpContext.Request;
		IFormCollection form;

		try
		{
			form = await request.ReadFormAsync();
		}
		catch (InvalidDataException ex)
		{
			// ReadFormAsync can throw InvalidDataException if the form content is malformed.
			// Wrap it in a ValueProviderException that the CompositeValueProvider special cases.
			throw new ValueProviderException(ex.Message, ex);
		}
		catch (IOException ex)
		{
			// ReadFormAsync can throw IOException if the client disconnects.
			// Wrap it in a ValueProviderException that the CompositeValueProvider special cases.
			throw new ValueProviderException(ex.Message, ex);
		}

		var valueProvider = new GenericValueProvider(
			BindingSource.Form,
			null,
			form,
			this.jsonSerializerOptions);

		context.ValueProviders.Add(valueProvider);
	}
}

