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
using System.Windows.Forms.DataVisualization.Charting;

namespace fr_Main.UserControls
{
    public partial class TQ : UserControl
    {
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        public TQ()
        {
            InitializeComponent();
        }

        private void TQ_Load(object sender, EventArgs e)
        {
            HienThiTongSoDonHang();
            HienThiTongSoSP();
            HienThiTongSoNV();
            HienThiTongDoanhThu();
            HienThiTongSoDonHangTrongThang();
            HienThiDoanhThuThangNay();
            HienThiDonHangGanDay();
            HienThiSanPhamGanHetHang();
            ShowTop5SanPham();
            Doanhthunam();
            guna2DataGridView1.Columns["MaHD"].HeaderText = "Mã HD";
            guna2DataGridView1.Columns["Ngayban"].HeaderText = "Ngày Bán";
            guna2DataGridView1.Columns["Tongtien"].HeaderText = "Tổng Tiền";
            guna2DataGridView1.Columns["MaNV"].HeaderText = "Mã NV";

            guna2DataGridView2.Columns["MaSP"].HeaderText = "Mã SP";
            guna2DataGridView2.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
            guna2DataGridView2.Columns["SoLuong"].HeaderText = "Số Lượng";
        }

        private int TinhTongSoDonHang()
        {
            string query = "SELECT COUNT(*) FROM HoaDon";
            int tongSoDonHang = Convert.ToInt32(db.getScalar(query));
            return tongSoDonHang;
        }
        private void HienThiTongSoDonHang()
        {
            int tongSoDonHang = TinhTongSoDonHang();
            label2.Text = $"{tongSoDonHang}";
        }

        private int TinhTongSoSP()
        {
            string query = "SELECT COUNT(*) FROM SanPham";
            int tongSoSP = Convert.ToInt32(db.getScalar(query));
            return tongSoSP;
        }
        private void HienThiTongSoSP()
        {
            int tongSoSP = TinhTongSoSP();
            label6.Text = $"{tongSoSP}";
        }
        private int TinhTongSoNV()
        {
            string query = "SELECT COUNT(*) FROM NhanVien";
            int tongSoNV = Convert.ToInt32(db.getScalar(query));
            return tongSoNV;
        }
        private void HienThiTongSoNV()
        {
            int tongSoNV = TinhTongSoNV();
            label8.Text = $"{tongSoNV}";
        }

        private int TinhTongDoanhThu()
        {
            string query = "SELECT SUM(TongTien) FROM HoaDon";
            decimal tongTien = Convert.ToDecimal(db.getScalar(query));
            return (int)tongTien;
        }
        private void HienThiTongDoanhThu()
        {
            int tongDoanhThu = TinhTongDoanhThu();
            string formattedTongDoanhThu = string.Format("{0:N0} VNĐ", tongDoanhThu);
            label7.Text = formattedTongDoanhThu;
        }

        private int TinhTongSoDonHangTrongThang()
        {
            DateTime ngayDauThangHienTai = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime ngayDauThangTiepTheo = ngayDauThangHienTai.AddMonths(1);

            string query = $"SELECT COUNT(*) FROM HoaDon WHERE NgayBan >= '{ngayDauThangHienTai}' AND NgayBan < '{ngayDauThangTiepTheo}'";

            int tongSoDonHangTrongThang = Convert.ToInt32(db.getScalar(query));
            return tongSoDonHangTrongThang;
        }
        private void HienThiTongSoDonHangTrongThang()
        {
            int tongSoDonHangTrongThang = TinhTongSoDonHangTrongThang();
            label10.Text = $"{tongSoDonHangTrongThang}";
        }

        private decimal TinhDoanhThuThangNay()
        {
            DateTime ngayDauThangHienTai = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime ngayDauThangTiepTheo = ngayDauThangHienTai.AddMonths(1);

            string query = $"SELECT SUM(TongTien) FROM HoaDon WHERE NgayBan >= '{ngayDauThangHienTai:yyyy-MM-dd}' AND NgayBan < '{ngayDauThangTiepTheo:yyyy-MM-dd}'";

            object result = db.getScalar(query);
            decimal doanhThuThangNay = (result != DBNull.Value && result != null) ? Convert.ToDecimal(result) : 0m;
            return doanhThuThangNay;

        }
        private void HienThiDoanhThuThangNay()
        {
            decimal doanhThuThangNay = TinhDoanhThuThangNay();
            string formattedDoanhThu = string.Format("{0:N0} VNĐ", doanhThuThangNay);
            label11.Text = $"{formattedDoanhThu}";
        }

        private void HienThiDonHangGanDay()
        {
            string query = "SELECT TOP 4 * FROM HoaDon ORDER BY NgayBan DESC";
            SqlDataReader reader = db.getDataReader(query);
            DataTable table = new DataTable();
            table.Load(reader);
            guna2DataGridView1.DataSource = table;
        }

        private void HienThiSanPhamGanHetHang()
        {
            string query = "SELECT MaSP, TenSP, SoLuong FROM SanPham WHERE SoLuong < 10";
            SqlDataReader reader = db.getDataReader(query);
            DataTable table = new DataTable();
            table.Load(reader);

            table.DefaultView.Sort = "SoLuong ASC";
            table = table.DefaultView.ToTable();

            guna2DataGridView2.DataSource = table;
        }
        private void Doanhthunam()
        {
            db.Open();

            string time = DateTime.Now.ToString("yyyy");

            string sql = string.Format("SELECT MONTH(HoaDon.NgayBan) AS Thang, SUM(HoaDon.TongTien) AS DoanhThu " +
                                       "FROM HoaDon " +
                                       "WHERE YEAR(HoaDon.NgayBan) = '{0}' " +
                                       "GROUP BY MONTH(HoaDon.NgayBan)", time);

            SqlDataReader reader = db.getDataReader(sql);
            DataTable table = new DataTable();
            table.Load(reader);

            string seriesName = "Doanh thu trong năm " + time;
            chart1.Series.Clear();
            chart1.Series.Add(seriesName);
            chart1.Series[seriesName].ChartType = SeriesChartType.Column;

            for (int i = 1; i <= 12; i++)
            {
                DataRow[] rows = table.Select($"Thang = {i}");
                if (rows.Length > 0)
                {
                    double doanhThuThang = Convert.ToDouble(rows[0]["DoanhThu"]);
                    chart1.Series[seriesName].Points.AddXY(i, doanhThuThang);
                }
                else
                {
                    chart1.Series[seriesName].Points.AddXY(i, 0);
                }
            }

            chart1.DataBind();

            db.Close();
        }

        private void Doanhthuthang()
        {
            db.Open();

            string time = DateTime.Now.ToString("yyyy-MM");

            string sql = string.Format("SELECT MONTH(HoaDon.NgayBan) AS Thang, SUM(HoaDon.TongTien) AS DoanhThu " +
                                       "FROM HoaDon " +
                                       "WHERE HOADON.NgayBan >= '{0}-01' AND HOADON.NgayBan < DATEADD(month, 1, '{0}-01') " +
                                       "GROUP BY MONTH(HoaDon.NgayBan)", time);

            SqlDataReader reader = db.getDataReader(sql);
            DataTable table = new DataTable();
            table.Load(reader);

            string seriesName = "Doanh thu tháng này";
            chart1.Series.Clear();
            chart1.Series.Add(seriesName);
            chart1.Series[seriesName].ChartType = SeriesChartType.Column;
            chart1.Series[seriesName].XValueMember = "Thang";
            chart1.Series[seriesName].YValueMembers = "DoanhThu";
            chart1.DataSource = table;
            chart1.DataBind();

            db.Close();
        }

        private void guna2RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Doanhthuthang();
        }

        private void guna2RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Doanhthunam();
        }

        private void ShowTop5SanPham()
        {
            db.Open();

            string sql = "SELECT TOP 5 SanPham.TenSP, SanPham.SoLuong AS SoLuong " +
                         "FROM SanPham " +
                         "ORDER BY SanPham.SoLuong DESC";

            SqlDataReader reader = db.getDataReader(sql);
            DataTable table = new DataTable();
            table.Load(reader);

            chart2.Series.Clear();
            chart2.Palette = ChartColorPalette.Pastel;

            string seriesName = "Top 5 sản phẩm có số lượng nhiều nhất";
            chart2.Series.Add(seriesName);
            chart2.Series[seriesName].ChartType = SeriesChartType.Doughnut;

            chart2.Series[seriesName].Font = new Font("Arial", 12, FontStyle.Bold);

            foreach (DataRow row in table.Rows)
            {
                string tenSP = row["TenSP"].ToString();
                int soLuong = Convert.ToInt32(row["SoLuong"]);

                DataPoint dataPoint = new DataPoint();
                dataPoint.AxisLabel = tenSP;
                dataPoint.YValues = new double[] { soLuong };
                chart2.Series[seriesName].Points.Add(dataPoint);
                dataPoint.LabelForeColor = Color.White;
            }
            foreach (var point in chart2.Series[seriesName].Points)
            {
                point.LegendText = point.AxisLabel;
                point.IsValueShownAsLabel = true;
            }
            db.Close();
        }

        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == guna2DataGridView1.Columns["Tongtien"].Index && e.RowIndex != -1)
            {
                if (e.Value != null && e.Value != DBNull.Value)
                {
                    decimal tongTien = Convert.ToDecimal(e.Value);
                    e.Value = tongTien.ToString("#,##0");
                    e.FormattingApplied = true;
                }
            }
        }
    }
}
