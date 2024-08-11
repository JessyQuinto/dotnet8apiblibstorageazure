using System;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using ProductoImagenes.Data;
using ProductoImagenes.Services;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Trace);

var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddDebug();
    config.SetMinimumLevel(LogLevel.Trace);
}).CreateLogger("Program");

try
{
    logger.LogInformation("Iniciando la aplicación");

    // Configuración de Key Vault
    string keyVaultUrl = "https://dotnetfeedback.vault.azure.net/";
    logger.LogInformation($"Configurando Key Vault: {keyVaultUrl}");
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential());

    // Configuración de la base de datos
    logger.LogInformation("Obteniendo cadena de conexión de la base de datos");
    string dbConnectionString = builder.Configuration["productobdfeedback"];
    logger.LogInformation($"Cadena de conexión obtenida: {dbConnectionString}");
    builder.Services.AddDbContext<ProductoDbContext>(options =>
        options.UseSqlServer(dbConnectionString));

    // Configuración del servicio Blob
    logger.LogInformation("Obteniendo cadena de conexión de Blob Storage");
    string blobConnectionString = builder.Configuration["blobstoragefeedback"];
    logger.LogInformation($"Cadena de conexión de Blob obtenida: {blobConnectionString}");
    string containerName = "tu-nombre-de-contenedor"; // Reemplaza con el nombre real de tu contenedor
    builder.Services.AddSingleton<IBlobService>(provider =>
        new BlobService(blobConnectionString, containerName));

    // Configuración de Swagger
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });
    });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    var app = builder.Build();

    // Verificación de conexión a la base de datos
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductoDbContext>();
        logger.LogInformation("Verificando conexión a la base de datos de Azure");
        try
        {
            // Intenta conectarse a la base de datos
            bool canConnect = await dbContext.Database.CanConnectAsync();
            if (canConnect)
            {
                logger.LogInformation("Conexión a la base de datos de Azure establecida exitosamente");
            }
            else
            {
                logger.LogWarning("No se pudo establecer conexión con la base de datos de Azure");
                // Puedes decidir si quieres que la aplicación continúe o no en este punto
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al intentar conectar con la base de datos de Azure");
            // Decide si quieres que la aplicación falle si no puede conectarse a la base de datos
            // throw; // Descomenta esta línea si quieres que la aplicación falle en caso de error de conexión
        }
    }

    // Configuración de Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API V1");
        c.RoutePrefix = "swagger";
    });

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    logger.LogInformation("Aplicación configurada, iniciando...");
    app.Run();
}
catch (Exception ex)
{
    logger.LogError(ex, "Error al iniciar la aplicación");
    throw;
}