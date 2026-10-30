using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using InventoryApp.DAL;

namespace InventoryApp.GUI
{
    public class frmCafePOS : Form
    {
        private CafeDAL dal = new CafeDAL();

        private GroupBox grpTables = null!;
        private FlowLayoutPanel pnlTables = null!;
        private DataGridView dgvBill = null!;
        private ComboBox cboMon = null!;
        private NumericUpDown numSoLuong = null!;
        private Label lblTotal = null!;
        private Button btnAddDish = null!, btnPay = null!;
        private int selectedTableId = 0;
        private string selectedTableName = "";

        public frmCafePOS()
        {
            BuildUI();
            LoadTables();
            LoadMenu();
        }

        private void BuildUI()
        {
            this.Text = "Bài 5.3: Hệ thống Quản lý Quán Cafe & Bán hàng (POS)";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Sơ đồ bàn Visual Table Layout
            grpTables = new GroupBox { Text = "Sơ đồ bàn (Xanh: Trống | Đỏ: Có khách)", Dock = DockStyle.Left, Width = 450, Padding = new Padding(10) };
            pnlTables = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };
            grpTables.Controls.Add(pnlTables);

            // Bảng hóa đơn & Order
            GroupBox grpOrder = new GroupBox { Text = "Chi tiết hóa đơn bàn được chọn", Dock = DockStyle.Fill, Padding = new Padding(10) };

            Panel pnlTopOrder = new Panel { Dock = DockStyle.Top, Height = 60 };
            Label lblM = new Label { Text = "Món:", Location = new Point(10, 18), AutoSize = true };
            cboMon = new ComboBox { Location = new Point(50, 15), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblSl = new Label { Text = "SL:", Location = new Point(240, 18), AutoSize = true };
            numSoLuong = new NumericUpDown { Location = new Point(270, 15), Width = 60, Minimum = 1, Value = 1 };
            btnAddDish = new Button { Text = "Thêm món", Location = new Point(345, 12), Width = 90, Height = 30, BackColor = Color.LightSkyBlue, FlatStyle = FlatStyle.Flat };
            btnAddDish.Click += BtnAddDish_Click;
            pnlTopOrder.Controls.AddRange(new Control[] { lblM, cboMon, lblSl, numSoLuong, btnAddDish });

            dgvBill = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White };

            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 60 };
            lblTotal = new Label { Text = "Tổng tiền: 0 VNĐ", Location = new Point(15, 18), AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.DarkRed };
            btnPay = new Button { Text = "THANH TOÁN", Location = new Point(350, 10), Width = 140, Height = 40, BackColor = Color.MediumSeaGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnPay.Click += BtnPay_Click;
            pnlBottom.Controls.AddRange(new Control[] { lblTotal, btnPay });

            grpOrder.Controls.Add(dgvBill);
            grpOrder.Controls.Add(pnlTopOrder);
            grpOrder.Controls.Add(pnlBottom);

            this.Controls.Add(grpOrder);
            this.Controls.Add(grpTables);
        }

        private void LoadTables()
        {
            pnlTables.Controls.Clear();
            DataTable dt = dal.GetDanhSachBan();
            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["MaBan"]);
                string name = row["TenBan"].ToString()!;
                string status = row["TrangThai"].ToString()!;

                Button btn = new Button
                {
                    Width = 95,
                    Height = 95,
                    Text = $"{name}\n({status})",
                    Tag = id,
                    BackColor = status == "Trống" ? Color.LightGreen : Color.LightCoral,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Margin = new Padding(8)
                };
                btn.Click += (s, e) =>
                {
                    selectedTableId = id;
                    selectedTableName = name;
                    grpTables.Text = $"Đang chọn: {name}";
                    LoadBillForTable(id);
                };
                pnlTables.Controls.Add(btn);
            }
        }

        private void LoadMenu()
        {
            DataTable dt = dal.GetDanhSachMon();
            cboMon.DataSource = dt;
            cboMon.DisplayMember = "TenMon";
            cboMon.ValueMember = "MaMon";
        }

        private void LoadBillForTable(int maBan)
        {
            DataTable dt = dal.GetChiTietHoaDonHienTai(maBan);
            dgvBill.DataSource = dt;
            decimal total = 0;
            foreach (DataRow row in dt.Rows)
            {
                total += Convert.ToDecimal(row["ThanhTien"]);
            }
            lblTotal.Text = $"Tổng tiền: {total:N0} VNĐ";
        }

        private void BtnAddDish_Click(object? sender, EventArgs e)
        {
            if (selectedTableId == 0)
            {
                MessageBox.Show("Vui lòng nhấp chọn một bàn trước khi gọi món!");
                return;
            }

            int maMon = Convert.ToInt32(cboMon.SelectedValue);
            DataRowView row = (DataRowView)cboMon.SelectedItem!;
            decimal gia = Convert.ToDecimal(row["DonGia"]);

            if (dal.GoiMon(selectedTableId, maMon, (int)numSoLuong.Value, gia))
            {
                LoadTables(); // Cập nhật lại màu bàn (chuyển sang đỏ)
                LoadBillForTable(selectedTableId);
            }
        }

        private void BtnPay_Click(object? sender, EventArgs e)
        {
            if (selectedTableId == 0 || dgvBill.Rows.Count == 0)
            {
                MessageBox.Show("Bàn chưa có món hoặc chưa chọn bàn!");
                return;
            }

            decimal total = 0;
            foreach (DataGridViewRow row in dgvBill.Rows)
            {
                total += Convert.ToDecimal(row.Cells["ThanhTien"].Value);
            }

            if (MessageBox.Show($"Xác nhận thanh toán cho {selectedTableName} tổng số tiền {total:N0} VNĐ?", "Thanh toán", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (dal.ThanhToan(selectedTableId, total))
                {
                    MessageBox.Show("Thanh toán thành công! Trạng thái bàn đã chuyển về Trống.");
                    LoadTables(); // Nút bàn đổi lại màu xanh
                    LoadBillForTable(selectedTableId);
                }
            }
        }
    }
}