using Microsoft.OpenApi.Any;

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

			opt.UseAllOfToExtendReferenceSchemas();

			opt.SchemaFilter<EnumSchemaFilter>();

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
		};
	}

	/// <summary>
	/// Converts enum int values in swagger UI parameters to string values
	/// </summary>
	public class EnumSchemaFilter : ISchemaFilter
	{
		public void Apply(OpenApiSchema model, SchemaFilterContext context)
		{
			if (context.Type.IsEnum)
			{
				model.Enum.Clear();
				Enum.GetNames(context.Type)
					.ToList()
					.ForEach(n =>
					{
						model.Enum.Add(new OpenApiString(n));
						model.Type = "string";
						model.Format = null;
					});
			}
		}
	}
}
