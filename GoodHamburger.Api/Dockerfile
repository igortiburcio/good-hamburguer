# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GoodHamburger.Api/GoodHamburger.Api.csproj", "GoodHamburger.Api/"]
COPY ["GoodHamburger.Application/GoodHamburger.Application.csproj", "GoodHamburger.Application/"]
COPY ["GoodHamburger.Infra/GoodHamburger.Infra.csproj", "GoodHamburger.Infra/"]
COPY ["GoodHamburger.Domain/GoodHamburger.Domain.csproj", "GoodHamburger.Domain/"]

RUN dotnet restore "GoodHamburger.Api/GoodHamburger.Api.csproj"

COPY . .
RUN dotnet publish "GoodHamburger.Api/GoodHamburger.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GoodHamburger.Api.dll"]
