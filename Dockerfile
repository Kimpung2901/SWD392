# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app

# ---------- Run ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .

# BẮT app listen đúng cổng Render (PORT)
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENV URLS=http://0.0.0.0:${PORT}

# (tuỳ chọn) bật swagger prod qua env var
ENV Swagger__Enabled=true

# KHÔNG EXPOSE 5122, KHÔNG dùng --urls cứng 5122
ENTRYPOINT ["dotnet", "WebNameProjectOfSWD.dll"]
