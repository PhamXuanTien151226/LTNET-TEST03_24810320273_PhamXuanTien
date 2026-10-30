using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace InventoryApp.GUI
{
    public class frmHotelMain : Form
    {
        private readonly string connStr = "Server=localhost;Database=HotelDB;Uid=root;Pwd=;Charset=utf8mb4;";
        private DataGridView dgvRooms = null!;
        private TextBox txtCCCD = null!, txtTenKH = null!, txtSDT = null!;
        private ComboBox cboPhongTrong = null!;
        private Button btnCheckIn = null!, btnFilter = null!;
        private ComboBox cboFilterStatus = null!;

        public frmHotelMain()
        {
            BuildUI();
            LoadRooms();
            LoadAvailableRooms();
        }

        private void BuildUI()
        {
            this.Text = "Bài 5.4: Hệ thống Quản lý Tiện ích Dịch vụ & Khách sạn (Hotel Management)";
            this.Size = new Size(1050, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Bộ lọc phòng ngắt kết nối (DataView/DataTable)
            Panel pnlFilter = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(10) };
            Label lblF = new Label { Text = "Lọc trạng thái phòng:", Location = new Point(15, 18), AutoSize = true };
            cboFilterStatus = new ComboBox { Location = new Point(150, 15), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cboFilterStatus.Items.AddRange(new object[] { "Tất cả", "Trống", "Đang ở", "Đang dọn dẹp" });
            cboFilterStatus.SelectedIndex = 0;
            btnFilter = new Button { Text = "Lọc phòng", Location = new Point(310, 13), Width = 90, Height = 30 };
            btnFilter.Click += (s, e) => { FilterRooms(cboFilterStatus.SelectedItem!.ToString()!); };
            pnlFilter.Controls.AddRange(new Control[] { lblF, cboFilterStatus, btnFilter });

            // Danh sách phòng bên trái
            GroupBox grpRooms = new GroupBox { Text = "Danh mục phòng", Dock = DockStyle.Left, Width = 550, Padding = new Padding(10) };
            dgvRooms = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White };
            grpRooms.Controls.Add(dgvRooms);

            // Khung Check-in nhận phòng bên phải
            GroupBox grpCheckIn = new GroupBox { Text = "Nghiệp vụ Check-in (SqlParameter chống Injection)", Dock = DockStyle.Fill, Padding = new Padding(15) };

            Label lblC = new Label { Text = "CCCD Khách:", Location = new Point(20, 40), AutoSize = true };
            txtCCCD = new TextBox { Location = new Point(120, 37), Width = 220 };

            Label lblT = new Label { Text = "Họ và tên:", Location = new Point(20, 80), AutoSize = true };
            txtTenKH = new TextBox { Location = new Point(120, 77), Width = 220 };

            Label lblS = new Label { Text = "SĐT:", Location = new Point(20, 120), AutoSize = true };
            txtSDT = new TextBox { Location = new Point(120, 117), Width = 220 };

            Label lblP = new Label { Text = "Chọn phòng:", Location = new Point(20, 160), AutoSize = true };
            cboPhongTrong = new ComboBox { Location = new Point(120, 157), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

            btnCheckIn = new Button { Text = "CHECK-IN NHẬN PHÒNG", Location = new Point(120, 210), Size = new Size(200, 40), BackColor = Color.CornflowerBlue, ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnCheckIn.Click += BtnCheckIn_Click;

            grpCheckIn.Controls.AddRange(new Control[] { lblC, txtCCCD, lblT, txtTenKH, lblS, txtSDT, lblP, cboPhongTrong, btnCheckIn });

            this.Controls.Add(grpCheckIn);
            this.Controls.Add(grpRooms);
            this.Controls.Add(pnlFilter);
        }

        private void LoadRooms()
        {
            using MySqlConnection conn = new MySqlConnection(connStr);
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT SoPhong, LoaiPhong, GiaTheoGio, TrangThai FROM Phong", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvRooms.DataSource = dt;
        }

        private void FilterRooms(string status)
        {
            if (dgvRooms.DataSource is DataTable dt)
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = status == "Tất cả" ? "" : $"TrangThai = '{status}'";
            }
        }

        private void LoadAvailableRooms()
        {
            using MySqlConnection conn = new MySqlConnection(connStr);
            MySqlCommand cmd = new MySqlCommand("SELECT SoPhong FROM Phong WHERE TrangThai = 'Trống'", conn);
            conn.Open();
            using MySqlDataReader r = cmd.ExecuteReader();
            cboPhongTrong.Items.Clear();
            while (r.Read()) cboPhongTrong.Items.Add(r["SoPhong"].ToString()!);
            if (cboPhongTrong.Items.Count > 0) cboPhongTrong.SelectedIndex = 0;
        }

        private void BtnCheckIn_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCCCD.Text) || string.IsNullOrWhiteSpace(txtTenKH.Text) || cboPhongTrong.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đủ CCCD, Họ tên và chọn phòng trống!");
                return;
            }

            using MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            MySqlTransaction trans = conn.BeginTransaction();

            try
            {
                // 1. Kiểm tra hoặc thêm Khách hàng bằng SqlParameter chống SQL Injection
                string sqlFind = "SELECT MaKH FROM KhachHang WHERE CCCD = @CCCD";
                MySqlCommand cmdFind = new MySqlCommand(sqlFind, conn, trans);
                cmdFind.Parameters.AddWithValue("@CCCD", txtCCCD.Text.Trim());
                object? res = cmdFind.ExecuteScalar();

                int maKh;
                if (res == null)
                {
                    string sqlAddKh = "INSERT INTO KhachHang (HoTen, CCCD, SoDienThoai) VALUES (@Ten, @CCCD, @SDT); SELECT LAST_INSERT_ID();";
                    MySqlCommand cmdAdd = new MySqlCommand(sqlAddKh, conn, trans);
                    cmdAdd.Parameters.AddWithValue("@Ten", txtTenKH.Text.Trim());
                    cmdAdd.Parameters.AddWithValue("@CCCD", txtCCCD.Text.Trim());
                    cmdAdd.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                    maKh = Convert.ToInt32(cmdAdd.ExecuteScalar());
                }
                else
                {
                    maKh = Convert.ToInt32(res);
                }

                string soPhong = cboPhongTrong.SelectedItem.ToString()!;

                // 2. Lập Phiếu nhận phòng
                string sqlPhieu = "INSERT INTO PhieuDatPhong (SoPhong, MaKH, NgayNhanPhong) VALUES (@SoPhong, @MaKH, NOW())";
                MySqlCommand cmdPhieu = new MySqlCommand(sqlPhieu, conn, trans);
                cmdPhieu.Parameters.AddWithValue("@SoPhong", soPhong);
                cmdPhieu.Parameters.AddWithValue("@MaKH", maKh);
                cmdPhieu.ExecuteNonQuery();

                // 3. Đổi trạng thái phòng thành 'Đang ở'
                string sqlUpPhong = "UPDATE Phong SET TrangThai = 'Đang ở' WHERE SoPhong = @SoPhong";
                MySqlCommand cmdUp = new MySqlCommand(sqlUpPhong, conn, trans);
                cmdUp.Parameters.AddWithValue("@SoPhong", soPhong);
                cmdUp.ExecuteNonQuery();

                trans.Commit();
                MessageBox.Show($"Check-in phòng {soPhong} cho khách {txtTenKH.Text} thành công!", "Check-in hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadRooms();
                LoadAvailableRooms();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                MessageBox.Show("Lỗi giao dịch: " + ex.Message);
            }
        }
    }
}