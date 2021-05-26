FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CoffeeShopClient/CoffeeShopClient.csproj", "CoffeeShopClient/"]
RUN dotnet restore "CoffeeShopClient/CoffeeShopClient.csproj"
COPY . .
WORKDIR "/src/CoffeeShopClient"
RUN dotnet build "CoffeeShopClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoffeeShopClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoffeeShopClient.dll"]
