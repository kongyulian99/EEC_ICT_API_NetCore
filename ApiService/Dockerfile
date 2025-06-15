# Sử dụng .NET Core SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /app

# Copy solution và restore các dependencies
COPY . .
RUN dotnet restore ApiService.sln

# Build và publish ứng dụng
RUN dotnet publish -c Release -o out

# Sử dụng ASP.NET Core runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Thiết lập biến môi trường cho production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Expose cổng 8080 (render.com sử dụng port này)
EXPOSE 8080

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "ApiService.dll"]
