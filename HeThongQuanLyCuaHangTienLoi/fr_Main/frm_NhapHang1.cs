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
using System.Security.Cryptography;

namespace fr_Main
{
    public partial class frm_NhapHang1 : Form
    {
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        public frm_NhapHang1()
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

        void LoadDL()
        {
            string str = "SELECT TOP 1 MaNhapHang FROM NhapHang ORDER BY MaNhapHang DESC";
            int manh = (int)db.getScalar(str) + 1;
            guna2TextBoxMHH.Text = manh.ToString();
            guna2TextBoxMHH.ReadOnly = true;
            string strMaNCC = "select * from NhaCungCap";
            DataTable tableNCC = db.getDataTable(strMaNCC);
            guna2ComboBoxDVT.DataSource = tableNCC;
            guna2ComboBoxDVT.DisplayMember = "TenNCC";
            guna2ComboBoxDVT.ValueMember = "MaNCC";

            string strMaHH = "select * from SanPham";
            DataTable tableMaHH = db.getDataTable(strMaHH);
            guna2ComboBox2.DataSource = tableMaHH;
            guna2ComboBox2.DisplayMember = "TenSP";
            guna2ComboBox2.ValueMember = "MaSP";
            guna2DataGridView1.DataSource = tableMaHH;

            guna2DataGridView2.Rows.Clear();
            guna2DateTimePicker1.Enabled = true;
            guna2DateTimePicker1.Value = DateTime.Today;

            guna2TextBox1.Text = string.Empty;
        }

        private void frm_NhapHang1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            LoadDL();
            guna2DataGridView1.Columns[0].HeaderText = "Mã SP";
            guna2DataGridView1.Columns[1].HeaderText = "Tên Sản Phẩm";
            guna2DataGridView1.Columns[2].HeaderText = "Giá Bán";
            guna2DataGridView1.Columns[3].HeaderText = "Đơn Vị Tính";
            guna2DataGridView1.Columns[4].HeaderText = "Số Lượng";
            guna2DataGridView1.Columns[5].HeaderText = "Danh Mục";
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(guna2TextBox1.Text) || (int)guna2NumericUpDown1.Value == 0)
            {
                MessageBox.Show("Số lượng hoặc giá nhập không hợp lệ!");
                return;
            }
            else
            {
                int MAPN = int.Parse(guna2TextBoxMHH.Text);
                int MAHANG = (int)guna2ComboBox2.SelectedValue;
                double GIA = double.Parse(guna2TextBox1.Text);
                int SOLUONG = (int)guna2NumericUpDown1.Value;
                string ThanhTien = (SOLUONG * GIA).ToString();

                object[] rowValue = { MAPN, MAHANG, SOLUONG, GIA };
                guna2DataGridView2.Rows.Add(rowValue);
                CapNhatThanhTien();
                guna2TrackBar1.Value = guna2TrackBar1.Minimum;
                guna2TextBox1.Text = string.Empty;
                guna2NumericUpDown1.Value = 0;
                guna2TrackBar1.Cursor = Cursors.Hand;
            }
        }

        private bool IsDataGridViewEmpty(DataGridView dgv)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                    return false;
            }
            return true;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

            if (IsDataGridViewEmpty(guna2DataGridView2))
            {
                MessageBox.Show("Không có dữ liệu để tạo hóa đơn.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DateTime ngayHD = DateTime.Parse(guna2DateTimePicker1.Text);
            DateTime ngayHienTai = DateTime.Today;
            if (ngayHD < ngayHienTai)
            {
                MessageBox.Show("Ngày không hợp lệ.");
                return;
            }
            try
            {
                string ngayHDFormatted = ngayHD.ToString("yyyy-MM-dd");
                string strHoaDon = "INSERT INTO NhapHang VALUES('" + int.Parse(guna2TextBoxMHH.Text) + "','" + ngayHDFormatted + "','" + 0 + "','" + (int)guna2ComboBoxDVT.SelectedValue + "','" + ShareVar.MaNV + "')";
                db.GetNonQuery(strHoaDon);
                foreach (DataGridViewRow row in guna2DataGridView2.Rows)
                {
                    string str = "SELECT TOP 1 MaCTNH FROM ChiTietNhapHang ORDER BY MaCTNH DESC";
                    int mact = (int)db.getScalar(str) + 1;

                    string strCTPN = string.Empty;
                    if (!row.IsNewRow)
                    {
                        int MaPN = int.Parse(guna2TextBoxMHH.Text);
                        object MAPN = row.Cells[0].Value;
                        object MAHANG = row.Cells[1].Value;
                        object SOLUONG = row.Cells[2].Value;
                        object GIA = row.Cells[3].Value;
                        strCTPN = "INSERT INTO ChiTietNhapHang VALUES('" + mact + "','" + SOLUONG + "','" + GIA + "',' " + MAHANG + "','" + MaPN + "')";
                        db.GetNonQuery(strCTPN);
                    }
                }
                MessageBox.Show("Thêm phiếu nhập thành công!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                guna2TextBox5.Clear();
                LoadDL();
            }
            catch
            {
                MessageBox.Show("Mã nhân viên trùng hoặc kiểu dữ liệu không chính xác!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            guna2TextBox1.Text = (guna2TrackBar1.Value * 1500).ToString();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView2.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in guna2DataGridView2.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        guna2DataGridView2.Rows.Remove(row);
                        MessageBox.Show("Xoá sản phẩm thành công!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                    }
                    if (guna2DataGridView2.Rows.Count == 1 && guna2DataGridView2.Rows[0].IsNewRow)
                    {
                        guna2DataGridView2.Rows.Clear();
                    }
                    CapNhatThanhTien();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CapNhatThanhTien()
        {
            double tongThanhTien = 0;
            foreach (DataGridViewRow row in guna2DataGridView2.Rows)
            {
                if (row.Cells[2].Value != null && row.Cells[3].Value != null)
                {
                    try
                    {
                        int soluong = Convert.ToInt32(row.Cells[2].Value);
                        double dongia = Convert.ToDouble(row.Cells[3].Value);

                        tongThanhTien += soluong * dongia;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi khi tính toán: " + ex.Message);
                        return;
                    }
                }
            }
            guna2TextBox5.Text = tongThanhTien.ToString("N0") + " VNĐ";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Opacity += .2;
        }
    }
}
