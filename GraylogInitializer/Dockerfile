FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GraylogInitializer.Console/GraylogInitializer.Console.csproj", "GraylogInitializer.Console/"]
RUN dotnet restore "GraylogInitializer.Console/GraylogInitializer.Console.csproj"
COPY . .
WORKDIR "/src/GraylogInitializer.Console"
RUN dotnet build "GraylogInitializer.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GraylogInitializer.Console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GraylogInitializer.Console.dll"]
