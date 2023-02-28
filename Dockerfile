#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BaseBotService/BaseBotService.csproj", "BaseBotService/"]
RUN dotnet restore "BaseBotService/BaseBotService.csproj"
COPY . .
WORKDIR "/src/BaseBotService"
RUN dotnet build "BaseBotService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BaseBotService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Honeycomb.dll"]