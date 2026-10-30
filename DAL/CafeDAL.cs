using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace InventoryApp.DAL
{
    public class CafeDAL
    {
        private readonly string connStr = "Server=localhost;Database=CafePOSDB;Uid=root;Pwd=;Charset=utf8mb4;";

        // Nạp danh sách bàn ăn (Disconnected Model)
        public DataTable GetDanhSachBan()
        {
            using MySqlConnection conn = new MySqlConnection(connStr);
            string sql = "SELECT MaBan, TenBan, TrangThai FROM BanAn";
            MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable GetDanhSachMon()
        {
            using MySqlConnection conn = new MySqlConnection(connStr);
            string sql = "SELECT MaMon, TenMon, DonGia FROM MonAn";
            MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable GetChiTietHoaDonHienTai(int maBan)
        {
            using MySqlConnection conn = new MySqlConnection(connStr);
            string sql = @"SELECT m.TenMon, c.SoLuong, c.DonGia, (c.SoLuong * c.DonGia) AS ThanhTien 
                           FROM ChiTietHoaDon c 
                           JOIN MonAn m ON c.MaMon = m.MaMon 
                           JOIN HoaDon h ON c.MaHoaDon = h.MaHoaDon 
                           WHERE h.MaBan = @MaBan AND h.TrangThai = 0";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaBan", maBan);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // Gọi món & Cập nhật trạng thái Có khách
        public bool GoiMon(int maBan, int maMon, int soLuong, decimal donGia)
        {
            using MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            MySqlTransaction trans = conn.BeginTransaction();
            try
            {
                // Kiểm tra hóa đơn chưa thanh toán của bàn
                string sqlHd = "SELECT MaHoaDon FROM HoaDon WHERE MaBan = @MaBan AND TrangThai = 0";
                MySqlCommand cmdHd = new MySqlCommand(sqlHd, conn, trans);
                cmdHd.Parameters.AddWithValue("@MaBan", maBan);
                object? res = cmdHd.ExecuteScalar();

                int maHd;
                if (res == null)
                {
                    string sqlTaoHd = "INSERT INTO HoaDon (MaBan, TrangThai) VALUES (@MaBan, 0); SELECT LAST_INSERT_ID();";
                    MySqlCommand cmdTao = new MySqlCommand(sqlTaoHd, conn, trans);
                    cmdTao.Parameters.AddWithValue("@MaBan", maBan);
                    maHd = Convert.ToInt32(cmdTao.ExecuteScalar());

                    // Chuyển trạng thái bàn sang "Có khách"
                    string sqlUpBan = "UPDATE BanAn SET TrangThai = 'Có khách' WHERE MaBan = @MaBan";
                    MySqlCommand cmdUp = new MySqlCommand(sqlUpBan, conn, trans);
                    cmdUp.Parameters.AddWithValue("@MaBan", maBan);
                    cmdUp.ExecuteNonQuery();
                }
                else
                {
                    maHd = Convert.ToInt32(res);
                }

                // Chèn hoặc cập nhật chi tiết món
                string sqlInsertCT = @"INSERT INTO ChiTietHoaDon (MaHoaDon, MaMon, SoLuong, DonGia) 
                                       VALUES (@MaHD, @MaMon, @SL, @DG) 
                                       ON DUPLICATE KEY UPDATE SoLuong = SoLuong + @SL";
                MySqlCommand cmdCT = new MySqlCommand(sqlInsertCT, conn, trans);
                cmdCT.Parameters.AddWithValue("@MaHD", maHd);
                cmdCT.Parameters.AddWithValue("@MaMon", maMon);
                cmdCT.Parameters.AddWithValue("@SL", soLuong);
                cmdCT.Parameters.AddWithValue("@DG", donGia);
                cmdCT.ExecuteNonQuery();

                trans.Commit();
                return true;
            }
            catch
            {
                trans.Rollback();
                return false;
            }
        }

        // Thanh toán gộp Transaction
        public bool ThanhToan(int maBan, decimal tongTien)
        {
            using MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            MySqlTransaction trans = conn.BeginTransaction();
            try
            {
                // 1. Cập nhật hóa đơn
                string sqlHd = @"UPDATE HoaDon SET GioRa = NOW(), TongTien = @TongTien, TrangThai = 1 
                                 WHERE MaBan = @MaBan AND TrangThai = 0";
                MySqlCommand cmdHd = new MySqlCommand(sqlHd, conn, trans);
                cmdHd.Parameters.AddWithValue("@TongTien", tongTien);
                cmdHd.Parameters.AddWithValue("@MaBan", maBan);
                cmdHd.ExecuteNonQuery();

                // 2. Chuyển trạng thái bàn về Trống
                string sqlBan = "UPDATE BanAn SET TrangThai = 'Trống' WHERE MaBan = @MaBan";
                MySqlCommand cmdBan = new MySqlCommand(sqlBan, conn, trans);
                cmdBan.Parameters.AddWithValue("@MaBan", maBan);
                cmdBan.ExecuteNonQuery();

                trans.Commit();
                return true;
            }
            catch
            {
                trans.Rollback();
                return false;
            }
        }
    }
}