# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
    
# Copy csproj and restore as distinct layers
COPY *.sln .
COPY ./BookFetcher/*csproj ./BookFetcher/
COPY ./WebAPI/*csproj ./WebAPI/
RUN dotnet restore
    
# Copy everything else and build
COPY ./BookFetcher/. ./BookFetcher/
COPY ./WebAPI/. ./WebAPI/

WORKDIR /app/WebAPI
RUN dotnet publish -c Release -o out
    
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/WebAPI/out .
ENTRYPOINT ["dotnet", "SkyrimLibrary.WebAPI.dll"]

