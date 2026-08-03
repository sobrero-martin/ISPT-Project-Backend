# Etapa 1: Construcción y compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo csproj y restaurar dependencias
COPY ["TuProyecto/TuProyecto.csproj", "TuProyecto/"]
RUN dotnet restore "TuProyecto/TuProyecto.csproj"

# Copiar el resto del código y compilar
COPY . .
WORKDIR "/src/TuProyecto"
RUN dotnet build "TuProyecto.csproj" -c Release -o /app/build

# Etapa 2: Publicación
FROM build AS publish
RUN dotnet publish "TuProyecto.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3: Imagen final para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Configurar el puerto para que Render pueda enrutar el tráfico correctamente
ENV ASPNETCORE_URLS=http://+:8080

COPY --from:publish /app/publish .
ENTRYPOINT ["dotnet", "TuProyecto.dll"]