import http from 'k6/http';
import { check, sleep } from 'k6';

const LastName = ['Lý','Hồ','Trương','Lê','Lưu', 'Cao', 'Vũ', 'Đặng', 'Bùi', 'Đỗ', 'Hà', 'Ngô', 'Dương', 'Đinh', 'Phan', 'Đoàn', 'Vương'];
const FirstName = ['Gia Bảo','Minh Huy','Lộc Phát','Hữu Tài','Mỹ Nhân', 'Minh Trường', 'Trọng Phúc', 'Hoàng Duy', 'Hồng Phú', 'Hữu Đức', 'Trọng Nghĩa', 'Minh Nghĩa', 'Hữu Phúc', 'Minh Phúc', 'Hữu Phát', 'Minh Phát', 'Hữu Phong', 'Minh Phong', 'Hữu Phú', 'Minh Phú'];

// Giữ nguyên các hàm helper
function randomName() {
    return LastName[Math.floor(Math.random() * LastName.length)] + ' ' + FirstName[Math.floor(Math.random() * FirstName.length)];
}

function generateRandomPhoneNumber() {
    let phoneNumber = '';
    for (let i = 0; i < 10; i++) {
        phoneNumber += Math.floor(Math.random() * 10);
    }
    return phoneNumber;
}

function getRandomEmail() {
    const domains = ["gmail.com", "yahoo.com", "outlook.com", "hotmail.com"];
    const characters = "abcdefghijklmnopqrstuvwxyz0123456789";
    const usernameLength = Math.floor(Math.random() * 10) + 5;
    let username = "";

    for (let i = 0; i < usernameLength; i++) {
        username += characters[Math.floor(Math.random() * characters.length)];
    }
    return `${username}@${domains[Math.floor(Math.random() * domains.length)]}`;
}

function getRandomDate() {
    const startDate = new Date(1975, 0, 1);
    const endDate = new Date(2005, 11, 31);
    const randomMillis = Math.floor(Math.random() * (endDate - startDate + 1)) + startDate.getTime();
    const randomDate = new Date(randomMillis);
    return `${randomDate.getFullYear()}-${String(randomDate.getMonth() + 1).padStart(2, '0')}-${String(randomDate.getDate()).padStart(2, '0')}`;
}

const token = 'eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJTRFQiOiJhZG1pbjIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiIxIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoicGdiYW9wNEBnbWFpbC5jb20iLCJqdGkiOiJiZWI4ZDZiMS1lMzE2LTQ2OGMtYWExMC05ZTM3MTU4MjhjYWMiLCJTdUR1bmciOiIxIiwiQmlLaG9hIjoiMCIsImV4cCI6MTczNDc2NDIwMCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDg1IiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDg1In0.XcM7CV_Voq7SvXZfpS3hFEd2o82uk4BT1aFCRDKt3jU';

// Cấu hình tối ưu cho 2000 requests
export let options = {
    vus: 10, // Sử dụng 10 virtual users
    iterations: 2000, // Tổng số tài khoản cần tạo
    rps: 9, // Giới hạn 9 requests/giây (~540 requests/phút, dưới giới hạn 600)
};

export default function () {
    const sdt = generateRandomPhoneNumber();
    let data = {
        "hoTen": randomName(),
        "gioiTinh": "1",
        "ngaySinh": getRandomDate(),
        "diaChiLienLac": "3/2, Q.Ninh Kiều, TP.Cần Thơ",
        "sdt": sdt,
        "email": getRandomEmail(),
        "taiKhoan": sdt,
        "iD_DanToc": "1",
        "iD_ChucVu": "13"
    };

    const url = 'http://localhost:3000/api/Voter';
    const boundary = '----WebKitFormBoundary7MA4YWxkTrZu0gW';

    let payload = '';
    for (let key in data) {
        payload += `--${boundary}\r\n`;
        payload += `Content-Disposition: form-data; name="${key}"\r\n\r\n`;
        payload += `${data[key]}\r\n`;
    }
    payload += `--${boundary}--\r\n`;

    const params = {
        headers: {
            'Content-Type': 'multipart/form-data; boundary=' + boundary,
            'Authorization': 'Bearer ' + token,
        },
    };

    const res = http.post(url, payload, params);

    // Kiểm tra kết quả
    check(res, {
        'status is 200': (r) => r.status === 200,
    });

    // Thêm delay để đảm bảo không vượt quá rate limit
    sleep(0.1); // 100ms delay giữa các request
}