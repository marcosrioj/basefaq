# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnet/Querify.Tenant.BackOffice.Api/Querify.Tenant.BackOffice.Api.csproj", "dotnet/Querify.Tenant.BackOffice.Api/"]
COPY . .
WORKDIR "/src/dotnet/Querify.Tenant.BackOffice.Api"
RUN --mount=type=cache,id=querify-nuget,target=/root/.nuget/packages,sharing=locked \
    dotnet restore "./Querify.Tenant.BackOffice.Api.csproj" --disable-parallel
RUN --mount=type=cache,id=querify-nuget,target=/root/.nuget/packages,sharing=locked \
    dotnet build "./Querify.Tenant.BackOffice.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN --mount=type=cache,id=querify-nuget,target=/root/.nuget/packages,sharing=locked \
    dotnet publish "./Querify.Tenant.BackOffice.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Querify.Tenant.BackOffice.Api.dll"]
