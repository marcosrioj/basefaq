FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnet/BaseFaq.Identity.App/BaseFaq.Identity.App.csproj", "dotnet/BaseFaq.Identity.App/"]
RUN dotnet restore "dotnet/BaseFaq.Identity.App/BaseFaq.Identity.App.csproj"
COPY . .
WORKDIR "/src/dotnet/BaseFaq.Identity.App"
RUN dotnet build "./BaseFaq.Identity.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BaseFaq.Identity.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BaseFaq.Identity.App.dll"]
