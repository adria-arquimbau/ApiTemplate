# Voxel.ApiTemplate

Este proyecto contiene un template para crear APIs en el equipo de procurement. No es, ni mucho menos, un trabajo finalizado y es simplemente una primera versión para iniciar una discusión. Tampoco pretende ser, cuando llegue a su estado final, un template de obligado uso, sinó que se espera que sea una guia de referencia de buenas prácticas.

## Partes de la solución

La solución está desarrollada como se desarrollan en general las APIs en Voxel: una api pública que hace llamadas HTTP a una api privada que es la que tiene toda la lógica de negocio. Esta manera de organizar las APIs es, obviamente, también discutible ya que añade una latencia a cambio de un poco más de seguridad. 

La API pública es muy sencilla y no tiene ninguna organización por dominio o capas. Los controladores llaman directamente a los endpoints de la API privada.

La API privada si que tiene una mayor organización. En este caso se usa MediatR para implementar los servicios de aplicación y son estos quien llaman a repositorios o, en caso necesario, a servicios de dominio. Se ha intentado que la capa de dominio quede libre de referencias a proyectos de infrastructura, siendo sólo MediatR y el logging las excepciones.

Ambos proyectos utilizan Serilog como framework de logging, logueando a un fichero y a graylog. Se ha implementado un middleware que añade a todos los logs todas aquella información de correlación que se reciba. Ver el punto de Distrubuted Tracing.

Para la parte de comunicación entre la API interna y la externa, no sé ha utilizado ningúna librería para crear el cliente, sinó que se han utilizado los clientes HTTP tipados que ASP .NET core provee. También se ha utilizado Polly para realizar reintentos en caso de un error transient.

Se ha separado la parte de las apis de la parte del hosteo de las apis, para así favorecer el testing. Se puede ver una explicación de todo esto [aquí](https://github.com/Xabaril/ManualEffectiveTestingHttpAPI). Se han implementado tests de integración/aceptación contra una base de datos en memoria utilizando el TestServer. Para los tests se inyecta una base de datos diferente a la de la aplicación (ambas inMemory). La implementación se ha basado en lo explicado [aquí](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1)

Se ha organizado el código en dos carpetas, src y test, porque creemos que queda más entendible.

Ambas apis tienen un healthcheck accesible en `/health`. El healthcheck de la api externa tiene en cuenta la salud de la api interna.

Ambas apis implementan la especificación Problem Details para los errores. Si la API interna devuelve un objeto ProblemDetails, este objeto es el que se envia al cliente para que así tenga el máximo de información.

Ambas apis implementan versionado. Por ahora el versionado se hace por URL, por ser el mecanismo que se integra más facilmente a .Net Core utlizando `Microsoft.AspNetCore.Mvc.Versioning`.

Ambas apis tienen Swagger.

## Distributed tracing
Al ser una aplicación ASP.NET Core 3.1, el tracing distribuido ya viene de serie e incluso se loguea sin hacer nada. Si ejecutamos la aplicación y vamos a Graylog, veremos que todos los logs de la misma request (los de la WebApi y los de la Api) tienen la misma TraceId. Para setear esta TraceId desde el exterior hay que pasar una header con el nombre `traceparent`.

Si necesitamos pasar otras variables hay que pasar la header [Correlation-Context](https://github.com/w3c/correlation-context/blob/master/correlation_context/HTTP_HEADER_FORMAT.md). Un ejemplo podria ser: `Correlation-Context: userId=sergey,serverNode=DF,isProduction=false`. Con el middleware de logging, estas variables se añadirian a nuestros logs.


## Librerias utilizadas
 - Logging
   - Serilog
   - Serilog.AspNetCore
   - Serilog.Extensions.Logging
   - Serilog.Sinks.Console
   - Serilog.Sinks.File
   - Serilog.Sinks.Graylog
   
 - Reliability
   - Polly
   - Polly.Extensions.HTTP
   
 - Data
   - Microsoft.EntityFrameworkCore
   - Microsoft.EntityFrameworkCore.InMemory
   
 - Testing
   - XUnit
   - AutoFixture.XUnit2
   - FakeItEasy
   - FluentAssertions
   - XBehave
   - Microsoft.AspNetCore.Mvc.Testing
   - Microsoft.AspNetCore.TestHost
   - Respawn
   
 - CrossCutting
   - MediatR
   - FluentValidator
   - Scrutor
   - Newtonsoft.Json
   - Optional
   
 - API Related
   - Hellang.Middleware.ProblemDetails
   - Microsoft.AspNetCore.Mvc.Versioning
   - Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer
   - Swashbuckle.AspNetCore
   - Swashbuckle.AspNetCore.SwaggerGen
   

## Qué falta

Como se ha dicho, la solución no está completa. Faltarían cosas tales como:
 - Autenticación y autorización (y sus correspondientes tests), tanto con usuario/password como con api-key.
 - Utilizar Polly en los accesos a la base de datos
 - Y seguramente un largo etc..


## Ejemplos de llamadas

### Get a value item
```
curl --location --request GET 'http://localhost:58857/api/v1.0/values/asdf' \
--header 'traceparent: sometraceid' \
--header 'Correlation-Context: userId=sergey,serverNode=DF:28,isProduction=false'
```

**Response**
```
{
    "key": "asdf",
    "value": 42
}
```

### Post a value item
*Throws a not implemented exception*
```
curl --location --request POST 'http://localhost:58857/api/v1.0/values' \
--header 'traceparent: sometraceid' \
--header 'Correlation-Context: userId=sergey,serverNode=DF:28,isProduction=false' \
--header 'Content-Type: application/json' \
--data-raw '{"key": "asdf2", "value": 5}'
```

**Response** *(ProblemDetails)*
```
{
    "type": "https://httpstatuses.com/501",
    "title": "Not Implemented",
    "status": 501,
    "detail": "The post method has not been implemented yet."
}
```

### Put a value item
*Throws a custom exception*
```
curl --location --request PUT 'http://localhost:58857/api/v1.0/values/asdfasdfasdf' \
--header 'traceparent: sometraceid' \
--header 'Correlation-Context: userId=sergey,serverNode=DF:28,isProduction=false' \
--header 'Content-Type: application/json' \
--data-raw '{"key": "asdfasdfasdf", "value": 5}'
```

**Response** *(ProblemDetails)*
```
{
    "type": "key-too-long",
    "title": "The key is too long.",
    "status": 403,
    "detail": "asdfasdfasdf"
}
```

### Generate a new migration

To generate a new migration we use the following command:

dotnet ef migrations add nameOfMigration -s ../Voxel.ApiTemplate.Values.Api

This creates a new migration with the name nameOfMigration. The -s points to the place the dbContext is instanced.

### Update a database with migrations

To update a database we use the following command

dotnet ef database update -s ../Voxel.ApiTemplate.Values.Api

This updates the database the startup is currently pointing to. It will update the database to the last migration available