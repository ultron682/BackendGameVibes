# Zobacz https://aka.ms/customizecontainer, aby dowiedzieć się, jak dostosować kontener debugowania i jak program Visual Studio używa tego pliku Dockerfile do kompilowania obrazów w celu szybszego debugowania.

# Ten etap jest używany podczas uruchamiania z programu VS w trybie szybkim (domyślnie dla konfiguracji debugowania)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
# HEALTHCHECK CMD curl --fail http://localhost:5000/api/health || exitWORKDIR /app
EXPOSE 5556


# Ten etap służy do kompilowania projektu usługi
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# HEALTHCHECK CMD curl --fail http://localhost:5000/api/health || exit
COPY ["BackendGameVibes/BackendGameVibes.csproj", "BackendGameVibes/"]
RUN dotnet restore "./BackendGameVibes/BackendGameVibes.csproj"
COPY . .
WORKDIR "/src/BackendGameVibes"
RUN dotnet build "./BackendGameVibes.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Ten etap służy do publikowania projektu usługi do skopiowania do etapu końcowego
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BackendGameVibes.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Ten etap jest używany w środowisku produkcyjnym lub w przypadku uruchamiania z programu VS w trybie regularnym (domyślnie, gdy nie jest używana konfiguracja debugowania)
FROM base AS final
WORKDIR /app
#COPY "./BackendGameVibes/GameVibesDatabase.db" .
# HEALTHCHECK CMD curl --fail http://localhost:5000/api/health || exit
COPY --from=publish /app/publish .
CMD ["dotnet", "BackendGameVibes.dll"]