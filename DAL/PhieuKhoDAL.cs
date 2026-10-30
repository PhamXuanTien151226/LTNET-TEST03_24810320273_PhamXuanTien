using System;
using System.Data;
using MySql.Data.MySqlClient;
using InventoryApp.DTO;

namespace InventoryApp.DAL
{
    public class PhieuKhoDAL
    {
        public (bool Success, string Message) SavePhieuKhoTransaction(PhieuKhoDTO phieu)
        {
            using MySqlConnection conn = DatabaseHelper.GetConnection();
            conn.Open();
            MySqlTransaction trans = conn.BeginTransaction();

            try
            {
                // 1. Thêm Phiếu Kho
                string sqlPhieu = @"INSERT INTO PhieuKho (LoaiPhieu, NgayLap, MaNCC, TongTien) 
                                    VALUES (@LoaiPhieu, @NgayLap, @MaNCC, @TongTien);
                                    SELECT LAST_INSERT_ID();";
                MySqlCommand cmdPhieu = new MySqlCommand(sqlPhieu, conn, trans);
                cmdPhieu.Parameters.AddWithValue("@LoaiPhieu", phieu.LoaiPhieu);
                cmdPhieu.Parameters.AddWithValue("@NgayLap", phieu.NgayLap);
                cmdPhieu.Parameters.AddWithValue("@MaNCC", (object?)phieu.MaNCC ?? DBNull.Value);
                cmdPhieu.Parameters.AddWithValue("@TongTien", phieu.TongTien);

                int maPhieuGenerated = Convert.ToInt32(cmdPhieu.ExecuteScalar());

                // 2. Duyệt từng chi tiết và cập nhật tồn kho
                foreach (var item in phieu.ChiTietList)
                {
                    if (phieu.LoaiPhieu == "XUAT")
                    {
                        // Khóa dòng và kiểm tra tồn kho
                        string sqlCheck = "SELECT SoLuongTon FROM VatTu WHERE MaVatTu = @MaVT FOR UPDATE";
                        MySqlCommand cmdCheck = new MySqlCommand(sqlCheck, conn, trans);
                        cmdCheck.Parameters.AddWithValue("@MaVT", item.MaVatTu);
                        object? res = cmdCheck.ExecuteScalar();

                        if (res == null)
                        {
                            trans.Rollback();
                            return (false, $"Vật tư [{item.MaVatTu}] không tồn tại!");
                        }

                        decimal tonHienTai = Convert.ToDecimal(res);
                        if (tonHienTai < item.SoLuong)
                        {
                            trans.Rollback(); // ROLLBACK khi không đủ hàng xuất
                            return (false, $"Vật tư [{item.MaVatTu}] chỉ còn {tonHienTai}, không đủ xuất {item.SoLuong}!");
                        }

                        // Trừ tồn kho
                        string sqlUpdate = "UPDATE VatTu SET SoLuongTon = SoLuongTon - @SL WHERE MaVatTu = @MaVT";
                        MySqlCommand cmdUp = new MySqlCommand(sqlUpdate, conn, trans);
                        cmdUp.Parameters.AddWithValue("@SL", item.SoLuong);
                        cmdUp.Parameters.AddWithValue("@MaVT", item.MaVatTu);
                        cmdUp.ExecuteNonQuery();
                    }
                    else // Nhập kho: Cộng tồn kho
                    {
                        string sqlUpdate = "UPDATE VatTu SET SoLuongTon = SoLuongTon + @SL WHERE MaVatTu = @MaVT";
                        MySqlCommand cmdUp = new MySqlCommand(sqlUpdate, conn, trans);
                        cmdUp.Parameters.AddWithValue("@SL", item.SoLuong);
                        cmdUp.Parameters.AddWithValue("@MaVT", item.MaVatTu);
                        cmdUp.ExecuteNonQuery();
                    }

                    // Thêm Chi Tiết Phiếu
                    string sqlCT = @"INSERT INTO ChiTietPhieuKho (MaPhieu, MaVatTu, SoLuong, DonGia) 
                                     VALUES (@MaPhieu, @MaVT, @SL, @DG)";
                    MySqlCommand cmdCT = new MySqlCommand(sqlCT, conn, trans);
                    cmdCT.Parameters.AddWithValue("@MaPhieu", maPhieuGenerated);
                    cmdCT.Parameters.AddWithValue("@MaVT", item.MaVatTu);
                    cmdCT.Parameters.AddWithValue("@SL", item.SoLuong);
                    cmdCT.Parameters.AddWithValue("@DG", item.DonGia);
                    cmdCT.ExecuteNonQuery();
                }

                trans.Commit();
                return (true, $"Lưu phiếu {phieu.LoaiPhieu} thành công! Mã phiếu: {maPhieuGenerated}");
            }
            catch (Exception ex)
            {
                try { trans.Rollback(); } catch { }
                return (false, "Lỗi Transaction: " + ex.Message);
            }
        }
    }
}