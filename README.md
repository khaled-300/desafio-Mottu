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

Navigate to the project directory: Open your terminal and navigate to `src` folder where the `docker-compose.yml` file resides.

## Build and start the services:

### Run the following command in your terminal:

command Bash:

```sh
docker-compose up -d --build
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

# How to test the API

The MotoRentalService API can be tested using swagger interface or the Postman and other ways of the sending the requests to the API.

- First create your user in the api using the Sample request provided in the api endpoint.
- Login to the api using the user just created before with the email and password.
- Then you are free to use the api, this is extra functionality to authenticate the user based on the role.
- The delivery user is going to be created based on the auth user. After the creation of the delivery user you can go for rent after selecting the rental plan and the motorcycle.

## Generate the token.
copy the token and go to the swagger ui and click on the authorize button 
![image](https://github.com/khaled-300/desafio-Mottu/assets/47085352/d4e4ac84-48dd-48b4-a1a6-616923862bbc)
and follow the next step to authenticate the api.
![image](https://github.com/khaled-300/desafio-Mottu/assets/47085352/1f32c8c0-7692-4dc4-9ca6-929b941eb023)

now API is authenticated and ready to use.
