# =========================
#  Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1) Copy solution & csproj để cache restore (sửa tên nếu khác)
COPY WebNameProjectOfSWD.sln ./
COPY WebNameProjectOfSWD/WebNameProjectOfSWD.csproj ./WebNameProjectOfSWD/
# Nếu có class libraries, mở comment và sửa đúng tên:
# COPY BLL/BLL.csproj ./BLL/
# COPY DAL/DAL.csproj ./DAL/

RUN dotnet restore

# 2) Copy toàn bộ source & publish Release
COPY . .
WORKDIR /src/WebNameProjectOfSWD
RUN dotnet publish -c Release -o /app

# =========================
#  Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy output đã publish
COPY --from=build /app .

# Thiết lập biến môi trường cho Kestrel
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Tuỳ chọn: bật các header từ reverse proxy
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

EXPOSE 8080

# ENTRYPOINT chạy app
ENTRYPOINT ["dotnet", "WebNameProjectOfSWD.dll"]