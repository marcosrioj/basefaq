FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnet/BaseFaq.Faq.FaqWeb.Test.IntegrationTests/BaseFaq.Faq.FaqWeb.Test.IntegrationTests.csproj", "dotnet/BaseFaq.Faq.FaqWeb.Test.IntegrationTests/"]
RUN dotnet restore "dotnet/BaseFaq.Faq.FaqWeb.Test.IntegrationTests/BaseFaq.Faq.FaqWeb.Test.IntegrationTests.csproj"
COPY . .
WORKDIR "/src/dotnet/BaseFaq.Faq.FaqWeb.Test.IntegrationTests"
RUN dotnet build "./BaseFaq.Faq.FaqWeb.Test.IntegrationTests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BaseFaq.Faq.FaqWeb.Test.IntegrationTests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BaseFaq.Faq.FaqWeb.Test.IntegrationTests.dll"]
