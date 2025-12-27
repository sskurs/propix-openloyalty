# Stage 1: Build the .NET application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /src

# Copy the project file and restore dependencies
COPY OpenLoyalty.Customer.Api.csproj ./
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# ---

# Stage 2: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy the output from the build stage
COPY --from=build-env /app/publish .

# The entry point for the container
ENTRYPOINT ["dotnet", "OpenLoyalty.Customer.Api.dll"]
