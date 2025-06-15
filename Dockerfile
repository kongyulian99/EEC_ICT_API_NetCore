# Stage 1: Build the application
# Sử dụng .NET SDK image. Bạn đang dùng .NET Core 3.1, nên hãy giữ đúng phiên bản này.
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
WORKDIR /app

# Sao chép file solution (.sln) vào thư mục làm việc gốc của container.
# Nó nằm ở thư mục gốc của repo, nên chỉ cần tên file.
COPY ApiService.sln .

# Sao chép các thư mục project chính và các project con của bạn.
# Đường dẫn này là tương đối so với thư mục gốc của repo.
# ApiService/ chứa ApiService.csproj và tất cả các project con như ApiService.Business/, v.v.
COPY ApiService/ ApiService/
# Nếu bạn có bất kỳ project nào nằm ngang cấp với ApiService.sln và ApiService/,
# hãy thêm các dòng COPY tương ứng tại đây (ví dụ: COPY OtherProject/ OtherProject/)

# Restore tất cả các dependencies cho toàn bộ solution.
# Lệnh này sẽ tìm MySolution.sln ở /app/MySolution.sln
RUN dotnet restore ApiService.sln

# Sao chép toàn bộ mã nguồn còn lại của dự án vào container.
# Bước này thực hiện sau 'dotnet restore' để tối ưu cache layer của Docker.
# Đây sẽ copy tất cả các file còn lại trong thư mục gốc của repo (ngoại trừ những gì trong .dockerignore)
COPY . .

# Chuyển đổi thư mục làm việc vào project API chính (ApiService)
# Sau đó publish ứng dụng này.
# Đường dẫn ở đây là /app/ApiService vì thư mục ApiService/ được COPY vào /app/ApiService.
WORKDIR /app/ApiService
RUN dotnet publish -c Release -o out --no-restore # --no-restore vì đã restore ở trên

# Stage 2: Create the runtime image
# Sử dụng ASP.NET runtime image cho .NET Core 3.1
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app

# Sao chép các file đã publish từ stage 'build-env' sang runtime image.
# Bao gồm ApiService.dll và tất cả các thư viện phụ thuộc (ApiService.Business.dll, ApiService.Core.dll...).
COPY --from=build-env /app/ApiService/out .

# Đặt biến môi trường PORT để ứng dụng ASP.NET Core lắng nghe trên cổng này.
# Render.com sẽ tự động inject biến môi trường PORT này.
ENV ASPNETCORE_URLS=http://+:$PORT

# Định nghĩa lệnh khởi chạy ứng dụng khi container bắt đầu.
ENTRYPOINT ["dotnet", "ApiService.dll"] # Đảm bảo đúng tên file DLL của project API chính của bạn