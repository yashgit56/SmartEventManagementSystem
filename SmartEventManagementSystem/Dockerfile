FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["SmartEventManagementSystem/SmartEventManagementSystem.csproj", "SmartEventManagementSystem/"]
COPY ["MessageContracts/MessageContracts.csproj", "MessageContracts/"]

RUN dotnet restore "SmartEventManagementSystem/SmartEventManagementSystem.csproj"

COPY . .

WORKDIR "/src/SmartEventManagementSystem"

RUN dotnet build "SmartEventManagementSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartEventManagementSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SmartEventManagementSystem.dll"]
