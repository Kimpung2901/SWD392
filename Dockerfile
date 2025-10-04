# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ solution (để các ProjectReference như BLL/DAL cũng có mặt)
COPY . .

# Restore đúng project API
RUN dotnet restore WebNameProjectOfSWD/WebNameProjectOfSWD.csproj

# Publish ra /app (không dùng AppHost để image gọn)
RUN dotnet publish WebNameProjectOfSWD/WebNameProjectOfSWD.csproj -c Release -o /app /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Render cung cấp biến PORT -> buộc Kestrel listen đúng port
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENV URLS=http://0.0.0.0:${PORT}

# (tùy chọn) Bật swagger ở Production qua env var
ENV Swagger__Enabled=true

ENTRYPOINT ["dotnet", "WebNameProjectOfSWD.dll"]
