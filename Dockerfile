# Stage 1: Build the application
# Sử dụng image SDK của .NET 8.0 làm base cho giai đoạn xây dựng (build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Sao chép file solution (.sln) vào thư mục làm việc gốc của container.
# Nó nằm ở gốc repo.
COPY ApiService.sln .

# Sao chép TẤT CẢ các thư mục project (chính và con) vào thư mục làm việc gốc của container.
# Bởi vì tất cả chúng đều nằm ngang cấp với ApiService.sln.
# Đảm bảo casing khớp chính xác với tên thư mục của bạn.
COPY ApiService/ ApiService/
COPY ApiService.Business/ ApiService.Business/
COPY ApiService.Core/ ApiService.Core/
COPY ApiService.Entity/ ApiService.Entity/
COPY ApiService.Implement/ ApiService.Implement/
COPY ApiService.Interface/ ApiService.Interface/
# Nếu bạn có thêm thư mục project nào khác nằm ngang cấp, hãy thêm dòng COPY tương ứng.

# Chạy lệnh khôi phục các gói NuGet cho toàn bộ solution.
# Lệnh này sẽ tìm ApiService.sln tại /app/ApiService.sln và các project con
# tại các đường dẫn tương đối (ví dụ: /app/ApiService.Core/ApiService.Core.csproj).
RUN dotnet restore ApiService.sln

# Sao chép toàn bộ mã nguồn còn lại của dự án vào container.
# Bước này thực hiện sau 'dotnet restore' để tối ưu lớp cache của Docker.
COPY . .

# Chuyển thư mục làm việc vào project API chính (ApiService)
# Sau đó publish ứng dụng này cho môi trường Release.
# Đường dẫn ở đây là /app/ApiService
WORKDIR /app/ApiService
RUN dotnet publish -c Release -o out --no-restore

# Stage 2: Create the runtime image
# Sử dụng image runtime của ASP.NET Core 8.0 làm base cho image cuối cùng.
# Image này nhỏ gọn hơn và chỉ chứa những gì cần thiết để chạy ứng dụng.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Mở cổng mà ứng dụng Kestrel sẽ lắng nghe bên trong container.
# Chúng ta sẽ đặt cổng này là 8080 để khớp với việc mapping cổng từ host
EXPOSE 8080

# Sao chép các file đã publish từ giai đoạn build vào image runtime
COPY --from=build-env /app/ApiService/out .

# Định nghĩa lệnh khởi động ứng dụng khi container chạy.
# Chúng ta truyền đối số --urls để Kestrel lắng nghe trên tất cả các địa chỉ IP
# tại cổng 8080 bên trong container.
ENTRYPOINT ["dotnet", "ApiService.dll", "--urls", "http://+:8080"]
