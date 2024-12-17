# Bước 1: Sử dụng SDK .NET để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Sao chép tệp dự án và khôi phục gói nuget
COPY ["BackEnd.csproj", "./"]
RUN dotnet restore "BackEnd.csproj"

# Sao chép toàn bộ mã nguồn và build ứng dụng
COPY . ./
RUN dotnet publish "BackEnd.csproj" -c Release -o /app/out

# Bước 2: Sử dụng Runtime .NET để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Sao chép kết quả từ giai đoạn build
COPY --from=build /app/out ./

# Mở cổng 80 nếu cần
EXPOSE 80

# Khởi chạy ứng dụng
ENTRYPOINT ["dotnet", "BackEnd.dll"]