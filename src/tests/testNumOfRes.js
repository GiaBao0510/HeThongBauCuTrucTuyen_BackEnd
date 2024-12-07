import http from 'k6/http';
import { check, sleep } from 'k6';

// Mảng mã cử tri
const idCuTriArray = ["20241207185619", "24120719553358", "24120719465843", "20241207191459", "20241207190225", "24120719371837", "24120719535862", "24120719553335", "24120719511452", "24120719564029", "24120720073601", "24120720010889", "24120719333962", "24120719351088", "24120719365836", "24120720014942", "20241204204902", "24120719310947", "24120719423736", "24120720052766", "24120719321676", "24120720061887", "24120720072773", "20241207191447", "24120719291289", "24120719494782", "24120720003564", "24120719374876", "24120719481622", "24120720063511", "20241207185040", "24120720061408", "24120719464222", "24120720080047", "24120720003184", "24120720095330", "24120719581377", "20241207191315", "24120720031714", "24120719381518", "24120719425333", "24120719595968", "24120719262805", "24120719580964", "20241207190102", "24120719562777", "24120720100311", "20241207190230", "24120719301448", "24120719422015", "24120719461587", "24120720022543", "24120719315661", "24120719325033", "24120719440688", "24120719554971", "20241207185722", "24120720134866", "24120720120461", "24120719420151", "24120719570022", "24120719571663", "24120719595560", "24120719564478", "24120719571202", "24120719585567", "24120720032061", "20241207185700", "20241207190701", "24120719433511", "24120720085681", "20241207191047", "24120719360418", "24120720020485", "20241207191525", "24120719363346", "24120719384202", "24120719505280", "24120719572265", "24120720062237", "20240916232132", "24120719423473", "20241207190202", "24120719325350", "24120719383273", "24120719570989", "24120719543811", "24120720110744", "20241204201439", "24120719371459", "24120719441079", "24120719452612", "24120719494809", "20241207191211", "20241207190310", "24120719575547", "20241207185015", "20241207185032", "20241207191249", "24120720053693", "24120720132688", "20241207191105", "20241207190122", "20241207190406", "24120720140085", "24120719574439"];

// Token của bạn
const token = 'eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJTRFQiOiJhZG1pbjIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiIxIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoicGdiYW9wNEBnbWFpbC5jb20iLCJqdGkiOiJiZWI4ZDZiMS1lMzE2LTQ2OGMtYWExMC05ZTM3MTU4MjhjYWMiLCJTdUR1bmciOiIxIiwiQmlLaG9hIjoiMCIsImV4cCI6MTczNDc2NDIwMCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDg1IiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDg1In0.XcM7CV_Voq7SvXZfpS3hFEd2o82uk4BT1aFCRDKt3jU'; // Thay thế bằng token của bạn
// Thiết lập tùy chọn kiểm thử
export let options = {
    stages: [
        { duration: '1m', target: 10 },  // Tăng dần lên 50 VUs trong 1 phút
        { duration: '5m', target: 10 },  // Duy trì 50 VUs trong 3 phút
        { duration: '1m', target: 0 },   // Giảm dần về 0 VUs trong 1 phút
    ],
};

// Biến toàn cục để quản lý chỉ số cử tri
let voterIndex = 0;

export default function () {
    // Lấy chỉ số cử tri hiện tại và tăng lên cho lần tiếp theo
    const index = __ITER;
    
    // Kiểm tra nếu đã hết cử tri
    if (index >= idCuTriArray.length) {
        return;
    }

    const idCuTri = idCuTriArray[index];

    // Tạo giá trị phiếu bầu ngẫu nhiên (giả sử giá trị trong khoảng từ 100 đến 200)
    const minVoteValue = 100;
    const maxVoteValue = 900;
    const giaTriPhieuBau = Math.floor(Math.random() * (maxVoteValue - minVoteValue + 1)) + minVoteValue;

    const url = 'http://localhost:3001/api/Voter/voter-vote';
    const payload = JSON.stringify({
        "ID_CuTri": idCuTri,
        "GiaTriPhieuBau": giaTriPhieuBau,
        "ngayBD": "2024-12-07 10:18:18",
        "ID_Cap": 14,
        "ID_DonViBauCu": 10
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token,
        },
    };

    const res = http.post(url, payload, params);

    // Kiểm tra phản hồi
    check(res, {
        'status is 200': (r) => r.status === 200,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });

    // Nghỉ một thời gian ngẫu nhiên để phân tán tải
    sleep(1); // Nghỉ từ 1 đến 3 giây
}