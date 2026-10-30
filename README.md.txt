# BÁO CÁO BÀI TẬP THỰC HÀNH TỔNG HỢP LẬP TRÌNH .NET C#

## MỤC 1: THÔNG TIN SINH VIÊN
- **Họ và tên sinh viên:** Phạm Xuân Tiến
- **Mã sinh viên:** 24810320273
- **Lớp học phần:** Lập trình C# / Windows Forms - D19QTANM1
- **Tên Đề tài / Bài tập:** Bài tập 5.3, 5.4, 5.5 - Hệ thống Quán Cafe (POS), Khách sạn & Quản lý Xuất Nhập Kho

---

## MỤC 2: HƯỚNG DẪN CÀI ĐẶT & CHẠY DỰ ÁN
1. Khởi động MySQL trên **XAMPP Control Panel** (Port 3306).
2. Chạy script SQL để tạo 3 cơ sở dữ liệu: `CafePOSDB`, `HotelDB`, `inventorydb`.
3. Mở file Solution `InventoryApp.sln` bằng Visual Studio.
4. Nhấn **F5** để khởi chạy Menu điều hướng tổng hợp (`frmMainApp`).

---

## MỤC 3: HÌNH ẢNH KẾT QUẢ CÁC MÀN HÌNH CHỨC NĂNG

### 1. Màn hình Chính / Tổng quan
![Màn hình chính](images/01_main_screen.png)
*Minh họa: Form điều hướng tổng hợp truy cập vào cả 3 bài tập.*

### 2. Màn hình Quản lý Danh mục / Tra cứu (CRUD)
![Màn hình CRUD](images/02_crud_screen.png)
*Minh họa: Tìm kiếm vật tư, nạp dữ liệu mô hình ngắt kết nối với DataTable/MySqlDataAdapter.*

### 3. Màn hình Thực thi Nghiệp vụ chính
![Màn hình Nghiệp vụ](images/03_transaction_screen.png)
*Minh họa: Nghiệp vụ Gọi món trên sơ đồ bàn động (Bài 5.3) và Lập phiếu kho (Bài 5.5).*

### 4. Màn hình Báo cáo - Thống kê
![Màn hình Thống kê](images/04_report_screen.png)
*Minh họa: Lọc danh sách trạng thái phòng khách sạn ngắt kết nối và tính tổng tiền hóa đơn.*

### 5. Minh họa Báo lỗi Giao dịch (Transaction Rollback)
![Minh họa Xử lý lỗi](images/05_error_handling.png)
*Minh họa: Cơ chế Rollback hủy toàn bộ giao dịch khi số lượng xuất vượt quá tồn kho hiện có, bảo đảm tính toàn vẹn dữ liệu.*