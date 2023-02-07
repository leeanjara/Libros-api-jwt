# Libros-api-jwt
api hecha con el propósito de aprender JWT
Casi todo el código, excepto el controller de Autorización y las configuraciones fue generado por visual studio
## Requerimientos 
  - Visual Studio 2022
  - Alguna instancia de SqlServer funcionando
  
## Pasos para ejecutar

- Abrir la solución desde visual studio 2022
- Compilar
- Reemplazar la connection string en el archivo app.settings.json por "Data Source=server;Initial Catalog=Libros;Integrated Security=True;" reemplazando "server"
- Abrir consola de nugget y ejecutar:
    > update-database
- Ajustar las configuraciones JwtConfig.Audience (podría ser algo como "https://LocalHost") y JwtConfig.SecretKey (debe ser un string de al menos 32 caracteres)
