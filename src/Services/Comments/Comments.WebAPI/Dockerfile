# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update && apt-get install -y curl
ENV ASPNETCORE_HTTP_PORTS=5002
USER $APP_UID
WORKDIR /app
EXPOSE $ASPNETCORE_HTTP_PORTS
HEALTHCHECK --interval=30s --timeout=30s --start-period=10s --retries=3 CMD curl --fail "http://localhost:${ASPNETCORE_HTTP_PORTS}/health" || exit 1

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./Services/Comments/Comments.WebAPI/Comments.WebAPI.csproj", "Services/Comments/Comments.WebAPI/"]
RUN dotnet restore "./Services/Comments/Comments.WebAPI/Comments.WebAPI.csproj"
COPY . .
WORKDIR /src/Services/Comments/Comments.WebAPI
RUN dotnet build "./Comments.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Comments.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Comments.WebAPI.dll"]