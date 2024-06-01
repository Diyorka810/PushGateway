FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore ./PushGateway.Services
# Build and publish a release
RUN dotnet publish ./PushGateway.Services/PushGateway.Services/PushGateway.Services.csproj --no-restore -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 8080

ENTRYPOINT ["dotnet", "PushGateway.Services.dll"]