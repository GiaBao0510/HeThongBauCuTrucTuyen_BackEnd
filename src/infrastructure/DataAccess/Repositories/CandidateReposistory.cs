using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using BackEnd.src.web_api.DTOs;
using BCrypt.Net;
using System.Data;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class CandidateReposistory : IDisposable
    {
        private readonly DatabaseContext _context;

        //Khởi tạo
        public CandidateReposistory(DatabaseContext context) => _context=context;
        //Hủy
        public void Dispose() => _context.Dispose();

        //  ---- 1. Tìm Email có bị trùng không
        private async Task<int> CheckEmail_AlreadyExitsForCandidate(string email, MySqlConnection connection){
            //Nếu đầu vào rỗng thì trả về false
            if(string.IsNullOrEmpty(email)) return 1;

            const string sql_check = "SELECT COUNT(Email) FROM UngCuVien WHERE Email = @Email;";
            using(var Command = new MySqlCommand(sql_check, connection)){
                Command.Parameters.AddWithValue("@Email",email);
                return Convert.ToInt32(await Command.ExecuteScalarAsync());
            }
        }

        //---- 2.Tìm CCCD có bị trung không
        private async Task<int> CheckCCCD_AlreadyExitsForCandidate(string CCCD,MySqlConnection connection){
            
            const string sql_check = "SELECT COUNT(CCCD) FROM UngCuVien WHERE CCCD = @CCCD;";
            using(var Command = new MySqlCommand(sql_check, connection)){
                Command.Parameters.AddWithValue("@CCCD",CCCD);
                return Convert.ToInt32(await Command.ExecuteScalarAsync());
            }
        }

        //---- 3.TÌm SDT có bị trùng không
        private async Task<int> CheckSDT_AlreadyExitsForCandidate(string SDT,MySqlConnection connection){

            const string sql_check = "SELECT COUNT(SDT) FROM UngCuVien WHERE SDT = @SDT;";
            using(var Command = new MySqlCommand(sql_check, connection)){
                Command.Parameters.AddWithValue("@SDT",SDT);
                return Convert.ToInt32(await Command.ExecuteScalarAsync());
            }
        }

        //---- 4.Tìm vai trò có tồn tại không nếu không tồn tại thì trả về false
        private async Task<bool> CheckRole_AlreadyExitsForCandidate(int role,MySqlConnection connection){
        
            const string sql_check = "SELECT COUNT(RoleID) FROM vaitro WHERE RoleID = @RoleID;";
            using(var Command = new MySqlCommand(sql_check, connection)){
                Command.Parameters.AddWithValue("@RoleID",role);
                int count = Convert.ToInt32(await Command.ExecuteScalarAsync());
                if(count < 0)
                    return false;       //Không tồn tại
            }
            return true;
        }

        //--- Kiểm tra thông tin không được trùng lặp như email, sdt, cccd
        private async Task<int> CheckTheCandidateInformationLoop(int N,CandidateDto UngCuVien, MySqlConnection connection){
            if(await CheckSDT_AlreadyExitsForCandidate(UngCuVien.SDT, connection) > N) return 0;
            if(await CheckEmail_AlreadyExitsForCandidate(UngCuVien.Email, connection) > N) return -1;
            if(await CheckCCCD_AlreadyExitsForCandidate(UngCuVien.CCCD, connection) > N) return -2;
            if(await CheckRole_AlreadyExitsForCandidate(UngCuVien.RoleID, connection) == false) return -3;
            return 1;
        }

        //Hàm mã hóa mật khẩu
        private string HashPassword(string pwd){
            string salt = BCrypt.Net.BCrypt.GenerateSalt(); //Tạo giá trị muối ngẫu nhiên
            int costFactor = 10; //Giá trị mặc định của cost là 10
            string hashedPwd = BCrypt.Net.BCrypt.HashPassword(pwd+salt, costFactor);

            return hashedPwd;
        }

        //hàm Kiểm tra các thông tin quan trọng không được null
        private bool CheckInformationIsNotEmpty(CandidateDto UngCuVien){
            if(UngCuVien.HoTen == null || UngCuVien.CCCD == null || UngCuVien.SDT == null || UngCuVien.GioiTinh == null || UngCuVien.GioiTinh.Length > 1 || UngCuVien.DiaChiLienLac == null
            || UngCuVien.Email == null || UngCuVien.RoleID == null || UngCuVien == null)
                return false;
            return true;
        }

        //Kiểm tra mật khẩu cũ
        private bool CheckOldPassword(string oldpwd, string hashedpwd){
            Console.WriteLine($"Match: {BCrypt.Net.BCrypt.Verify(oldpwd, hashedpwd)}");
            return BCrypt.Net.BCrypt.Verify(oldpwd, hashedpwd);
        }

        //  ---- 5.Thêm ------
        public async Task<int> _AddCandidate(CandidateDto UngCuVien){
            using var connection = await _context.Get_MySqlConnection();

            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open){
                await connection.OpenAsync();
            }

            //Kiểm tra đầu vào là không được null và giá trị hợp lệ
            if(CheckInformationIsNotEmpty(UngCuVien) == false){
                Console.WriteLine("Xuất hiện có lỗi trong kiểm tra rỗng");
                return -4;
            }

            //Kiểm tra điều kiện đầu vào sao cho SDT, email, cccd không được trùng nhau mới được phép thêm vào
            int KiemTraTrungNhau = await CheckTheCandidateInformationLoop(0, UngCuVien, connection);
            if(KiemTraTrungNhau <= 0){
                Console.WriteLine("Xuất hiện có lỗi trong kiểm tra trùng nhau");
                return KiemTraTrungNhau;
            }

            //Tạo ID tự động
            DateTime currentTime = DateTime.Now;
            string ID_ucv = $"{currentTime:yyyyMMddHHmmss}";

            //Mã hóa mật khẩu, CCCD
            string hashedPwd = HashPassword(UngCuVien.MatKhau);
            string hashedCCCD = HashPassword(UngCuVien.CCCD);

            //Thực hiện thêm bảng cử tri           
            string Input = @"INSERT INTO UngCuVien(ID_ucv,HoTen,GioiTinh,NgaySinh,DiaChiLienLac,CCCD,SDT,Email,HinhAnh,RoleID,TrangThai) 
            VALUES(@ID_ucv,@HoTen,@GioiTinh,@NgaySinh,@DiaChiLienLac,@CCCD,@SDT,@Email,@HinhAnh,@RoleID,@TrangThai);";

            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@ID_ucv",ID_ucv);
                commandAdd.Parameters.AddWithValue("@HoTen",UngCuVien.HoTen);
                commandAdd.Parameters.AddWithValue("@GioiTinh",UngCuVien.GioiTinh);
                commandAdd.Parameters.AddWithValue("@NgaySinh",UngCuVien.NgaySinh);
                commandAdd.Parameters.AddWithValue("@DiaChiLienLac",UngCuVien.DiaChiLienLac);
                commandAdd.Parameters.AddWithValue("@CCCD",hashedCCCD);
                commandAdd.Parameters.AddWithValue("@SDT",UngCuVien.SDT);
                commandAdd.Parameters.AddWithValue("@Email",UngCuVien.Email);
                commandAdd.Parameters.AddWithValue("@HinhAnh",UngCuVien.HinhAnh);
                commandAdd.Parameters.AddWithValue("@RoleID",UngCuVien.RoleID);
                commandAdd.Parameters.AddWithValue("@TrangThai",UngCuVien.TrangThai);
                await commandAdd.ExecuteNonQueryAsync();
                Console.WriteLine("Thêm thông tin cá nhân thành công");
            } 

            //Thực hiện thêm bảng tài khoản
            string inputAccount = @"INSERT INTO taikhoan(TaiKhoan,MatKhau,BiKhoa,LyDoKhoa,NgayTao,SuDung,RoleID)
            VALUES(@TaiKhoan,@MatKhau,@BiKhoa,@LyDoKhoa,@NgayTao,@SuDung,@RoleID);";

            using(var commandAddAcount = new MySqlCommand(inputAccount, connection)){
                commandAddAcount.Parameters.AddWithValue("@TaiKhoan",UngCuVien.SDT);
                commandAddAcount.Parameters.AddWithValue("@MatKhau",hashedPwd);
                commandAddAcount.Parameters.AddWithValue("@BiKhoa",0);
                commandAddAcount.Parameters.AddWithValue("@LyDoKhoa","null");
                commandAddAcount.Parameters.AddWithValue("@NgayTao",UngCuVien.NgayTao);
                commandAddAcount.Parameters.AddWithValue("@SuDung",1);
                commandAddAcount.Parameters.AddWithValue("@RoleID",UngCuVien.RoleID);

                await commandAddAcount.ExecuteNonQueryAsync();
                Console.WriteLine("Thêm tài khoản thành công");
            }
            Console.WriteLine("OK");

            return 1; 
        }

        //---- 6.Lấy all cử tri
        public async Task<List<Candidate>> _GetListOfCandidate(){
            var list = new List<Candidate>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM UngCuVien", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new Candidate{
                    ID_ucv = reader["ID_ucv"] as string,
                    HoTen = reader["HoTen"] as string,
                    GioiTinh = reader["GioiTinh"] as string,
                    NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")),
                    DiaChiLienLac = reader["DiaChiLienLac"] as string,
                    CCCD = reader["CCCD"] as string,
                    SDT = reader["SDT"] as string,
                    Email = string.IsNullOrEmpty(reader["Email"] as string) ? "" : (reader["Email"] as string),
                    HinhAnh = string.IsNullOrEmpty(reader["HinhAnh"] as string) ? "" : (reader["HinhAnh"] as string),
                    RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                    TrangThai = reader["TrangThai"] as string
                });
            }
            return list;
        }

        //---- 7.Lấy all cử tri - tài khoản
        public async Task<List<CandidateDto>> _GetListOfCandidateAndAccount(){
            var list = new List<CandidateDto>();
            using var connection = await _context.Get_MySqlConnection();
            
            string sql =@"SELECT * FROM UngCuVien ct 
            INNER JOIN taikhoan tk ON tk.TaiKhoan = ct.SDT";
            using var command = new MySqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new CandidateDto{
                    ID_ucv = reader["ID_ucv"] as string,
                    HoTen = reader["HoTen"] as string,
                    GioiTinh = reader["GioiTinh"] as string,
                    NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")),
                    DiaChiLienLac = reader["DiaChiLienLac"] as string,
                    CCCD = reader["CCCD"] as string,
                    SDT = reader["SDT"] as string,
                    TrangThai = reader["TrangThai"] as string,
                    Email = string.IsNullOrEmpty(reader["Email"] as string) ? "" : (reader["Email"] as string),
                    HinhAnh = string.IsNullOrEmpty(reader["HinhAnh"] as string) ? "" : (reader["HinhAnh"] as string),
                    RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                    TaiKhoan = reader["TaiKhoan"] as string,
                    MatKhau = reader["MatKhau"] as string,
                    BiKhoa = reader["BiKhoa"] as string,
                    LyDoKhoa = reader["LyDoKhoa"] as string,
                    NgayTao = reader.GetDateTime(reader.GetOrdinal("NgayTao")),
                    SuDung = reader.GetInt32(reader.GetOrdinal("SuDung"))
                });
            }
            return list;
        }    

        //---- 8.Lấy theo ID
        public async Task<Candidate> _GetCandidateBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = @"
                SELECT * FROM UngCuVien
                WHERE ID_ucv = @ID_ucv";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_ucv",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new Candidate{
                    ID_ucv = reader["ID_ucv"] as string,
                    HoTen = reader["HoTen"] as string,
                    GioiTinh = reader["GioiTinh"] as string,
                    NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")),
                    DiaChiLienLac = reader["DiaChiLienLac"] as string,
                    CCCD = reader["CCCD"] as string,
                    SDT = reader["SDT"] as string,
                    TrangThai = reader["TrangThai"] as string,
                    Email = string.IsNullOrEmpty(reader["Email"] as string) ? "" : (reader["Email"] as string),
                    HinhAnh = string.IsNullOrEmpty(reader["HinhAnh"] as string) ? "" : (reader["HinhAnh"] as string),
                    RoleID = reader.GetInt32(reader.GetOrdinal("RoleID"))
                };
            }

            return null;
        }

        //---- 9.Sửa theo ID - Admin
        public async Task<int> _EditCandidateBy_ID_Admin(string ID, CandidateDto UngCuVien){
            using var connection = await _context.Get_MySqlConnection();

            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

            //Kiểm tra xem thông tin đầu vào phải không được trùng chỉ được trùng với bản thân thôi
            int KiemTraTrungNhau = await CheckTheCandidateInformationLoop(1, UngCuVien, connection);
            if(KiemTraTrungNhau <= 0){
                Console.WriteLine("Xuất hiện có lỗi trong kiểm tra trùng nhau");
                return KiemTraTrungNhau;
            }

            //Kiểm tra đầu vào là không được null và giá trị hợp lệ
            if(CheckInformationIsNotEmpty(UngCuVien) == false){
                Console.WriteLine("Xuất hiện có lỗi trong kiểm tra rỗng");
                return -4;
            }
            
            //Băm mật khẩu, CCCD
            string hashedPwd = HashPassword(UngCuVien.MatKhau),
                    hashedCCCD = HashPassword(UngCuVien.CCCD);

            const string sqlUngCuVien = @"
                UPDATE UngCuVien 
                SET HoTen = @HoTen, GioiTinh=@GioiTinh, NgaySinh=@NgaySinh,
                    DiaChiLienLac=@DiaChiLienLac, CCCD=@CCCD, SDT=@SDT,
                    Email=@Email, HinhAnh=@HinhAnh, RoleID=@RoleID, TrangThai=@TrangThai
                WHERE ID_ucv = @ID_ucv;";

            //Cập nhật cử tri
            using (var command0 = new MySqlCommand(sqlUngCuVien, connection)){
                command0.Parameters.AddWithValue("@ID_ucv",ID);
                command0.Parameters.AddWithValue("@HoTen",UngCuVien.HoTen);
                command0.Parameters.AddWithValue("@GioiTinh",UngCuVien.GioiTinh);
                command0.Parameters.AddWithValue("@NgaySinh",UngCuVien.NgaySinh);
                command0.Parameters.AddWithValue("@DiaChiLienLac",UngCuVien.DiaChiLienLac);
                command0.Parameters.AddWithValue("@CCCD",hashedCCCD);
                command0.Parameters.AddWithValue("@SDT",UngCuVien.SDT);
                command0.Parameters.AddWithValue("@Email",UngCuVien.Email);
                command0.Parameters.AddWithValue("@HinhAnh",UngCuVien.HinhAnh);
                command0.Parameters.AddWithValue("@TrangThai",UngCuVien.TrangThai);
                command0.Parameters.AddWithValue("@RoleID",UngCuVien.RoleID);
                
                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command0.ExecuteNonQueryAsync();
                if(rowAffected < 0){
                    return -5;
                }
                Console.WriteLine("Đã hoàn thành cập nhật thông tin cá nhân");
            }

            const string sqlTaiKhoan = @"
                UPDATE taikhoan
                SET TaiKhoan=@SDT, MatKhau=@MatKhau, BiKhoa=@BiKhoa,
                    LyDoKhoa=@LyDoKhoa, NgayTao=@NgayTao, SuDung=@SuDung,
                    RoleID=@RoleID
                WHERE TaiKhoan = @TaiKhoan;";
            //Cập nhật tài khoản
            using (var command1 = new MySqlCommand(sqlTaiKhoan, connection)){
                command1.Parameters.AddWithValue("@SDT",UngCuVien.SDT);
                command1.Parameters.AddWithValue("@TaiKhoan",UngCuVien.TaiKhoan);
                command1.Parameters.AddWithValue("@MatKhau",hashedPwd);
                command1.Parameters.AddWithValue("@BiKhoa",UngCuVien.BiKhoa);
                command1.Parameters.AddWithValue("@LyDoKhoa",UngCuVien.LyDoKhoa);
                command1.Parameters.AddWithValue("@NgayTao",UngCuVien.NgayTao);
                command1.Parameters.AddWithValue("@RoleID",UngCuVien.RoleID);
                command1.Parameters.AddWithValue("@SuDung",UngCuVien.SuDung);
                
                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command1.ExecuteNonQueryAsync();
                if(rowAffected < 0){
                    return -6;
                } 
                
                Console.WriteLine("Đã hoàn thành cập nhật thông tin tài khoản");
            }

            return 1;            
        }

        //---- 10.Sửa theo ID - Candidate
        public async Task<int> _EditCandidateBy_ID_Candidate(string ID, CandidateDto UngCuVien){
            using var connection = await _context.Get_MySqlConnection();

            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

            //Kiểm tra xem thông tin đầu vào phải không được trùng chỉ được trùng với bản thân thôi
            int KiemTraTrungNhau = await CheckTheCandidateInformationLoop(1, UngCuVien, connection);
            if(KiemTraTrungNhau <= 0){
                Console.WriteLine("Xuất hiện có lỗi trong kiểm tra trùng nhau");
                return KiemTraTrungNhau;
            }

            //Kiểm tra đầu vào là không được null và giá trị hợp lệ
            if(CheckInformationIsNotEmpty(UngCuVien) == false){
                Console.WriteLine("Xuất hiện có lỗi trong kiểm tra rỗng");
                return -4;
            }
            
            // --- Kiểm tra mật khẩu cũ xem người dùng có nhớ không
            const string sqlMatKhau = "SELECT MatKhau FROM Taikhoan WHERE TaiKhoan = @TaiKhoan";

            //Kiểm tra mật khẩu nếu khớp thì mới cho cập nhật
            using (var command2 = new MySqlCommand(sqlMatKhau, connection)){
                command2.Parameters.AddWithValue("@TaiKhoan",UngCuVien.SDT);
                using var reader = await command2.ExecuteReaderAsync();
                
                if(string.IsNullOrEmpty(UngCuVien.MatKhauCu)) return -7;

                if(await reader.ReadAsync()){
                    string hashdedpw = reader["MatKhau"] as string;
                    Console.WriteLine($">HashPWD: {hashdedpw}");
                    Console.WriteLine($">OldPWD: {UngCuVien.MatKhauCu}");
                    if (hashdedpw == null || !CheckOldPassword(UngCuVien.MatKhauCu, hashdedpw))
                    {
                        return -7; // Mật khẩu cũ không khớp
                    }
                    else
                    {
                        Console.WriteLine("Đổi PWD thành công");
                    }
                }
            }
            
            //Băm mật khẩu, CCCD
            string hashedPwd = HashPassword(UngCuVien.MatKhau),
                    hashedCCCD = HashPassword(UngCuVien.CCCD);

            const string sqlUngCuVien = @"
                UPDATE UngCuVien 
                SET HoTen = @HoTen, GioiTinh=@GioiTinh, NgaySinh=@NgaySinh,
                    DiaChiLienLac=@DiaChiLienLac, CCCD=@CCCD, SDT=@SDT,
                    Email=@Email, HinhAnh=@HinhAnh, RoleID=@RoleID,TrangThai=@TrangThai
                WHERE ID_ucv = @ID_ucv;";

            //Cập nhật cử tri
            using (var command0 = new MySqlCommand(sqlUngCuVien, connection)){
                command0.Parameters.AddWithValue("@ID_ucv",ID);
                command0.Parameters.AddWithValue("@HoTen",UngCuVien.HoTen);
                command0.Parameters.AddWithValue("@GioiTinh",UngCuVien.GioiTinh);
                command0.Parameters.AddWithValue("@NgaySinh",UngCuVien.NgaySinh);
                command0.Parameters.AddWithValue("@DiaChiLienLac",UngCuVien.DiaChiLienLac);
                command0.Parameters.AddWithValue("@CCCD",hashedCCCD);
                command0.Parameters.AddWithValue("@SDT",UngCuVien.SDT);
                command0.Parameters.AddWithValue("@Email",UngCuVien.Email);
                command0.Parameters.AddWithValue("@HinhAnh",UngCuVien.HinhAnh);
                command0.Parameters.AddWithValue("@RoleID",UngCuVien.RoleID);
                command0.Parameters.AddWithValue("@TrangThai",UngCuVien.TrangThai);
                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command0.ExecuteNonQueryAsync();
                if(rowAffected < 0){
                    return -5;
                }
                Console.WriteLine("Đã hoàn thành cập nhật thông tin cá nhân");
            }

            const string sqlTaiKhoan = @"
                UPDATE taikhoan
                SET TaiKhoan=@SDT, MatKhau=@MatKhau, BiKhoa=@BiKhoa,
                    LyDoKhoa=@LyDoKhoa, NgayTao=@NgayTao, SuDung=@SuDung,
                    RoleID=@RoleID
                WHERE TaiKhoan = @TaiKhoan;";
            //Cập nhật tài khoản
            using (var command1 = new MySqlCommand(sqlTaiKhoan, connection)){
                command1.Parameters.AddWithValue("@SDT",UngCuVien.SDT);
                command1.Parameters.AddWithValue("@TaiKhoan",UngCuVien.TaiKhoan);
                command1.Parameters.AddWithValue("@MatKhau",hashedPwd);
                command1.Parameters.AddWithValue("@BiKhoa",UngCuVien.BiKhoa);
                command1.Parameters.AddWithValue("@LyDoKhoa",UngCuVien.LyDoKhoa);
                command1.Parameters.AddWithValue("@NgayTao",UngCuVien.NgayTao);
                command1.Parameters.AddWithValue("@RoleID",UngCuVien.RoleID);
                command1.Parameters.AddWithValue("@SuDung",UngCuVien.SuDung);
                
                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command1.ExecuteNonQueryAsync();
                if(rowAffected < 0){
                    return -6;
                } 
                
                Console.WriteLine("Đã hoàn thành cập nhật thông tin tài khoản");
            }

            return 1;            
        }

        //Xóa theo ID
        public async Task<bool> _DeleteCandidateBy_ID(string ID){
            using var connection = await _context.Get_MySqlConnection();

            //Tìm số điện thoại theo ID để dò tài khoản
            const string sql_SDT = "SELECT SDT FROM UngCuVien WHERE ID_ucv = @ID_ucv";
            string Acc = null;
            using (var command0 = new MySqlCommand(sql_SDT, connection)){
                command0.Parameters.AddWithValue("@ID_ucv",ID);
                
                //Nếu có lấy ID tài khoản
                using var reader = await command0.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                    Acc = reader.GetString(reader.GetOrdinal("SDT"));
            }

            //Nếu ID không tìm thấy thì xóa
            if(string.IsNullOrEmpty(Acc))
                return false;

            //Xóa tài khoản
            const string sql_deleteAcc = "DELETE FROM TaiKhoan WHERE TaiKhoan = @TaiKhoan";
            using (var command1 = new MySqlCommand(sql_deleteAcc, connection)){
                command1.Parameters.AddWithValue("@TaiKhoan",Acc);
                int rowAffected = await command1.ExecuteNonQueryAsync();
                if(rowAffected < 1) return false;
            }

            //Xóa thông tin cử tri
            const string sqldelete = "DELETE FROM UngCuVien WHERE ID_ucv = @ID_ucv";
            
            using (var command2 = new MySqlCommand(sqldelete, connection)){
                command2.Parameters.AddWithValue("@ID_ucv",ID);    
                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command2.ExecuteNonQueryAsync();
                if(rowAffected < 1) return false;
            }
            return true;
        }

    }
}