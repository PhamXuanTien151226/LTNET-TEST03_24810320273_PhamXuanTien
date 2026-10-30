using InventoryApp.BUS;
using InventoryApp.DAL;
using InventoryApp.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace InventoryApp.GUI
{
    public class frmInventoryMain : Form
    {
        private InventoryBUS bus = new InventoryBUS();
        private List<ChiTietPhieuDTO> dsChiTiet = new List<ChiTietPhieuDTO>();

        private RadioButton radNhap = null!, radXuat = null!;
        private TextBox txtSearch = null!;
        private Button btnSearch = null!, btnRefresh = null!;
        private DataGridView dgvVatTu = null!;

        private ComboBox cboVatTu = null!;
        private NumericUpDown numSoLuong = null!, numDonGia = null!;
        private Button btnAddItem = null!, btnSavePhieu = null!;
        private DataGridView dgvChiTiet = null!;
        private Label lblTongTien = null!;

        public frmInventoryMain()
        {
            InitializeCustomLayout(); // Đổi InitLayout() thành tên này
            LoadVatTuData();
        }

        private void InitializeCustomLayout()
        {
            this.Text = "Quản lý Xuất - Nhập Kho (Kiến trúc 3-Tier & Transaction ACID)";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5F);

            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            Label lblKw = new Label { Text = "Tìm vật tư:", Location = new Point(15, 18), AutoSize = true };
            txtSearch = new TextBox { Location = new Point(95, 15), Width = 220 };
            btnSearch = new Button { Text = "Tìm kiếm", Location = new Point(325, 14), Width = 85, Height = 30 };
            btnRefresh = new Button { Text = "Tất cả", Location = new Point(415, 14), Width = 80, Height = 30 };
            btnSearch.Click += (s, e) => { dgvVatTu.DataSource = bus.TimKiemVatTu(txtSearch.Text); };
            btnRefresh.Click += (s, e) => { LoadVatTuData(); };

            pnlTop.Controls.AddRange(new Control[] { lblKw, txtSearch, btnSearch, btnRefresh });

            GroupBox grpLeft = new GroupBox { Text = "Danh mục vật tư tồn kho", Dock = DockStyle.Left, Width = 500, Padding = new Padding(10) };
            dgvVatTu = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            grpLeft.Controls.Add(dgvVatTu);

            GroupBox grpRight = new GroupBox { Text = "Lập phiếu xuất / nhập kho", Dock = DockStyle.Fill, Padding = new Padding(10) };
            Panel pnlOrderInput = new Panel { Dock = DockStyle.Top, Height = 140 };
            radNhap = new RadioButton { Text = "Nhập Kho", Location = new Point(15, 10), Checked = true, AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            radXuat = new RadioButton { Text = "Xuất Kho", Location = new Point(130, 10), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };

            Label lblItem = new Label { Text = "Mã VT:", Location = new Point(15, 45), AutoSize = true };
            cboVatTu = new ComboBox { Location = new Point(70, 42), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblSl = new Label { Text = "SL:", Location = new Point(230, 45), AutoSize = true };
            numSoLuong = new NumericUpDown { Location = new Point(260, 42), Width = 80, Minimum = 1, Maximum = 10000, Value = 1 };

            Label lblGia = new Label { Text = "Đơn giá:", Location = new Point(350, 45), AutoSize = true };
            numDonGia = new NumericUpDown { Location = new Point(415, 42), Width = 110, Minimum = 0, Maximum = 100000000, Value = 50000 };

            btnAddItem = new Button { Text = "Thêm vào phiếu", Location = new Point(70, 85), Size = new Size(130, 35), BackColor = Color.LightSkyBlue, FlatStyle = FlatStyle.Flat };
            btnAddItem.Click += BtnAddItem_Click;

            pnlOrderInput.Controls.AddRange(new Control[] { radNhap, radXuat, lblItem, cboVatTu, lblSl, numSoLuong, lblGia, numDonGia, btnAddItem });

            dgvChiTiet = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.WhiteSmoke
            };

            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 60 };
            lblTongTien = new Label { Text = "Tổng cộng: 0 VNĐ", Location = new Point(15, 18), AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.DarkRed };
            btnSavePhieu = new Button { Text = "LƯU PHIẾU KHO", Location = new Point(360, 10), Size = new Size(160, 40), BackColor = Color.ForestGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnSavePhieu.Click += BtnSavePhieu_Click;
            pnlBottom.Controls.AddRange(new Control[] { lblTongTien, btnSavePhieu });

            grpRight.Controls.Add(dgvChiTiet);
            grpRight.Controls.Add(pnlOrderInput);
            grpRight.Controls.Add(pnlBottom);

            this.Controls.Add(grpRight);
            this.Controls.Add(grpLeft);
            this.Controls.Add(pnlTop);
        }

        private void LoadVatTuData()
        {
            try
            {
                DataTable dt = bus.GetDanhSachVatTu();

                // Xóa liên kết cũ và bật tự động gán dữ liệu vào các cột
                dgvVatTu.DataSource = null;
                dgvVatTu.AutoGenerateColumns = true;
                dgvVatTu.DataSource = dt;

                // Đổ danh sách mã vật tư vào ComboBox lập phiếu
                cboVatTu.Items.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["MaVatTu"] != null)
                    {
                        cboVatTu.Items.Add(row["MaVatTu"].ToString()!);
                    }
                }

                if (cboVatTu.Items.Count > 0)
                {
                    cboVatTu.SelectedIndex = 0;
                }

                // Kiểm tra số dòng nạp được
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Kết nối thành công nhưng bảng VatTu đang có 0 dòng dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối / truy vấn: " + ex.Message, "Lỗi nạp dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddItem_Click(object? sender, EventArgs e)
        {
            if (cboVatTu.SelectedItem == null) return;
            dsChiTiet.Add(new ChiTietPhieuDTO
            {
                MaVatTu = cboVatTu.SelectedItem.ToString()!,
                SoLuong = numSoLuong.Value,
                DonGia = numDonGia.Value
            });
            HienThiChiTiet();
        }

        private void HienThiChiTiet()
        {
            dgvChiTiet.DataSource = null;
            dgvChiTiet.DataSource = dsChiTiet;
            decimal tong = 0;
            foreach (var item in dsChiTiet) tong += item.ThanhTien;
            lblTongTien.Text = $"Tổng cộng: {tong:N0} VNĐ";
        }

        private void BtnSavePhieu_Click(object? sender, EventArgs e)
        {
            decimal tong = 0;
            foreach (var item in dsChiTiet) tong += item.ThanhTien;

            PhieuKhoDTO phieu = new PhieuKhoDTO
            {
                LoaiPhieu = radNhap.Checked ? "NHAP" : "XUAT",
                NgayLap = DateTime.Now,
                MaNCC = radNhap.Checked ? 1 : (int?)null,
                TongTien = tong,
                ChiTietList = new List<ChiTietPhieuDTO>(dsChiTiet)
            };

            var res = bus.XuLyLuuPhieu(phieu);
            if (res.Success)
            {
                MessageBox.Show(res.Message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dsChiTiet.Clear();
                HienThiChiTiet();
                LoadVatTuData();
            }
            else
            {
                MessageBox.Show(res.Message, "Giao dịch thất bại (Rollback)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}