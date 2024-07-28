using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using fr_Main.UserControls;

namespace fr_Main
{
    public partial class frm_BD : Form
    {
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        public frm_BD()
        {
            InitializeComponent();
        }

        private const int CS_DropShadow = 0x00020000;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = CS_DropShadow;
                return cp;
            }
        }

        private void Load_ChartSanPham()
        {
            db.Open();

            string query = "exec TKDoanhThu";

            SqlDataReader reader = db.getDataReader(query);
            DataTable table = new DataTable();
            table.Load(reader);



            string seriesName = "Tổng doanh thu sản phẩm";
            chart1.Series.Clear();
            chart1.Series.Add(seriesName);
            chart1.Series[seriesName].XValueMember = "TenSP";
            chart1.Series[seriesName].YValueMembers = "DoanhThu";
            chart1.DataSource = table;
            chart1.DataBind();

        }

        private void frm_BD_Load(object sender, EventArgs e)
        {
            Load_ChartSanPham();
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            if (guna2RadioButton1.Checked)
            {
                db.Open();
                string time = guna2TextBoxMHH.Text;
                if (string.IsNullOrEmpty(time))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thời gian cần thống kê.");
                    return;
                }

                DateTime selectedDate;
                if (!DateTime.TryParseExact(time, "M-yyyy", null, System.Globalization.DateTimeStyles.None, out selectedDate))
                {
                    MessageBox.Show("Vui lòng nhập đúng định dạng tháng-năm (VD: 1-2023).");
                    return;
                }
                string selectedMonth = selectedDate.ToString("yyyy-MM");
                string sql = string.Format("SELECT SanPham.TenSP, SUM(SanPham.DonGia * ChiTietHoaDon.SOLUONG) AS DoanhThu " +
                                               "FROM SanPham " +
                                               "JOIN ChiTietHoaDon ON SanPham.MaSP = ChiTietHoaDon.SanPham " +
                                               "JOIN HoaDon ON ChiTietHoaDon.MaHD = HoaDon.MaHD " +
                                               "WHERE HOADON.NgayBan >= '{0}-01' AND HOADON.NgayBan < DATEADD(month, 1, '{0}-01')" +
                                               "GROUP BY SanPham.TenSP", selectedMonth);

                 SqlDataReader reader = db.getDataReader(sql);
                 DataTable table = new DataTable();
                 table.Load(reader);
                 string seriesName = "Tổng doanh thu sản phẩm trong tháng: " + time;
                 chart1.Series.Clear();
                 chart1.Series.Add(seriesName);
                 chart1.Series[seriesName].XValueMember = "TenSP";
                 chart1.Series[seriesName].YValueMembers = "DoanhThu";
                 chart1.DataSource = table;
                 chart1.DataBind();

                 db.Close();

            }
            if (guna2RadioButton2.Checked)
            {
                db.Open();

                string time = guna2TextBoxMHH.Text;
                if (string.IsNullOrEmpty(time))
                {
                    MessageBox.Show("Vui đầy đủ thời gian cần thống kê.");
                    return;
                }
                string sql = "SELECT SanPham.TenSP, SUM(SanPham.DonGia * ChiTietHoaDon.SoLuong) AS DoanhThu " +
                             "FROM SanPham " +
                             "JOIN ChiTietHoaDon ON SanPham.MaSP = ChiTietHoaDon.SanPham " +
                             "JOIN HoaDon ON ChiTietHoaDon.MaHD = HoaDon.MaHD " +
                             "WHERE YEAR(HOADON.NgayBan) = '" + time + "' " +
                             "GROUP BY SanPham.TenSP";

                SqlDataReader reader = db.getDataReader(sql);
                DataTable table = new DataTable();
                table.Load(reader);

                string seriesName = "Doanh thu sản phẩm trong năm: " + time;
                chart1.Series.Clear();
                chart1.Series.Add(seriesName);
                chart1.Series[seriesName].XValueMember = "TenSP";
                chart1.Series[seriesName].YValueMembers = "DoanhThu";
                chart1.DataSource = table;
                chart1.DataBind();

                db.Close();
            }
        }

        private void frm_BD_FormClosing(object sender, FormClosingEventArgs e)
        {
            TKE f = new TKE();
            f.Show();
            this.Hide();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
