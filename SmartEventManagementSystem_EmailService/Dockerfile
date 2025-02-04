FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["SmartEventManagementSystem_EmailService/SmartEventManagementSystem_EmailService.csproj", "SmartEventManagementSystem_EmailService/"]
COPY ["MessageContracts/MessageContracts.csproj", "MessageContracts/"]

RUN dotnet restore "SmartEventManagementSystem_EmailService/SmartEventManagementSystem_EmailService.csproj"

COPY . .

WORKDIR "/src/SmartEventManagementSystem_EmailService"
RUN dotnet build "SmartEventManagementSystem_EmailService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartEventManagementSystem_EmailService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartEventManagementSystem_EmailService.dll"]
