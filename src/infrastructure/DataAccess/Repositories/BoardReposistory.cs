using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class BoardReposistory : IDisposable
    {
        private readonly DatabaseContext _context;

        //Khởi tạo
        public BoardReposistory(DatabaseContext context) => _context = context;

        //hủy
        public void Dispose() => _context.Dispose();

        //Liệt kê
        public async Task<List<BoardDto>> _GetListOfBoard(){
            var list = new List<BoardDto>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM ban", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new BoardDto{
                    ID_Ban = reader.GetInt32(reader.GetOrdinal("ID_Ban")),
                    TenBan = reader.GetString(reader.GetOrdinal("TenBan")),
                    ID_DonViBauCu = reader.GetInt32(reader.GetOrdinal("ID_DonViBauCu"))
                });
            }
            return list;
        }

        //thêm
        public async Task<bool> _AddBoard(Board ban){
            using var connection = await _context.Get_MySqlConnection();
            
            //Kiểm tra mã số đơn vị bầu cử tại ban có tồn tại không
            string checkInput = "SELECT COUNT(*) FROM donvibaucu WHERE ID_DonViBauCu = @ID_DonViBauCu";
            using(var commandCheck = new MySqlCommand(checkInput, connection)){
                commandCheck.Parameters.AddWithValue("@ID_DonViBauCu",ban.ID_DonViBauCu);
                int count = Convert.ToInt32(await commandCheck.ExecuteScalarAsync());
                if(count < 1)
                    return false;
            }

            //Thực hiện thêm            
            string Input = $"INSERT INTO ban(Tenban,ID_DonViBauCu) VALUES(@Tenban,@ID_DonViBauCu);";
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@Tenban",ban.TenBan);
                commandAdd.Parameters.AddWithValue("@ID_DonViBauCu",ban.ID_DonViBauCu);
                await commandAdd.ExecuteNonQueryAsync();
            } 

            return true; 
        }

        //Lấy theo ID
        public async Task<Board> _GetBoardBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = @"
                SELECT * FROM ban 
                WHERE ID_Ban = @ID_Ban";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_Ban",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new Board{
                    ID_Ban = reader.GetInt32(reader.GetOrdinal("ID_Ban")),
                    TenBan = reader.GetString(reader.GetOrdinal("TenBan")),
                    ID_DonViBauCu = reader.GetInt32(reader.GetOrdinal("ID_DonViBauCu"))
                };
            }

            return null;
        }

        //Sửa
        public async Task<bool> _EditBoardBy_ID(string ID, Board Board){
            using var connection = await _context.Get_MySqlConnection();

            //Tìm kiếm quận huyện có tồn tại không
            const string sqlCheck = "SELECT COUNT(*) FROM donvibaucu WHERE ID_DonViBauCu = @ID_DonViBauCu";
            using(var command0 = new MySqlCommand(sqlCheck, connection)){
                command0.Parameters.AddWithValue("@ID_DonViBauCu",Board.ID_DonViBauCu);
                int count = Convert.ToInt32(await command0.ExecuteScalarAsync());
                
                if(count < 1)
                    return false;
            }

            //Cập nhật
            const string sqlupdate = @"UPDATE ban SET TenBan = @TenBan, ID_DonViBauCu = @ID_DonViBauCu WHERE ID_Ban = @ID_Ban";
            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@ID_Ban",ID);
                command.Parameters.AddWithValue("@TenBan",Board.TenBan);
                command.Parameters.AddWithValue("@ID_DonViBauCu",Board.ID_DonViBauCu);

                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected > 0;
            }
            
        }

        //Xóa
        public async Task<bool> _DeleteBoardBy_ID(string ID){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                DELETE FROM ban
                WHERE ID_Ban = @ID_Ban";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@ID_Ban",ID);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }
    }
}