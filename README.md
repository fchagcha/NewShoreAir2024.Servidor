# Calculadora de Rutas de Viaje

Esta es una solución que permite calcular rutas de viaje entre diferentes destinos alrededor del mundo. La solución recibe como parámetros el origen y el destino del viaje del usuario. A continuación, el sistema consulta todos los vuelos asociados disponibles y determina si es posible encontrar una ruta de viaje entre los dos destinos especificados. En caso de que la ruta sea posible, se devuelve la ruta de viaje al usuario; de lo contrario, se muestra un mensaje indicando que la ruta no puede ser calculada.

## Características Principales

- Utiliza **.NET 8** como tecnología principal.
- Se integra con el **API** https://recruiting-api.newshore.es/api/flights para obtener información sobre vuelos.
- Implementa una **arquitectura limpia** para garantizar la modularidad y mantenibilidad del código.
- Se implemneta también Interceptors, Behaviours y Middlewares.
- Base de datos SqlLite hospedad en la misma solución.
- La aplicación ejecuta la página Swagger
- En el archivo appsettings.json del Api, se puede configurar:
