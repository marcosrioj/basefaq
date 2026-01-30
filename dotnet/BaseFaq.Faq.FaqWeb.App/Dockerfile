FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnet/BaseFaq.Faq.FaqWeb.App/BaseFaq.Faq.FaqWeb.App.csproj", "dotnet/BaseFaq.Faq.FaqWeb.App/"]
RUN dotnet restore "dotnet/BaseFaq.Faq.FaqWeb.App/BaseFaq.Faq.FaqWeb.App.csproj"
COPY . .
WORKDIR "/src/dotnet/BaseFaq.Faq.FaqWeb.App"
RUN dotnet build "./BaseFaq.Faq.FaqWeb.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BaseFaq.Faq.FaqWeb.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BaseFaq.Faq.FaqWeb.App.dll"]
