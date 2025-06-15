# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
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

# Sao chép thư mục Db/ nếu bạn cần nó trong container (thường không cần cho build/run API)
# COPY Db/ Db/

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
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/ApiService/out .

ENV ASPNETCORE_URLS=http://+:$PORT
ENTRYPOINT ["dotnet", "ApiService.dll"]