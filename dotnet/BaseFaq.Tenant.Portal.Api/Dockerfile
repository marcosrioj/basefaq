FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5002
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnet/BaseFaq.Tenant.Portal.Api/BaseFaq.Tenant.Portal.Api.csproj", "dotnet/BaseFaq.Tenant.Portal.Api/"]
RUN dotnet restore "dotnet/BaseFaq.Tenant.Portal.Api/BaseFaq.Tenant.Portal.Api.csproj"
COPY . .
WORKDIR "/src/dotnet/BaseFaq.Tenant.Portal.Api"
RUN dotnet build "./BaseFaq.Tenant.Portal.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BaseFaq.Tenant.Portal.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BaseFaq.Tenant.Portal.Api.dll"]
