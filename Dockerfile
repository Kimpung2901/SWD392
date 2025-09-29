# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1) Chỉ copy file *.csproj để restore nhanh (đúng đường dẫn thư mục)
COPY WebNameProjectOfSWD/WebNameProjectOfSWD.csproj WebNameProjectOfSWD/
COPY BLL/BLL.csproj BLL/
COPY DAL/DAL.csproj DAL/
# (nếu có .sln, bạn có thể COPY thêm)
# COPY SWD392.sln ./

# 2) Restore dựa trên project chính (hoặc .sln nếu bạn dùng)
RUN dotnet restore WebNameProjectOfSWD/WebNameProjectOfSWD.csproj

# 3) Copy toàn bộ mã nguồn sau khi restore thành công
COPY . .

# 4) Publish (không cần AppHost để image gọn)
RUN dotnet publish WebNameProjectOfSWD/WebNameProjectOfSWD.csproj -c Release -o /app /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .

# Render cấp biến PORT -> ép Kestrel listen đúng PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENV URLS=http://0.0.0.0:${PORT}

# Bật swagger ở Production nếu muốn (có thể xoá)
ENV Swagger__Enabled=true

# KHÔNG hardcode --urls 5122 hay EXPOSE 5122
ENTRYPOINT ["dotnet", "WebNameProjectOfSWD.dll"]
