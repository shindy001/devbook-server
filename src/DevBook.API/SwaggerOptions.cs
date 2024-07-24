namespace DevBook.API;

public static class SwaggerOptions
{
	public static Action<SwaggerGenOptions> WithDevBookOptions()
	{
		return opt =>
		{
			// Support inheritance and polymorphism
			opt.UseOneOfForPolymorphism();
			opt.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);

			// Swagger Bearer auth
			opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Description = "Please enter token.",
				Scheme = "bearer",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.Http
			});
			opt.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					new List<string>()
				}
			});

			opt.UseAllOfToExtendReferenceSchemas();
		};
	}
}
