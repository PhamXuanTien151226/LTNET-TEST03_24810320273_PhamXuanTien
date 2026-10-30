using System.Data;
using InventoryApp.DAL;
using InventoryApp.DTO;

namespace InventoryApp.BUS
{
    public class InventoryBUS
    {
        private VatTuDAL vatTuDAL = new VatTuDAL();
        private PhieuKhoDAL phieuKhoDAL = new PhieuKhoDAL();

        public DataTable GetDanhSachVatTu() => vatTuDAL.GetAllVatTu();

        public DataTable TimKiemVatTu(string keyword) => vatTuDAL.SearchVatTu(keyword.Trim());

        public (bool Success, string Message) XuLyLuuPhieu(PhieuKhoDTO phieu)
        {
            if (phieu.ChiTietList.Count == 0)
                return (false, "Phiếu phải có ít nhất 1 mặt hàng!");

            foreach (var item in phieu.ChiTietList)
            {
                if (item.SoLuong <= 0) return (false, "Số lượng mặt hàng phải lớn hơn 0!");
                if (item.DonGia < 0) return (false, "Đơn giá không được âm!");
            }

            return phieuKhoDAL.SavePhieuKhoTransaction(phieu);
        }
    }
}