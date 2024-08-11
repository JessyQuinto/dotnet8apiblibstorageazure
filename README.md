# dotnet8apiblibstorageazure

# ProductoImagenes Backend

Backend API para la aplicación ProductoImagenes, que permite gestionar archivos en Azure Blob Storage con metadatos en Azure SQL Database.

## Características

- Subir archivos a Azure Blob Storage
- Listar archivos almacenados
- Descargar archivos
- Actualizar archivos existentes
- Eliminar archivos
- Integración con Azure Key Vault para gestión segura de secretos

## Tecnologías Utilizadas

- ASP.NET Core 8.0
- Azure SQL Database
- Azure Blob Storage
- Azure Key Vault

## Prerrequisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- Una cuenta de Azure con servicios de SQL Database, Blob Storage, y Key Vault configurados

## Configuración

1. Clone el repositorio:
   ```
   git clone https://github.com/tu-usuario/ProductoImagenes.git
   cd ProductoImagenes
   ```

2. Actualice la configuración en `appsettings.json` con sus valores de Azure:
   ```json
   {
     "KeyVault": {
       "Vault": "https://your-keyvault.vault.azure.net/"
     }
   }
   ```

3. Asegúrese de que su aplicación tenga acceso a Azure Key Vault y que los siguientes secretos estén configurados:
   - `productobdfeedback`: Cadena de conexión de Azure SQL Database
   - `blobstoragefeedback`: Cadena de conexión de Azure Blob Storage

## Ejecución

Para ejecutar la aplicación localmente:

```
dotnet run
```

La API estará disponible en `https://localhost:5001`.

## Endpoints de la API

- `GET /api/productos`: Listar todos los productos
- `GET /api/productos/{id}`: Obtener un producto específico
- `POST /api/productos/upload`: Subir un nuevo archivo
- `PUT /api/productos/{id}`: Actualizar un archivo existente
- `DELETE /api/productos/{id}`: Eliminar un archivo

## Configuración de CORS

El backend está configurado para permitir solicitudes CORS desde `http://localhost:4200`. Si necesita cambiar esto, modifique la política CORS en `Program.cs`.

## Contribuir

Las contribuciones son bienvenidas. Por favor, abra un issue para discutir cambios mayores antes de crear un pull request.

## Licencia

[MIT](https://choosealicense.com/licenses/mit/)
