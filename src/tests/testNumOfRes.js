import http from 'k6/http';
import { check, sleep } from 'k6';

// Mảng mã cử tri
const idCuTriArray = ["24120816262719", "24121018385282", "24120815222208", "24120816251931", "24121015253991", "24121015335729", "24121015263341", "24121015561044", "24121016414644", "24121015291148", "24121016012588", "24121016561105", "24121018351027", "24121016402697", "24120815372316", "24120816041579", "24121017042654", "24120816174191", "24121016013810", "24120816223037", "24121016394254", "24120816193165", "24121015291565", "24121015331671", "24121015590450", "24121015301612", "24121015325504", "24120816131541", "24121016404828", "24121018300900", "24120815242477", "24120815363781", "24121018380852", "24120816211714", "24121016392306", "24121016594413", "24120816081702", "20241204204902", "24120816125432", "24120816220849", "24121016403285", "24121015315951", "24120816155164", "24120816235392", "24121018374773", "24121015555441", "24120816085168", "24120815591240", "24120816115691", "24120816272200", "24120816053662", "24120816270665", "24121016432862", "24121015282875", "24121016451377", "24121018374437", "24120815362250", "24121016433713", "24120815185980", "24120816184541", "24120816185882", "24121016432807", "24120815314832", "24120815572852", "24120816140048", "24120816142796", "24121015323966", "24121016034104", "24120815321259", "24121017003467", "24121018354529", "24120816104890", "24120816281575", "24120816112614", "24121015595060", "24120816022726", "24120816091762", "24120815313492", "24120816123581", "24121016453105", "24121016565251", "24121017030535", "24121015305477", "24121016452461", "24120816102972", "24121015332055", "24121016385376", "24120815414775", "24120816192423", "24120816215545", "24121015273266", "24120816105113", "24120816210945", "24120816211149", "24120816265002", "24120816084119", "24120815190060", "24120815590178", "24120815310346", "24120816101052", "24120816120094", "24121015330101", "24120816031979", "24121015325902", "24121015565475", "24121018365434", "24120816084205", "24120816270716", "24121016454756", "24120816005159", "24120815285456", "24121016432419", "24121016435074", "24121018370933", "24120815250808", "24120816121587", "24120816281554", "24121015261269", "24120816023863", "24120816035974", "24120816155239", "24121016390979", "24121016430383", "24120816005932", "24121018370156", "20240916232132", "24120815304825", "24120815370302", "24120815402581", "24121015272966"]
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
        "ngayBD": "2024-12-10 10:20:20",
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