# Use the ASP.NET Core runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UnitOfWorkDemo1.csproj", "."]
RUN dotnet restore "./UnitOfWorkDemo1.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "UnitOfWorkDemo1.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "UnitOfWorkDemo1.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UnitOfWorkDemo1.dll"]
