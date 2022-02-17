#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["DataService/DataService.csproj", "DataService/"]
COPY ["Interfaces/Interfaces.csproj", "Interfaces/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Repository.InMemory/Repository.InMemory.csproj", "Repository.InMemory/"]
COPY ["Contexts/Contexts.csproj", "Contexts/"]
RUN dotnet restore "DataService/DataService.csproj"
COPY . .
WORKDIR "/src/DataService"
RUN dotnet build "DataService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataService.dll"]