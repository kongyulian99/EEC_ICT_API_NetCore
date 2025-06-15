# Stage 1: Build the application
# Sử dụng .NET SDK image. Bạn đang dùng .NET Core 3.1, nên hãy giữ đúng phiên bản này.
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
WORKDIR /app

# 1. Sao chép file solution (.sln) vào thư mục làm việc gốc của container.
# File này nằm ở gốc repo, ngang hàng với Dockerfile.
COPY ApiService.sln .

# 2. SAO CHÉP TOÀN BỘ THƯ MỤC 'ApiService/' VÀ CÁC THƯ MỤC CON CỦA NÓ.
# Lệnh này sẽ đưa:
# - ApiService.csproj
# - Thư mục ApiService.Business/ (cùng với ApiService.Business.csproj)
# - Thư mục ApiService.Core/ (cùng với ApiService.Core.csproj) <-- ĐÂY LÀ CHỖ KHẮC PHỤC LỖI CỦA BẠN
# - Thư mục ApiService.Entity/, ApiService.Implement/, ApiService.Interface/ (và các .csproj của chúng)
# ... vào đường dẫn /app/ApiService/ trong container.
COPY ApiService/ ApiService/

# 3. Restore tất cả các dependencies cho toàn bộ solution.
# Lệnh này sẽ tìm ApiService.sln tại /app/ApiService.sln và tất cả các project con
# tại các đường dẫn tương đối (ví dụ: /app/ApiService/ApiService.Core/ApiService.Core.csproj).
RUN dotnet restore ApiService.sln

# 4. Sao chép toàn bộ mã nguồn còn lại của dự án vào container.
# Bước này thực hiện sau 'dotnet restore' để tối ưu cache layer của Docker.
# Nó sẽ sao chép các file .cs, Controllers, Models, appsettings, v.v. mà chưa được copy ở trên.
COPY . .

# 5. Chuyển đổi thư mục làm việc vào project API chính (ApiService)
# Sau đó publish ứng dụng này.
# Đường dẫn ở đây là /app/ApiService vì thư mục ApiService/ được COPY vào /app/ApiService.
WORKDIR /app/ApiService
RUN dotnet publish -c Release -o out --no-restore # --no-restore vì đã restore ở trên

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
# Copy the published output from the 'build-env' stage to the runtime image.
# Đây sẽ bao gồm ApiService.dll và tất cả các thư viện phụ thuộc
# (ApiService.Business.dll, ApiService.Core.dll, v.v.)
COPY --from=build-env /app/ApiService/out .

# Set environment variable for the port. ASP.NET Core will listen on this.
ENV ASPNETCORE_URLS=http://+:$PORT

# Define the entry point for the container
ENTRYPOINT ["dotnet", "ApiService.dll"] # Đảm bảo đúng tên file DLL của project API chính của bạn