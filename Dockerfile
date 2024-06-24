FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UnitOfWorkDemo1.csproj", "."]
RUN dotnet restore "./UnitOfWorkDemo1.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "UnitOfWorkDemo1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UnitOfWorkDemo1.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UnitOfWorkDemo1.dll"]
