using System;
using System.Drawing;
using System.Windows.Forms;

namespace InventoryApp.GUI
{
    public class frmMainApp : Form
    {
        public frmMainApp()
        {
            this.Text = "HỆ THỐNG BÀI TẬP THỰC HÀNH C# .NET - D19QTANM1";
            this.Size = new Size(520, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lbl = new Label
            {
                Text = "BÁO CÁO THỰC HÀNH LẬP TRÌNH .NET\nSinh viên: Phạm Xuân Tiến - 24810320273",
                Dock = DockStyle.Top,
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };

            Button btn1 = new Button
            {
                Text = "1. BÀI 5.3: HỆ THỐNG POS QUÁN CAFE",
                Location = new Point(60, 95),
                Size = new Size(380, 55),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.LightSkyBlue,
                FlatStyle = FlatStyle.Flat
            };

            Button btn2 = new Button
            {
                Text = "2. BÀI 5.4: QUẢN LÝ TIỆN ÍCH KHÁCH SẠN",
                Location = new Point(60, 165),
                Size = new Size(380, 55),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.Thistle,
                FlatStyle = FlatStyle.Flat
            };

            Button btn3 = new Button
            {
                Text = "3. BÀI 5.5: QUẢN LÝ XUẤT - NHẬP KHO",
                Location = new Point(60, 235),
                Size = new Size(380, 55),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };

            // Sự kiện khi bấm nút sẽ mở từng Form tương ứng
            btn1.Click += (s, e) => new frmCafePOS().ShowDialog();
            btn2.Click += (s, e) => new frmHotelMain().ShowDialog();
            btn3.Click += (s, e) => new frmInventoryMain().ShowDialog();

            this.Controls.AddRange(new Control[] { lbl, btn1, btn2, btn3 });
        }
    }
}