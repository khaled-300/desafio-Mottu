# Use the .NET SDK image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN ls -la
# CMD ["dotnet", "ef", "database", "update", "--project", "MotoRentalService.infrastructure", "--startup-project", "MotoRentalService.API"]
CMD ["tail", "-f", "/dev/null"]