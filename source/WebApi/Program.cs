using FSP.Api.Infrastructure.Data.DbContexts;
using FSP.Api.WebApi.Configurations;
using FSP.Api.Domain.Constants;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);
builder.Services.AddMemoryCache();

// Configure authorization policies based on claims
builder.Services.AddAuthorization(options =>
{
    // Anexos
    options.AddPolicy("AttachmentsRead", policy =>
        policy.RequireClaim("Permission", Permissions.AttachmentsRead));
    
    options.AddPolicy("AttachmentsWrite", policy =>
        policy.RequireClaim("Permission", Permissions.AttachmentsWrite));

    // Empresas
    options.AddPolicy("CompaniesWrite", policy =>
        policy.RequireClaim("Permission", Permissions.CompaniesWrite));

    // Logs
    options.AddPolicy(Permissions.LogsRead, policy =>
        policy.RequireClaim("Permission", Permissions.LogsRead));

    // Permissões
    options.AddPolicy("PermissionsWrite", policy =>
        policy.RequireClaim("Permission", Permissions.PermissionsWrite));

    // Títulos
    options.AddPolicy("TitlesRead", policy =>
        policy.RequireClaim("Permission", Permissions.TitlesRead));
    
    options.AddPolicy("TitlesValidation", policy =>
        policy.RequireClaim("Permission", Permissions.TitlesValidation));
    
    options.AddPolicy("TitlesApproval", policy =>
        policy.RequireClaim("Permission", Permissions.TitlesApproval));

    // Observações
    options.AddPolicy("ObservationsRead", policy =>
        policy.RequireClaim("Permission", Permissions.ObservationsRead));
    
    options.AddPolicy("ObservationsWrite", policy =>
        policy.RequireClaim("Permission", Permissions.ObservationsWrite));

    // Usuários
    options.AddPolicy("UsersWrite", policy =>
        policy.RequireClaim("Permission", Permissions.UsersWrite));

    // Publish
    options.AddPolicy("CanPublish", policy =>
        policy.RequireClaim("canPublish", "true"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
await app.InitialiseDatabaseAsync();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwaggerConfiguration();

app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/swagger"));

app.Run();

public partial class Program { }
