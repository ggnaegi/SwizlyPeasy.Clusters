#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SwizlyPeasy.Clusters.Demo/SwizlyPeasy.Clusters.Demo.csproj", "SwizlyPeasy.Clusters.Demo/"]
RUN dotnet restore "SwizlyPeasy.Clusters.Demo/SwizlyPeasy.Clusters.Demo.csproj"
COPY . .
WORKDIR "/src/SwizlyPeasy.Clusters.Demo"
RUN dotnet build "SwizlyPeasy.Clusters.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SwizlyPeasy.Clusters.Demo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SwizlyPeasy.Clusters.Demo.dll"]