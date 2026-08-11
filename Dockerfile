# syntax=docker/dockerfile:1

# ---- Stage 1: build the Angular app ----
FROM node:22-alpine AS build-web
WORKDIR /web
# Install deps first (better layer caching)
COPY frontend/package*.json ./
RUN npm ci
COPY frontend/ ./
RUN npm run build -- --configuration production
# Angular output lands in dist/pdr-web/browser

# ---- Stage 2: build & publish the .NET API ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-api
WORKDIR /src
# Restore first (better layer caching)
COPY backend/Pdr.Api/Pdr.Api.csproj backend/Pdr.Api/
RUN dotnet restore backend/Pdr.Api/Pdr.Api.csproj
COPY backend/ backend/
RUN dotnet publish backend/Pdr.Api/Pdr.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
# Drop the built Angular app into wwwroot so .NET serves it as static files
COPY --from=build-web /web/dist/pdr-web/browser/ /app/publish/wwwroot/

# ---- Stage 3: runtime image ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build-api /app/publish ./
# Kestrel listens on 8080 inside the container
ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80
ENTRYPOINT ["dotnet", "Pdr.Api.dll"]
