FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5010
EXPOSE 5011

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnet/BaseFaq.Faq.Portal.Api/BaseFaq.Faq.Portal.Api.csproj", "dotnet/BaseFaq.Faq.Portal.Api/"]
RUN dotnet restore "dotnet/BaseFaq.Faq.Portal.Api/BaseFaq.Faq.Portal.Api.csproj"
COPY . .
WORKDIR "/src/dotnet/BaseFaq.Faq.Portal.Api"
RUN dotnet build "./BaseFaq.Faq.Portal.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BaseFaq.Faq.Portal.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BaseFaq.Faq.Portal.Api.dll"]
