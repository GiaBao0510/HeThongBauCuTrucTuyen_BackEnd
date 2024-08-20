//let stt = [11,12,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29];
let arrTinh = [
    "Cao Bằng", "Lạng Sơn", "Quảng Ninh", "Hải Phòng", "Thái Bình",
    "Nam Định", "Phú Thọ", "Thái Nguyên", "Yên Bái", "Tuyên Quang","Hà Giang",
    "Lào Cai", "Lai Châu", "Sơn La","Điện Biên" , "Hòa Bình", "Hà Nội", "Hải Dương",
    "Ninh Bình","Thanh Hóa","Nghệ An","Hà Tĩnh","Đà Nẵng","Đắk Lắk","Đắk Nông","Lâm Đồng", "TP.Hồ Chí Minh",
    "Đồng Nai", "Bình Dương","Long An","Tiền Giang","Vĩnh Long","Cần Thơ", "Đồng Tháp",
    "An Giang","Kiên Giang","Cà Mau","Tây Ninh","Bến Tre","Bà Rịa - Vũng Tàu", "Quảng Bình",
    "Quảng Trị","Thừa Thiên Huế", "Quảng Ngãi","Bình Định","Phú Yên","Khánh Hòa","Cục Cảnh sát giao thông", "Gia Lai","Kon Tum",
    "Sóc Trăng","Trà Vinh", "Ninh Thuận", "Bình Thuận","Vĩnh Phúc","Hưng Yên","Hà Nam",
    "Quảng Nam", "Bình Phước","Bạc Liêu","Hậu Giang","Bắc Cạn","Bắc Giang","Bắc Ninh" 
];
let stt = 0, dem = 0;

for(let i = 11; i<= 99;i++){
    
    if(i == 13 || i ==42 | i ==44|i==45|i==46 | i ==87 | i==91 |i==96){
        continue;
    }
    else if(i >=15 && i<16){
        //console.log(dem,": ",i,"-",arrTinh[stt]);
        console.log(`INSERT INTO tinhthanh(STT,TenTinhThanh) VALUES(${i} ,\"${arrTinh[stt]}"\);`);
    }
    else if(i >=29 && i<33){
        //console.log(dem,": ",i,"-",arrTinh[stt]);
        console.log(`INSERT INTO tinhthanh(STT,TenTinhThanh) VALUES(${i} ,\"${arrTinh[stt]}"\);`);
    }
    else if(i == 39 | i == 60){
        console.log(`INSERT INTO tinhthanh(STT,TenTinhThanh) VALUES(${i} ,\"${arrTinh[27]}"\);`);
        if(i ==60) stt++;
    }
    else if(i == 40){
        console.log(`INSERT INTO tinhthanh(STT,TenTinhThanh) VALUES(${i} ,\"${arrTinh[16]}"\);`);
    }
    else if(i == 41){
        console.log(`INSERT INTO tinhthanh(STT,TenTinhThanh) VALUES(${i} ,\"${arrTinh[26]}"\);`);
    }
    else if(i >=50 && i<59){
        console.log(`INSERT INTO tinhthanh(STT,TenTinhThanh) VALUES(${i} ,\"${arrTinh[26]}"\);`);
    }
    
    else{
        console.log(`INSERT INTO tinhthanh(STT,TenTinhThanh) VALUES(${i} ,\"${arrTinh[stt]}"\);`);
        stt++;
    }
    dem++;
}