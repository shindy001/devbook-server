﻿namespace DevBook.API.Features.BookStore.ProductCategories;

public sealed record ProductCategory()
	: Entity(Guid.NewGuid())
{
	public required string Name { get; init; }
}
