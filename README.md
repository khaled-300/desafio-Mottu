# MotoRentalService - Docker Compose Setup

This project uses Docker Compose to manage its dependencies and run the application in a containerized environment.

## Prerequisites

- Docker: Ensure you have Docker installed on your system. You can download it from the official website: https://www.docker.com/
- Docker Compose: Docker Compose is usually bundled with Docker. Verify its installation by running docker-compose version in your terminal. If not installed, follow the instructions here: https://docs.docker.com/compose/install/
  Building and Running the Application

You can run the commands to know if docker and docker-componse are installed.
```sh
docker --version
docker-compose --version
```

Navigate to the project directory: Open your terminal and navigate to the root directory of your `src` folder where the `docker-compose.yml` file resides.

## Build and start the services:

### Run the following command in your terminal:
command Bash:
```sh
docker-compose up -d
```
This command will:

Build the Docker image for the motorentalservice_api service based on the Dockerfile in the MotoRentalService.API directory.
Start all the defined services in the background (`-d` flag). These services include:
`postgres_rental_db`: A PostgreSQL database container.
`motorentalservice_api`: The MotoRentalService API container.
migration (optional): A container to run database migrations.
Verify application status:

Check if the containers are running:

command Bash:
```sh
docker-compose ps
```
View container logs (if needed):
command Bash:
```sh
docker-compose logs <service_name>
```
Replace `<service_name>` with the specific service name (e.g., `motorentalservice_api`).

## Accessing the Application

The MotoRentalService API will be accessible on port 8080 of your host machine. You can typically access it using http://localhost:8080/swagger/index.html in your web browser.

## Additional Notes

To stop the running services, use `docker-compose down`.
To rebuild the containers with any code changes, run docker-compose up `-d` again.

