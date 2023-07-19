# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
    
# Copy csproj and restore as distinct layers
COPY *.sln .
COPY ./WebAPI/*csproj ./WebAPI/
COPY ./lib/ReindexerSharp/ReindexerSharp/*csproj ./lib/ReindexerSharp/ReindexerSharp/
RUN dotnet restore
    
# Copy everything else and build
COPY ./lib/. ./lib/
COPY ./WebAPI/. ./WebAPI/

WORKDIR /app/WebAPI
RUN dotnet publish -c Release -o out
    
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/WebAPI/out .
ENTRYPOINT ["dotnet", "SkyrimLibrary.WebAPI.dll"]

