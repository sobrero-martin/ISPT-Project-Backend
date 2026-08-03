# Etapa 1: Restaurar y compilar la solución con .NET 10
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copiar primero los archivos de proyecto (.csproj) manteniendo su estructura de carpetas
COPY ["BD/BD.csproj", "BD/"]
COPY ["DTO/DTO.csproj", "DTO/"]
COPY ["Repositorio/Repositorio.csproj", "Repositorio/"]
COPY ["ISPT-Project-Backend.Server/ISPT-Project-Backend.Server.csproj", "ISPT-Project-Backend.Server/"]

# 2. Restaurar dependencias de toda la solución
RUN dotnet restore "ISPT-Project-Backend.Server/ISPT-Project-Backend.Server.csproj"

# 3. Copiar todo el código fuente restante
COPY . .

# 4. Compilar y publicar el proyecto principal (la API)
WORKDIR "/src/ISPT-Project-Backend.Server"
RUN dotnet publish "ISPT-Project-Backend.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Imagen final optimizada para ejecución con el runtime de .NET 10
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ISPT-Project-Backend.Server.dll"]