FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "RGroupConstruction.API/RGroupConstruction.API.csproj"
RUN dotnet build "RGroupConstruction.API/RGroupConstruction.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RGroupConstruction.API/RGroupConstruction.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RGroupConstruction.API.dll"]
