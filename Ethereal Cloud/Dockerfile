#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN mkdir -p /home/app/.aspnet/DataProtection-Keys && \
    chown -R app:app /home/app/.aspnet && \
    chmod -R 700 /home/app/.aspnet
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Ethereal Cloud.csproj", "Ethereal Cloud/"]
WORKDIR "/src/Ethereal Cloud"
RUN dotnet restore "./Ethereal Cloud.csproj"
COPY . .

RUN dotnet build "./Ethereal Cloud.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ethereal Cloud.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY certs/ /app/certs/
ENTRYPOINT ["dotnet", "Ethereal Cloud.dll"]