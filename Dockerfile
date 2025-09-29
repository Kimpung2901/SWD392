# ===========================
# STAGE 1: Restore + Build
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file csproj
COPY ["WebNameProjectOfSWD/WebNameProjectOfSWD.csproj", "WebNameProjectOfSWD/"]

# Restore packages
RUN dotnet restore "WebNameProjectOfSWD/WebNameProjectOfSWD.csproj"

# Copy toàn bộ source
COPY . .

WORKDIR "/src/WebNameProjectOfSWD"

# Build project
RUN dotnet build "WebNameProjectOfSWD.csproj" -c Release -o /app/build

# ===========================
# STAGE 2: Publish
# ===========================
FROM build AS publish
RUN dotnet publish "WebNameProjectOfSWD.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===========================
# STAGE 3: Runtime
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebNameProjectOfSWD.dll"]
