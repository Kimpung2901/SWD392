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

# 2) Copy toàn bộ source & publish Release (self-contained không cần thiết trên Render)
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

# -------------------------
# QUAN TRỌNG CHO RENDER
# -------------------------
# Render đặt biến môi trường PORT (ví dụ 10000). Phải cho Kestrel lắng nghe 0.0.0.0:$PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
# Nếu muốn mặc định local khi PORT không có:
ENV PORT=8080

# Tuỳ chọn: bật các header từ reverse proxy của Render (để Scheme/ClientIP đúng)
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Expose cho local test (Render sẽ tự map cổng theo $PORT)
EXPOSE 8080

# ENTRYPOINT chạy app
ENTRYPOINT ["dotnet", "WebNameProjectOfSWD.dll"]
