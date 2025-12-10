FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 
WORKDIR /src 
COPY ["LoyaltyWorker.csproj", "./"] 
RUN dotnet restore 
COPY . . 
WORKDIR "/src/." 
RUN dotnet build "LoyaltyWorker.csproj" -c Release -o /app/build 

# Stage 2: Publish stage 
FROM build AS publish 
RUN dotnet publish "LoyaltyWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false 

# Stage 3: Final runtime image 
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final 
WORKDIR /app 
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "LoyaltyWorker.dll"]