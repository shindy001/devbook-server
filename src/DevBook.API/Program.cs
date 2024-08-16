using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var devBookClientOrigin = builder.Configuration.GetSection("DevBookClientOrigins").Get<string[]>()!;
var authTokenTTLInMinutes = builder.Configuration.GetSection("AuthTokenTTLInMinutes").Get<int>()!;
var graphQLIntrospectionAllowed = builder.Configuration.GetSection("GraphQLIntrospectionAllowed").Get<bool>()!;
var devBookCorsPolicyName = "DevBookCorsPolicy";
var featureModuleManager = new FeatureModuleManager();

builder.AddServiceDefaults();
builder.Services.RegisterDevBookDbContext();
builder.Services.RegisterRequestPipelines();
builder.Services.RegisterAuth(tokenTTLinMinutes: authTokenTTLInMinutes, requireConfirmedAccountOnSignIn: false);
featureModuleManager.RegisterFeatureModules(builder.Services, [typeof(Program).Assembly]);

builder.Services.AddSwaggerGen(SwaggerOptions.WithDevBookOptions());
builder.Services.AddEndpointsApiExplorer()
	.ConfigureHttpJsonOptions(opt =>
	{
		opt.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
		opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});

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
	.AllowIntrospection(allow: graphQLIntrospectionAllowed)
	.AddProjections()
	.AddAPITypes()
	.AddQueryConventions()
	.AddMutationConventions();

var app = builder.Build();

// Create DB if not exist or migrate if not up to date
app.InitializeDb(applyMigrations: true);

await featureModuleManager.InitializeModules(app);

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(devBookCorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

// Health check + Alive
app.MapDefaultEndpoints();

featureModuleManager.MapFeatureModulesEndpoints(app);

app.MapGraphQLHttp("/graphql")
	.RequireCors(devBookCorsPolicyName);

app.MapGraphQLSchema("/graphql/schema")
	.RequireCors(devBookCorsPolicyName)
	.RequireAuthorization();

if (app.Environment.IsDevelopment())
{
	app.MapBananaCakePop("/graphql/ui");
}

app.Run();

public partial class Program { }
