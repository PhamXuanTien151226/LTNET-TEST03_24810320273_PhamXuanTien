using System;
using System.Data;
using MySql.Data.MySqlClient;
using InventoryApp.DTO;

namespace InventoryApp.DAL
{
    public class VatTuDAL
    {
        public DataTable GetAllVatTu()
        {
            using MySqlConnection conn = DatabaseHelper.GetConnection();
            string sql = "SELECT MaVatTu, TenVatTu, DVT, SoLuongTon FROM VatTu";
            MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable SearchVatTu(string keyword)
        {
            using MySqlConnection conn = DatabaseHelper.GetConnection();
            string sql = "SELECT MaVatTu, TenVatTu, DVT, SoLuongTon FROM VatTu WHERE MaVatTu LIKE @kw OR TenVatTu LIKE @kw";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}