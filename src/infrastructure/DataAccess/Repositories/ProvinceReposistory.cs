using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class ProvinceReposistory : IDisposable
    {
        private readonly DatabaseContext _context;

        //Khởi tạo
        public ProvinceReposistory(DatabaseContext context) => _context = context;

        //Liệt kê
        public async Task<List<Province>> _GetListOfProvice(){
            var list = new List<Province>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM tinhthanh", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new Province{
                    STT = reader.GetInt32(reader.GetOrdinal("STT")),
                    TenTinhThanh = reader.GetString(reader.GetOrdinal("TenTinhThanh"))
                });
            }
            return list;
        }

        //Thêm
        public async Task<bool> _AddProvince(Province tinhthanh){
            using var connection = await _context.Get_MySqlConnection();
            
            //Kiểm tra số thứ tự có trùng không nếu trùng thì khoog thêm được
            string checkInput = "SELECT COUNT(*) FROM tinhthanh WHERE STT = @STT";
            using(var commandCheck = new MySqlCommand(checkInput, connection)){
                commandCheck.Parameters.AddWithValue("@STT",tinhthanh.STT);
                int count = Convert.ToInt32(await commandCheck.ExecuteScalarAsync());
                if(count > 0)
                    return false;
            }

            //Thực hiện thêm            
            string Input = $"INSERT INTO tinhthanh(STT,TenTinhThanh) VALUES(@STT ,@TenTinhThanh);";
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@STT",tinhthanh.STT);
                commandAdd.Parameters.AddWithValue("@TenTinhThanh",tinhthanh.TenTinhThanh);
                await commandAdd.ExecuteNonQueryAsync();
            } 

            return true; 
        }

        //Lấy theo ID
        public async Task<Province> _GetProvinceBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = @"
                SELECT * FROM tinhthanh 
                WHERE STT = @STT";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@STT",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new Province{
                    STT = reader.GetInt32(reader.GetOrdinal("STT")),
                    TenTinhThanh = reader.GetString(reader.GetOrdinal("TenTinhThanh"))
                };
            }

            return null;
        }

        //Sửa
        public async Task<bool> _EditProvinceBy_ID(string ID, Province province){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                UPDATE tinhthanh 
                SET TenTinhThanh = @TenTinhThanh
                WHERE STT = @STT";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@STT",ID);
            command.Parameters.AddWithValue("@TenTinhThanh",province.TenTinhThanh);

            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }

        //Xóa
        public async Task<bool> _DeleteProvinceBy_ID(string ID){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                DELETE FROM tinhthanh
                WHERE STT = @STT";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@STT",ID);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }
        
        public void Dispose() => _context.Dispose();
    }
}