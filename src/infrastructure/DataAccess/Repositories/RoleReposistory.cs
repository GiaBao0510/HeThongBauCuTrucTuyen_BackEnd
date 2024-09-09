using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class RoleReposistory : IDisposable, IRoleRepository
    {
        private readonly DatabaseContext _context;

        //Hàm khởi tạo
        public RoleReposistory(DatabaseContext context){
            _context = context;
        }

        //Liệt kê
        public async Task<List<Roles>> _GetListOfRoles(){
            var roles = new List<Roles>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM vaitro", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                roles.Add(new Roles{
                    RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                    TenVaiTro = reader.GetString(reader.GetOrdinal("TenVaiTro"))
                });
            }

            return roles;
        }

        //Thêm
        public async Task _AddRole(Roles role){
            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand($"INSERT INTO vaitro(TenVaiTro) VALUES(@TenVaiTro);", connection);
            command.Parameters.AddWithValue("@TenVaiTro",role.TenVaiTro);
            await command.ExecuteNonQueryAsync();
        }

        //Lấy thông tin theo ID
        public async Task<List<Roles>> _GetRoleBy_ID(string id){
            var listRole = new List<Roles>();
            using var connection = await _context.Get_MySqlConnection();
            
            //Mệnh đề truy vấn
            string sql = $"SELECT * FROM vaitro WHERE RoleID = @RoleID";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@RoleID",id);
            
            //In thông tin tìm được
            using var reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync()){
                listRole.Add(new Roles{
                    RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                    TenVaiTro = reader.GetString(reader.GetOrdinal("TenVaiTro")),
                });
            }

            return listRole;
        }
        
        //Sửa
        public async Task<bool> _EditRoleBy_ID(string ID, Roles role)
        {
            using var connection = await _context.Get_MySqlConnection();
            
            // Kiểm tra xem vai trò có tồn tại không
            string sqlCheck = "SELECT COUNT(*) FROM vaitro WHERE RoleID = @RoleID";
            using (var commandCheck = new MySqlCommand(sqlCheck, connection))
            {
                commandCheck.Parameters.AddWithValue("@RoleID", ID);
                int count = Convert.ToInt32(await commandCheck.ExecuteScalarAsync());
                if (count == 0)
                    return false;
            }
            
            // Thực hiện cập nhật
            string sqlUpdate = "UPDATE vaitro SET TenVaiTro = @TenVaiTro WHERE RoleID = @RoleID";
            using (var commandUpdate = new MySqlCommand(sqlUpdate, connection))
            {
                commandUpdate.Parameters.AddWithValue("@TenVaiTro", role.TenVaiTro);
                commandUpdate.Parameters.AddWithValue("@RoleID", ID);
                await commandUpdate.ExecuteNonQueryAsync();
            }
            
            return true;
        }

        //Xóa
        public async Task<bool> _DeleteRoleBy_ID(string ID)
        {
            using var connection = await _context.Get_MySqlConnection();
            
            // Kiểm tra xem vai trò có tồn tại không
            string sqlCheck = "SELECT COUNT(*) FROM vaitro WHERE RoleID = @RoleID";
            using (var commandCheck = new MySqlCommand(sqlCheck, connection))
            {
                commandCheck.Parameters.AddWithValue("@RoleID", ID);
                int count = Convert.ToInt32(await commandCheck.ExecuteScalarAsync());
                if (count == 0)
                    return false;
            }
            
            // Thực hiện xóa
            string sqlUpdate = "DELETE FROM vaitro WHERE RoleID = @RoleID";
            using (var commandUpdate = new MySqlCommand(sqlUpdate, connection))
            {
                commandUpdate.Parameters.AddWithValue("@RoleID", ID);
                await commandUpdate.ExecuteNonQueryAsync();
            }
            
            return true;
        }

        public void Dispose(){
            _context.Dispose();
        }
    }
}