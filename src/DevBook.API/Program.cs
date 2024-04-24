var builder = WebApplication.CreateBuilder(args);

var devBookClientOrigin = builder.Configuration.GetSection("DevBookClientOrigins").Get<string[]>()!;
var authTokenTTLInMinutes = builder.Configuration.GetSection("AuthTokenTTLInMinutes").Get<int>()!;
var devBookCorsPolicyName = "DevBookCorsPolicy";

builder.AddServiceDefaults();
builder.Services.RegisterDevBookDbContext();
builder.Services.RegisterRequestPipelines();
builder.Services.RegisterAuthentication(tokenTTLinMinutes: authTokenTTLInMinutes);
builder.Services.RegisterFeatureModules([typeof(Program).Assembly]);

builder.Services.AddSwaggerGen(SwaggerOptions.WithDevBookOptions());
builder.Services.AddEndpointsApiExplorer()
	.ConfigureHttpJsonOptions(opt
		=> opt.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull);

builder.Services.AddCors(options =>
{
	options.AddPolicy(
		devBookCorsPolicyName, 
		p => p.WithOrigins(devBookClientOrigin)
			.AllowAnyMethod()
			.SetIsOriginAllowed(isAllowed => true)
			.AllowAnyHeader()
			.AllowCredentials());
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services
	.AddGraphQLServer()
	.AddAuthorization()
	.AddProjections()
	.AddAPITypes()
	.AddQueryConventions()
	.AddMutationConventions();

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

app.UseCors(devBookCorsPolicyName);
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

app.MapGraphQLHttp("/graphql")
	.RequireAuthorization()
	.RequireCors(devBookCorsPolicyName);

app.MapGraphQLSchema("/graphql/schema")
	.RequireAuthorization()
	.RequireCors(devBookCorsPolicyName);

if (app.Environment.IsDevelopment())
{
	app.MapBananaCakePop("/graphql/ui");
}

app.Run();
