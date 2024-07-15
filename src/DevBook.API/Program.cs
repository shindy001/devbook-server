using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var devBookClientOrigin = builder.Configuration.GetSection("DevBookClientOrigins").Get<string[]>()!;
var authTokenTTLInMinutes = builder.Configuration.GetSection("AuthTokenTTLInMinutes").Get<int>()!;
var graphQLIntrospectionAllowed = builder.Configuration.GetSection("GraphQLIntrospectionAllowed").Get<bool>()!;
var defaultUsers = builder.Configuration.GetSection("DefaultUsers").Get<UserDbSeed[]>();
var devBookCorsPolicyName = "DevBookCorsPolicy";
var featureModuleRegister = new FeatureModuleRegister();

builder.AddServiceDefaults();
builder.Services.RegisterDevBookDbContext();
builder.Services.RegisterRequestPipelines();
builder.Services.RegisterAuth(tokenTTLinMinutes: authTokenTTLInMinutes, requireConfirmedAccountOnSignIn: false);
featureModuleRegister.RegisterFeatureModules(builder.Services, [typeof(Program).Assembly]);

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

// Seed roles to DB if not defined
await app.SeedRoles(
	DevBookUserRoles.Admin,
	DevBookUserRoles.User);

// Seed default users from appsettings to DB
if (defaultUsers is not null && defaultUsers.Any())
{
	await app.SeedUsers(defaultUsers);
}

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

app.MapGroup("/identity")
	.MapIdentityApi<DevBookUser>()
	.WithTags("Identity");

featureModuleRegister.MapFeatureModulesEndpoints(app);

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
