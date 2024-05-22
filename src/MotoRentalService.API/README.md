# MotoRentalService.API Docker Setup

This guide explains how to build and run the `MotoRentalService.API` using Docker and Docker Compose. It includes steps to build the Docker image for the API and run it with PostgreSQL as a service.

## Prerequisites

- Docker
- Docker Compose

## Building the Docker Image

1. Open a terminal and navigate to the `MotoRentalService.API` directory.

2. Build the Docker image with the following command:

   ```sh
   docker build -t moto_rental_service:latest .
   ```

   This command will use the `Dockerfile` in the current directory to build the image and tag it as `moto_rental_service:latest`.

## Running the Services with Docker Compose

1. Ensure you have a `docker-compose.yml` file in the `MotoRentalService.API` directory.

2. Run the services using Docker Compose with the following command:

   ```sh
   docker-compose up -d
   ```

   This command will start both the `MotoRentalService.API` and `Postgres` containers on the same network, allowing them to communicate with each other.

3. To stop the services, use the following command:

   ```sh
   docker-compose down
   ```

## Notes

- The `moto_rental_service` container will use the environment variables to connect to the `postgres_db` container.

## Troubleshooting

- If you encounter any issues with building the image or running the containers, check the Docker logs for more information:

  ```sh
  docker logs moto_rental_service
  docker logs postgres_db
  ```

- Ensure that the Docker daemon is running and you have sufficient permissions to execute Docker commands.
