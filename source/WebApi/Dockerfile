FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["source/WebApi/FSP.WebApi.csproj", "source/WebApi/"]
COPY ["source/Application/FSP.Application.csproj", "source/Application/"]
COPY ["source/Domain/FSP.Domain.csproj", "source/Domain/"]
COPY ["source/Infrastructure/FSP.Infrastructure.csproj", "source/Infrastructure/"]
RUN dotnet restore "source/WebApi/FSP.WebApi.csproj"
COPY . .
WORKDIR "/src/source/WebApi"
RUN dotnet build "./FSP.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FSP.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FSP.WebApi.dll"]
