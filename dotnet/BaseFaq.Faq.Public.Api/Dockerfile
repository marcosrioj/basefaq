FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5020
EXPOSE 5021

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnet/BaseFaq.Faq.Public.Api/BaseFaq.Faq.Public.Api.csproj", "dotnet/BaseFaq.Faq.Public.Api/"]
RUN dotnet restore "dotnet/BaseFaq.Faq.Public.Api/BaseFaq.Faq.Public.Api.csproj"
COPY . .
WORKDIR "/src/dotnet/BaseFaq.Faq.Public.Api"
RUN dotnet build "./BaseFaq.Faq.Public.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BaseFaq.Faq.Public.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BaseFaq.Faq.Public.Api.dll"]
