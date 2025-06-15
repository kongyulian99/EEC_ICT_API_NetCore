# Stage 1: Build the application
# Sử dụng .NET SDK image. Bạn đang dùng .NET Core 3.1, nên hãy giữ đúng phiên bản này.
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
WORKDIR /app

# BƯỚC QUAN TRỌNG: SAO CHÉP CÁC FILE PROJECT (*.csproj) TRƯỚC!
# Sao chép file solution (.sln) vào thư mục làm việc gốc của container.
# Nó nằm ở thư mục gốc của repo, nên chỉ cần tên file.
COPY ApiService.sln .

# Sao chép các thư mục project con và file .csproj của chúng.
# Các đường dẫn này là tương đối so với thư mục gốc của repo (build context).
# Vì tất cả các project con đều nằm trong thư mục ApiService/,
# bạn cần chỉ định đường dẫn từ gốc repo tới từng .csproj.
COPY ApiService/ApiService.csproj ApiService/
COPY ApiService/ApiService.Business/ApiService.Business.csproj ApiService/ApiService.Business/
COPY ApiService/ApiService.Core/ApiService.Core.csproj ApiService/ApiService.Core/
COPY ApiService/ApiService.Entity/ApiService.Entity.csproj ApiService/ApiService.Entity/
COPY ApiService/ApiService.Implement/ApiService.Implement.csproj ApiService/ApiService.Implement/
COPY ApiService/ApiService.Interface/ApiService.Interface.csproj ApiService/ApiService.Interface/
# THÊM CÁC DÒNG COPY TƯƠNG TỰ CHO BẤT KỲ PROJECT CON NÀO KHÁC NẾU CÓ,
# Đảm bảo đường dẫn nguồn bắt đầu bằng 'ApiService/'

# Restore tất cả các dependencies cho toàn bộ solution.
# Lệnh này sẽ tìm ApiService.sln tại /app/ApiService.sln và các project con tại các đường dẫn tương đối.
RUN dotnet restore ApiService.sln

# Sao chép toàn bộ mã nguồn còn lại của dự án vào container.
# Bước này thực hiện sau 'dotnet restore' để tối ưu cache layer của Docker.
# Nó sẽ sao chép tất cả các file .cs, Controllers, Models, v.v. mà chưa được copy ở trên.
# (Và cả các file trong thư mục Db/ nếu bạn muốn chúng có trong container,
# mặc dù thường không cần cho runtime của API)
COPY . .

# Chuyển đổi thư mục làm việc vào project API chính (ApiService)
# Sau đó publish ứng dụng này.
# Đường dẫn ở đây là /app/ApiService vì thư mục ApiService/ được COPY vào /app/ApiService.
WORKDIR /app/ApiService
RUN dotnet publish -c Release -o out --no-restore # --no-restore vì đã restore ở trên

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/ApiService/out . # Đảm bảo đường dẫn này đúng output từ publish

ENV ASPNETCORE_URLS=http://+:$PORT
ENTRYPOINT ["dotnet", "ApiService.dll"]