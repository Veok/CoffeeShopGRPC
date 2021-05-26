FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CoffeeShop/CoffeeShop.csproj", "CoffeeShop/"]
RUN dotnet restore "CoffeeShop/CoffeeShop.csproj"
COPY . .
WORKDIR "/src/CoffeeShop"
RUN dotnet build "CoffeeShop.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoffeeShop.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoffeeShop.dll"]
