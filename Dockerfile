FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["RedisCachingWorker/RedisCachingWorker.csproj", "RedisCachingWorker/"]
RUN dotnet restore "RedisCachingWorker/RedisCachingWorker.csproj"
COPY . .
WORKDIR "/src/RedisCachingWorker"
RUN dotnet build "RedisCachingWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RedisCachingWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RedisCachingWorker.dll"]
