# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiamos todo el proyecto
COPY . .

# Restauramos paquetes NuGet
RUN dotnet restore

# Publicamos específicamente el proyecto Api
RUN dotnet publish "Api/Api.csproj" -c Release -o /out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiamos el build publicado
COPY --from=build /out .

# Exponemos el puerto para Railway
EXPOSE 8080

# Arrancamos la app
ENTRYPOINT ["dotnet", "Api.dll"]


