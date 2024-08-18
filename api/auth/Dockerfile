FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER app
WORKDIR /app
EXPOSE 16080
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["auth.csproj", "."]
RUN dotnet restore "auth.csproj"
COPY . .
RUN dotnet build "auth.csproj" -c $BUILD_CONFIGURATION -o /app

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "auth.csproj" -c $BUILD_CONFIGURATION -o /app /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "auth.dll"]