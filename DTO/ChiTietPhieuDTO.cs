namespace InventoryApp.DTO
{
    public class ChiTietPhieuDTO
    {
        public string MaVatTu { get; set; } = string.Empty;
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
}