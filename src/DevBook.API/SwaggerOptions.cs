namespace DevBook.API;

public static class SwaggerOptions
{
	public static Action<SwaggerGenOptions> WithDevBookOptions()
	{
		return opt =>
		{
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

			opt.OperationFilter<SwaggerIdentityOperationIdFilter>();
			opt.UseAllOfToExtendReferenceSchemas();
		};
	}

	/// <summary>
	/// Assignes missing OperationIds to .net8 Identity actions added with MapIdentityApi, endpoints should be grouped as 'identity'
	/// Also acts as workaround for swaggergen/nswag connected service conflict of .net 8 2fa action which is by default generated as 2fa function which is not valid c# name
	/// <see href="https://github.com/dotnet/aspnetcore/issues/49720" />
	/// </summary>
	private class SwaggerIdentityOperationIdFilter : IOperationFilter
	{
		private const string OperationIdPrefix = "Identity.";

		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			// Check if it's a Identity action
			if (context.ApiDescription.RelativePath?.StartsWith("identity", StringComparison.OrdinalIgnoreCase) == true)
			{
				var relativePath = context.ApiDescription.RelativePath ?? string.Empty;
				operation.OperationId = $"{OperationIdPrefix}{GenerateOperationIdBaseString(relativePath)}";
				return;
			}
		}

		private static string GenerateOperationIdBaseString(string inputString)
		{
			var input = inputString.Split('/');
			if (input.Length == 0)
			{
				return string.Empty;
			}

			return string.Join(string.Empty, input.Skip(1).Select(CapitalizeFirstLetter));
		}

		private static string CapitalizeFirstLetter(string input)
		{
			return string.Concat(input.First().ToString().ToUpper(), input.AsSpan(1));
		}
	}
}
