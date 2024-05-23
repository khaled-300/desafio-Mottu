# Use the .NET SDK image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"
