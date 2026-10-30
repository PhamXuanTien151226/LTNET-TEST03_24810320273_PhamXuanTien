using System;
using System.Collections.Generic;

namespace InventoryApp.DTO
{
    public class PhieuKhoDTO
    {
        public int MaPhieu { get; set; }
        public string LoaiPhieu { get; set; } = "NHAP";
        public DateTime NgayLap { get; set; } = DateTime.Now;
        public int? MaNCC { get; set; }
        public decimal TongTien { get; set; }
        public List<ChiTietPhieuDTO> ChiTietList { get; set; } = new List<ChiTietPhieuDTO>();
    }
}