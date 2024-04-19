var builder = WebApplication.CreateBuilder(args);

var DevBookClientOrigin = builder.Configuration.GetValue<string>("DevBookClientOrigin")!;

builder.AddServiceDefaults();
builder.Services.RegisterDB();
builder.Services.RegisterRequestPipelines();
builder.Services.RegisterAuthentication();
builder.Services.RegisterFeatureModules([typeof(Program).Assembly]);

builder.Services.AddSwaggerGen(SwaggerOptions.WithDevBookOptions());
builder.Services.AddEndpointsApiExplorer()
	.ConfigureHttpJsonOptions(opt
		=> opt.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull);

builder.Services.AddCors(options =>
{
	options.AddPolicy("DevBookClient", 
		p => p.WithOrigins(DevBookClientOrigin)
		.AllowAnyMethod()
		.SetIsOriginAllowed(isAllowed => true)
		.AllowAnyHeader()
		.AllowCredentials());
});

builder.Services
	.AddGraphQLServer()
	.AddAuthorization()
	.AddAPITypes();

var app = builder.Build();

// Create DB if not exist or migrate if not up to date
app.InitializeDb(applyMigrations: true);

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();

	app.UseSwagger();
	app.UseSwaggerUI(opt =>
	{
		opt.InjectStylesheet("/swagger-ui/SwaggerDark.css");
	});
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseCors("DevBookClient");
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Health check + Alive
app.MapDefaultEndpoints();

app.MapGroup("/identity")
	.MapIdentityApi<DevBookUser>()
	.WithTags($"Identity");

app.MapFeatureModulesEndpoints();

app.MapGraphQL()
	.RequireAuthorization();

app.Run();
