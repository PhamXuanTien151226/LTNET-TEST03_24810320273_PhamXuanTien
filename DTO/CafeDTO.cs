namespace InventoryApp.DTO
{
    public class BanAnDTO
    {
        public int MaBan { get; set; }
        public string TenBan { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Trống";
    }

    public class MonAnDTO
    {
        public int MaMon { get; set; }
        public string TenMon { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
    }
}