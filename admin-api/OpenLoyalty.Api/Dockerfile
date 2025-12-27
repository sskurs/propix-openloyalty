# Stage 1: Build the .NET application
# CORRECTED: Use the .NET 9.0 SDK to match your project's target framework
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /src

# Copy all source code from the build context (the 'backend-dotnet' directory)
COPY . .

# Restore dependencies for the API project.
RUN dotnet restore "OpenLoyalty.Api/OpenLoyalty.Api.csproj"

# Publish the application, creating a release build in the /app/publish directory.
RUN dotnet publish "OpenLoyalty.Api/OpenLoyalty.Api.csproj" -c Release -o /app/publish

# --- 

# Stage 2: Create the final, lightweight production image
# CORRECTED: Use the .NET 9.0 runtime to match the build stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy the compiled output from the build-env stage into the final image.
COPY --from=build-env /app/publish .

# The entry point for the container. This command runs your .NET application.
ENTRYPOINT ["dotnet", "OpenLoyalty.Api.dll"]
