using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace fr_Main.UserControls
{
    public partial class TKE : UserControl
    {
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        public TKE()
        {
            InitializeComponent();
        }



        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (guna2RadioButton1.Checked || guna2RadioButton2.Checked)
            {
                db.Open();
                string maHH = guna2TextBoxDG.Text;
                string tenHH = guna2TextBoxMHH.Text;
                string time = guna2TextBoxTHH.Text;

                if (string.IsNullOrEmpty(maHH) || string.IsNullOrEmpty(tenHH))
                {
                    MessageBox.Show("Vui lòng nhập mã và tên hàng hóa cần thống kê.");
                    db.Close();
                    return;
                }

                string sql = string.Empty;

                if (guna2RadioButton1.Checked)
                {
                    DateTime selectedDate;
                    if (!DateTime.TryParseExact(time, "M-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out selectedDate))
                    {
                        MessageBox.Show("Vui lòng nhập đúng định dạng tháng-năm (VD: 1-2023).");
                        db.Close();
                        return;
                    }

                    string selectedMonth = selectedDate.ToString("yyyy-MM");

                    sql = "SELECT SUM(ChiTietHoaDon.SoLuong * SanPham.DonGia) AS TongDoanhThu " +
                          "FROM ChiTietHoaDon " +
                          "JOIN HOADON ON ChiTietHoaDon.MaHD = HOADON.MaHD " +
                          "JOIN SanPham ON ChiTietHoaDon.SanPham = SanPham.MaSP " +
                          "WHERE SanPham.MaSP = '" + maHH + "' AND SanPham.TenSP = N'" + tenHH + "' " +
                          "AND HOADON.NgayBan >= '" + selectedMonth + "-01' AND HOADON.NgayBan < DATEADD(month, 1, '" + selectedMonth + "-01');";
                }

                else if (guna2RadioButton2.Checked)
                {
                    int selectedYear;
                    if (!int.TryParse(time, out selectedYear))
                    {
                        MessageBox.Show("Vui lòng nhập đúng định dạng năm (VD: 2023).");
                        db.Close();
                        return;
                    }

                    sql = "SELECT SUM(ChiTietHoaDon.SoLuong * SanPham.DonGia) AS TongDoanhThu " +
                          "FROM ChiTietHoaDon " +
                          "JOIN HOADON ON ChiTietHoaDon.MaHD = HOADON.MaHD " +
                          "JOIN SanPham ON ChiTietHoaDon.SanPham = SanPham.MaSP " +
                          "WHERE SanPham.MaSP = '" + maHH + "' AND SanPham.TenSP = N'" + tenHH + "' " +
                          "AND YEAR(HOADON.NgayBan) = " + selectedYear + ";";
                }

                SqlDataReader reader = db.getDataReader(sql);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        decimal totalDoanhThu = Convert.ToDecimal(reader.GetValue(0));
                        MessageBox.Show("Tổng số doanh thu của sản phẩm " + tenHH + " trong " + time + ": " + totalDoanhThu);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm cần thống kê.");
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm cần thống kê.");
                }

                db.Close();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn loại thống kê.");
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            frm_BD f = new frm_BD();
            f.ShowDialog();
        }

        private string GetQueryForChart(string loaiThongKe)
        {
            string maHH = guna2TextBoxDG.Text;
            string tenHH = guna2TextBoxMHH.Text;
            string time = guna2TextBoxTHH.Text;

            string sql = string.Empty;

            if (loaiThongKe == "thang")
            {
                DateTime selectedDate;
                if (DateTime.TryParseExact(time, "M-yyyy", null, DateTimeStyles.None, out selectedDate))
                {
                    string selectedMonth = selectedDate.ToString("yyyy-MM");
                    sql = "SELECT HOADON.NgayBan, SUM(ChiTietHoaDon.SoLuong * SanPham.DonGia) AS TongDoanhThu " +
                          "FROM ChiTietHoaDon " +
                          "JOIN HOADON ON ChiTietHoaDon.MaHD = HOADON.MaHD " +
                          "JOIN SanPham ON ChiTietHoaDon.SanPham = SanPham.MaSP " +
                          "WHERE SanPham.MaSP = '" + maHH + "' AND SanPham.TenSP = N'" + tenHH + "' " +
                          "AND HOADON.NgayBan >= '" + selectedMonth + "-01' AND HOADON.NgayBan < DATEADD(month, 1, '" + selectedMonth + "-01') " +
                          "GROUP BY HOADON.NgayBan;";
                }
            }
            else if (loaiThongKe == "nam")
            {
                int selectedYear;
                if (int.TryParse(time, out selectedYear))
                {
                    sql = "SELECT YEAR(HOADON.NgayBan) AS Nam, SUM(ChiTietHoaDon.SoLuong * SanPham.DonGia) AS TongDoanhThu " +
                          "FROM ChiTietHoaDon " +
                          "JOIN HOADON ON ChiTietHoaDon.MaHD = HOADON.MaHD " +
                          "JOIN SanPham ON ChiTietHoaDon.SanPham = SanPham.MaSP " +
                          "WHERE SanPham.MaSP = '" + maHH + "' AND SanPham.TenSP = N'" + tenHH + "' " +
                          "AND YEAR(HOADON.NgayBan) = " + selectedYear + " " +
                          "GROUP BY YEAR(HOADON.NgayBan);";
                }
            }
            return sql;
        }

        private void guna2DataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int indexSelect = e.RowIndex;
            DataGridViewRow data = guna2DataGridView3.Rows[indexSelect];
            guna2TextBoxDG.Text = data.Cells[0].Value.ToString();
            guna2TextBoxMHH.Text = data.Cells[1].Value.ToString();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = guna2DataGridView1.CurrentCell.RowIndex;
            int MAHD = (int)guna2DataGridView1.Rows[rowIndex].Cells[0].Value;
            string strCTHD = "select * from ChiTietHoaDon WHERE MaHD= '" + MAHD + "'";

            SqlDataReader readerHD = db.getDataReader(strCTHD);
            DataTable tableHD = new DataTable();
            tableHD.Load(readerHD);
            guna2DataGridView2.DataSource = tableHD;
        }

        private void guna2DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            string str = "select *from HOADON WHERE NGAYHD = '" + guna2DateTimePicker1.Value.ToShortDateString() + "'";
            SqlDataReader reader = db.getDataReader(str);
            DataTable table = new DataTable();
            table.Load(reader);
            guna2DataGridView1.DataSource = table;
        }

        private void TKE_Load(object sender, EventArgs e)
        {
            string str = "select *from HOADON";
            SqlDataReader reader = db.getDataReader(str);
            DataTable table = new DataTable();
            table.Load(reader);
            guna2DataGridView1.DataSource = table;

            string str1 = "select *from SanPham";
            SqlDataReader reader1 = db.getDataReader(str1);
            DataTable table1 = new DataTable();
            table1.Load(reader1);
            guna2DataGridView3.DataSource = table1;

            string strHH = "exec TTSP";
            SqlDataReader readerHH = db.getDataReader(strHH);
            DataTable tableHH = new DataTable();
            tableHH.Load(readerHH);

            guna2DataGridView1.Columns["MaHD"].HeaderText = "Mã HĐ";
            guna2DataGridView1.Columns["NgayBan"].HeaderText = "Ngày Bán";
            guna2DataGridView1.Columns["TongTien"].HeaderText = "Tổng Tiền";
            guna2DataGridView1.Columns["MaNV"].HeaderText = "Mã NV";
        }
    }
}
