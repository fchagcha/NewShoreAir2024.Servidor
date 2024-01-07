Este proyecto está construido con .Net 8 usuando VS2022 , aplicando arquitectura orientada al dominio usando la libreria Fabrela.Domain.Core de mi autoria

1.- Compile la aplicación para verificar los paquetes Nuget.

2.- Ejecute la aplicación con el perfil NewShoreAir.Api. La aplicación se ejecutará como autohospedada

3.- Base de datos SqlLite hospedad en la misma solución.

4.- La aplicación ejecuta la página Swagger

5.- En el archivo appsettings.json del Api, se puede configurar:

	- direccion para consumir api, actualmente lo hace con Rutas múltiples y de retorno, maxima dificultad
	
	- configurar el tiempo de vida de cache, para evitar el consumo repetitivo del API
