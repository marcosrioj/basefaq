FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5040
EXPOSE 5041

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnet/BaseFaq.AI.Matching.Api/BaseFaq.AI.Matching.Api.csproj", "dotnet/BaseFaq.AI.Matching.Api/"]
RUN dotnet restore "dotnet/BaseFaq.AI.Matching.Api/BaseFaq.AI.Matching.Api.csproj"
COPY . .
WORKDIR "/src/dotnet/BaseFaq.AI.Matching.Api"
RUN dotnet build "./BaseFaq.AI.Matching.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BaseFaq.AI.Matching.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BaseFaq.AI.Matching.Api.dll"]
