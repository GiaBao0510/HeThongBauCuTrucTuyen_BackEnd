#Tải image aspnet:8.0 từ Microsoft Container Registry
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

#Tạo và chuyển đến thư mục /app trong container
WORKDIR /app

# Khai báo cổng 80(http) và 443(https) cho container
EXPOSE 80
EXPOSE 443

#Tải image SDK .Net đầy đủ  
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

#Tạo và chuyển đến thư mục src
WORKDIR /src

#copy tất cả file .csproj vào container và chạy lệnh dotnet restore để tải về các gói
COPY ["BackEnd.csproj","./"]
RUN dotnet restore

#copy toàn bộ source vào container và Build ứng dụng ở chế độ Release và output vào thư mục /app/build
COPY . .
RUN dotnet build -c Release -o /app/build

#Tạo stage "publish" từ stage "build"
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

#Quay lại lấy image base(runtime)
FROM base AS final
WORKDIR /app

#copy các file đã publish từ stage "publish"
COPY --from=publish /app/publish .

#Thiết lập lệnh chạy ứng dụng khi container khởi động
ENTRYPOINT ["dotnet", "HeThongBauCuTrucTuyen_BackEnd.dll"]

#Thiết lập biến môi trường cho Aspnet core và khai báo ứng dụng đang chạy trong container
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_RUNNING_IN_CONTAINER=true

#Tạo user mới không có quyền root # chuyển sang sử dụng user này để chạy ứng dụng
RUN useradd -m myappuser
USER myappuser