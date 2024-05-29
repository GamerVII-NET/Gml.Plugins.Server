FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Gml.Plugins.Server/Gml.Plugins.Server.csproj", "src/Gml.Plugins.Server/"]
RUN dotnet restore "src/Gml.Plugins.Server/Gml.Plugins.Server.csproj"
COPY . .
WORKDIR "/src/src/Gml.Plugins.Server"
RUN dotnet build "Gml.Plugins.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gml.Plugins.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gml.Plugins.Server.dll"]
