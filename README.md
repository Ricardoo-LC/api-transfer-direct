# API Transfer Direct

## Requisitos previos

Requisitos:

- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)
- [Git](https://git-scm.com/)

## Instalación y configuración

Sigue los pasos a continuación para configurar y ejecutar el proyecto:

1. Clona este repositorio en tu máquina local:

   ```bash
   git clone https://github.com/Ricardoo-LC/api-transfer-direct.git
   ```

2. Accede al directorio del proyecto:

   ```bash
   cd api-transfer-direct
   ```

3. Construye y ejecuta los contenedores Docker:

   ```bash
   docker-compose up --build
   ```

4. Aplica las migraciones iniciales para configurar la base de datos. Esto debe realizarse dentro del contenedor que ejecuta la aplicación. 

   ```bash
   docker exec -it <nombre-contenedor> bash
   ```

   Una vez dentro del contenedor, aplica las migraciones:

   ```bash
   dotnet ef database update
   ```

5. La API estará disponible en `http://localhost:5000`.

## Ejecución de pruebas unitarias

Para ejecutar las pruebas unitarias:

1. Dentro del contenedor, ejecuta el siguiente comando:

   ```bash
   dotnet test
   ```
