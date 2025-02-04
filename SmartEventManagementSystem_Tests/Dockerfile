FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["SmartEventManagementSystem_Tests/SmartEventManagementSystem_Tests.csproj", "SmartEventManagementSystem_Tests/"]
COPY ["SmartEventManagementSystem/SmartEventManagementSystem.csproj", "SmartEventManagementSystem/"]

RUN dotnet restore "SmartEventManagementSystem_Tests/SmartEventManagementSystem_Tests.csproj"

COPY . .

WORKDIR "/src/SmartEventManagementSystem_Tests"
RUN dotnet build "SmartEventManagementSystem_Tests.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartEventManagementSystem_Tests.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartEventManagementSystem_Tests.dll"]
