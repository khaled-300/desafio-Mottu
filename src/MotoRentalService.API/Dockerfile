# Use the .NET SDK image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Create a non-root user and set environment variables
# RUN id -u app &>/dev/null || adduser --disabled-password --gecos '' app
# USER app
ARG BUILD_CONFIGURATION=Development
ENV IMAGE_ROOT_DIRECTORY="/app/Images"

# Copy the entire solution to the build context
COPY . .

# Check directory contents
RUN ls -la 

# Restore dependencies for the API project
RUN dotnet restore ./MotoRentalService.API

# Build and publish a release
RUN dotnet publish ./MotoRentalService.API -c $BUILD_CONFIGURATION -o ./out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MotoRentalService.API.dll"]
