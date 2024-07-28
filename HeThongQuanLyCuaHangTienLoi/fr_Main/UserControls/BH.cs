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
using Guna.UI2.WinForms;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

namespace fr_Main.UserControls
{
    public partial class BH : UserControl
    {
        TaiKhoan tk = new TaiKhoan();
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        DataTable dt = new DataTable();

        public BH(int MaNV, int VaiTro)
        {
            tk.MaNV1 = MaNV;
            tk.Quyen1 = VaiTro;
            InitializeComponent();
        }

        public BH()
        {
            InitializeComponent();
        }

        void LoadCombobox()
        {
            string dvt = "select * from DonViTinh";
            DataTable dt = db.getDataTable(dvt);
            guna2ComboBoxDVT.DataSource = dt;
            guna2ComboBoxDVT.DisplayMember = "TenDonVi";
            guna2ComboBoxDVT.ValueMember = "MaDonVi";

            string dm = "select * from DanhMuc";
            DataTable dt1 = db.getDataTable(dm);
            guna2ComboBoxDM.DataSource = dt1;
            guna2ComboBoxDM.DisplayMember = "TenDanhMuc";
            guna2ComboBoxDM.ValueMember = "MaDanhMuc";

            DataTable dt2 = db.getDataTable(dm);
            guna2ComboBox1.DataSource = dt2;
            guna2ComboBox1.ValueMember = "MaDanhMuc";
        }

        public void LamMoi()
        {
            string str1 = "Select TOP 1 MaHD from HoaDon ORDER BY MaHD DESC";
            int mahd = (int)db.getScalar(str1) + 1;
            guna2TextBox1.Text = mahd.ToString();
            LoadCombobox();
            string str = "select *from SanPham";
            SqlDataReader reader = db.getDataReader(str);
            DataTable table = new DataTable();
            table.Load(reader);
            guna2DataGridView1.DataSource = table;

            guna2TextBox1.ReadOnly = true;
            guna2ComboBoxDM.Enabled = guna2ComboBoxDVT.Enabled = false;
            guna2TextBoxMHH.Enabled = false;
            guna2TextBoxTHH.Enabled = false;
            guna2TextBoxDG.Enabled = false;
            guna2TextBox4.Text = tk.MaNV1.ToString();

            DataTable dt = (DataTable)guna2DataGridView1.DataSource;
            LKDL(dt);
            guna2TextBox5.Clear();
            guna2DataGridView2.Rows.Clear();
        }

        private void BH_Load(object sender, EventArgs e)
        {
            string str1 = "Select TOP 1 MaHD from HoaDon ORDER BY MaHD DESC";
            int mahd = (int)db.getScalar(str1) + 1;
            guna2TextBox1.Text = mahd.ToString();
            LoadCombobox();
            guna2ComboBox1.SelectedValue = -1;
            guna2ComboBox1.SelectedValue = -1;
            string str = "select *from SanPham";
            SqlDataReader reader = db.getDataReader(str);
            DataTable table = new DataTable();
            table.Load(reader);
            guna2DataGridView1.DataSource = table;

            guna2TextBox1.ReadOnly = true;
            guna2ComboBoxDM.Enabled = guna2ComboBoxDVT.Enabled = false;
            guna2TextBoxMHH.Enabled = false;
            guna2TextBoxTHH.Enabled = false;
            guna2TextBoxDG.Enabled = false;
            guna2TextBox4.Text = tk.MaNV1.ToString();

            DataTable dt = (DataTable)guna2DataGridView1.DataSource;
            LKDL(dt);
            guna2DataGridView1.Columns["MaSP"].HeaderText = "Mã SP";
            guna2DataGridView1.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
            guna2DataGridView1.Columns["DonGia"].HeaderText = "Đơn Giá";
            guna2DataGridView1.Columns["MaDonVi"].HeaderText = "Mã Đơn Vị";
            guna2DataGridView1.Columns["SoLuong"].HeaderText = "Số Lượng";
            guna2DataGridView1.Columns["MaDanhMuc"].HeaderText = "Mã Danh Mục";
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int indexSelect = e.RowIndex;
            DataGridViewRow data = guna2DataGridView1.Rows[indexSelect];
            guna2TextBoxMHH.Text = data.Cells[0].Value.ToString();
            guna2TextBoxTHH.Text = data.Cells[1].Value.ToString();
            guna2TextBoxDG.Text = data.Cells[2].Value.ToString();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            guna2DataGridView2.AllowUserToAddRows = false;
            int maSP = int.Parse(guna2TextBoxMHH.Text);
            string tenSP = guna2TextBoxTHH.Text;

            double test = double.Parse(guna2KhuyenMai.Text);

            double donGia = 0;
            if (test > 0)
            {
                donGia = double.Parse(guna2TextBoxDG.Text) - (double.Parse(guna2TextBoxDG.Text) * test / 100);
            }
            else
            {
                donGia = double.Parse(guna2TextBoxDG.Text);
            }

            string donVT = guna2ComboBoxDVT.Text;
            int slMua = (int)guna2NumericUpDown1.Value;
            int slTon = (int)db.getScalar("select SoLuong from SanPham where MaSP = '" + int.Parse(guna2TextBoxMHH.Text) + "'");
            if (slMua == 0)
            {
                MessageBox.Show("Số lượng mua không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            else
            {
                if (slMua > slTon)
                {
                    DialogResult result = MessageBox.Show("Số lượng hàng trong kho không đủ. Bạn có muốn nhập thêm hàng không?", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        frm_NhapHang1 fromnhaphang = new frm_NhapHang1();
                        fromnhaphang.ShowDialog();
                    }
                }
                else
                {
                    try
                    {
                        object[] rowValue = { maSP, tenSP, donGia, donVT, slMua };
                        guna2DataGridView2.Rows.Add(rowValue);
                        CapNhatThanhTien();
                        guna2NumericUpDown1.Value = 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ngoại lệ: " + ex.Message);
                        MessageBox.Show("Hết hàng!! Vui lòng chọn sản phẩm khác\n\n" + ex.Message, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }

                }
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

        public void reset()
        {
            string str = "select * from SanPham";
            SqlDataReader reader = db.getDataReader(str);
            DataTable table = new DataTable();
            table.Load(reader);
            guna2DataGridView1.DataSource = table;
            LKDL(table);
        }

        private DataTable GetDataTableFromDGV(Guna.UI2.WinForms.Guna2DataGridView dgv)
        {

            var dt = new DataTable();
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                Type columnType = column.ValueType ?? typeof(string);
                dt.Columns.Add(column.Name, columnType);
            }
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        dr[i] = row.Cells[i].Value ?? DBNull.Value;
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

            if (IsDataGridViewEmpty(guna2DataGridView2))
            {
                MessageBox.Show("Không có dữ liệu để tạo hóa đơn.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                DataTable dt = GetDataTableFromDGV(guna2DataGridView2);
                string MaHD = guna2TextBox1.Text;
                string MaNV = guna2TextBox4.Text;
                string TongTien = guna2TextBox5.Text;
                frm_HoaDon f = new frm_HoaDon(this, dt, MaHD, MaNV, TongTien);
                f.ShowDialog();
            }
        }

        void ClearBindings()
        {
            guna2TextBoxMHH.DataBindings.Clear();
            guna2TextBoxTHH.DataBindings.Clear();
            guna2TextBoxDG.DataBindings.Clear();
        }

        void LKDL(DataTable pDT)
        {
            ClearBindings();
            guna2TextBoxMHH.DataBindings.Add("Text", pDT, "MaSP");
            guna2TextBoxTHH.DataBindings.Add("Text", pDT, "TenSP");
            guna2TextBoxDG.DataBindings.Add("Text", pDT, "DonGia");
        }

        private void guna2DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                object selectedMaDonVi = guna2DataGridView1.SelectedRows[0].Cells["MaDonVi"].Value;
                guna2ComboBoxDVT.SelectedValue = selectedMaDonVi;

                object selectedMDanhMuc = guna2DataGridView1.SelectedRows[0].Cells["MaDanhMuc"].Value;
                guna2ComboBoxDM.SelectedValue = selectedMDanhMuc;

                DataGridViewRow selectedRow = guna2DataGridView1.SelectedRows[0];
                int maSP = (int)selectedRow.Cells[0].Value;
                int kt = (int)db.getScalar("select count(*) from GiamGia where MaSP = '" + maSP + "'");
                if (kt > 0)
                {
                    double phanTram = Convert.ToDouble(db.getScalar("select TOP 1 PhanTramGiamGia from GiamGia where MaSP = '" + maSP + "' and CONVERT(date, GETDATE()) >= NgayBD and CONVERT(date, GETDATE()) <= NgayKT ORDER BY PhanTramGiamGia DESC"));
                    guna2KhuyenMai.Text = phanTram.ToString();
                }
                else
                {
                    guna2KhuyenMai.Text = 0.ToString();
                }
            }
        }

        private void guna2TextBox2_Leave(object sender, EventArgs e)
        {
            guna2TextBox2.Clear();

            if (guna2TextBox2.Text == "")
            {
                guna2TextBox2.Text = "Tên, mã sản phẩm";
                guna2TextBox2.ForeColor = Color.Silver;
            }
            reset();
        }

        private void guna2TextBox2_Enter(object sender, EventArgs e)
        {
            if (guna2TextBox2.Text == "Tên, mã sản phẩm")
            {
                guna2TextBox2.Text = "";
                guna2TextBox2.ForeColor = Color.Black;
            }
            guna2ComboBox1.SelectedValue = -1;
            guna2ComboBox1.SelectedValue = -1;
            guna2ComboBox2.SelectedIndex = -1;
        }

        private void guna2ComboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            guna2DataGridView1.DataSource = null;

            guna2ComboBox1.SelectedValue = -1;
            guna2ComboBox1.SelectedValue = -1;

            string chon = guna2ComboBox2.SelectedItem.ToString();

            if (chon == "Giá giảm dần")
            {
                string str = "select * from SanPham ORDER BY DonGia DESC";
                SqlDataReader reader = db.getDataReader(str);
                DataTable table = new DataTable();
                table.Load(reader);
                guna2DataGridView1.DataSource = table;
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);

            }
            else if (chon == "Giá tăng dần")
            {
                string str = "select * from SanPham ORDER BY DonGia ASC";
                SqlDataReader reader = db.getDataReader(str);
                DataTable table = new DataTable();
                table.Load(reader);
                guna2DataGridView1.DataSource = table;
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);
            }
            else if (chon == "Số lượng tăng dần")
            {
                string str = "select * from SanPham ORDER BY SoLuong ASC";
                SqlDataReader reader = db.getDataReader(str);
                DataTable table = new DataTable();
                table.Load(reader);
                guna2DataGridView1.DataSource = table;
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);
            }

            else if (chon == "Số lượng giảm dần")
            {
                string str = "select * from SanPham ORDER BY SoLuong DESC";
                SqlDataReader reader = db.getDataReader(str);
                DataTable table = new DataTable();
                table.Load(reader);
                guna2DataGridView1.DataSource = table;
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);
            }
        }

        private void guna2ComboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            guna2DataGridView1.DataSource = null;
            guna2ComboBox2.SelectedIndex = -1;
            if (guna2ComboBox1.SelectedValue != null)
            {
                int madm = (int)guna2ComboBox1.SelectedValue;
                string str = "select * from SanPham where MaDanhMuc = '" + madm + "' ";
                SqlDataReader reader = db.getDataReader(str);
                DataTable table = new DataTable();
                table.Load(reader);
                guna2DataGridView1.DataSource = table;
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);
            }
        }

        private void HighlightRowAndCell(string searchText)
        {
            searchText = searchText.ToLower();

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.BackColor = Color.Empty;
                }

                DataGridViewCell cellToCheck = row.Cells[1];
                if (cellToCheck.Value != null && cellToCheck.Value.ToString().ToLower().Contains(searchText))
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.BackColor = Color.LightBlue;
                    }
                }
            }
        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {
            guna2DataGridView1.CurrentCell = null;

            string searchText = guna2TextBox2.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                HighlightRowAndCell(searchText);
            }
            else
            {
                foreach (DataGridViewRow row in guna2DataGridView1.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.BackColor = Color.Empty;
                    }
                }
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView2.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in guna2DataGridView2.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        guna2DataGridView2.Rows.Remove(row);
                        CapNhatThanhTien();
                    }
                    else
                    {
                        guna2DataGridView2.ClearSelection();
                        if (guna2DataGridView2.Rows.Count > 0)
                        {
                            int lastRowIndex = guna2DataGridView2.Rows.Count - 1;
                            if (!guna2DataGridView2.Rows[lastRowIndex].IsNewRow)
                            {
                                guna2DataGridView2.Rows[lastRowIndex].Selected = true;
                                guna2DataGridView2.FirstDisplayedScrollingRowIndex = lastRowIndex;
                            }                            
                        }
                        MessageBox.Show("Không có dòng nào để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    MessageBox.Show("Xoá sản phẩm thành công!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void dataBinding(DataTable pDT)
        {
            guna2TextBoxMHH.DataBindings.Clear();
            guna2TextBoxTHH.DataBindings.Clear();
            guna2ComboBoxDVT.DataBindings.Clear();
            guna2TextBoxDG.DataBindings.Clear();
            guna2NumericUpDown1.DataBindings.Clear();

            guna2TextBoxMHH.DataBindings.Add("Text", pDT, "Column1");
            guna2TextBoxTHH.DataBindings.Add("Text", pDT, "SanPham");
            guna2TextBoxDG.DataBindings.Add("Text", pDT, "DonGia");
            guna2NumericUpDown1.DataBindings.Add("Text", pDT, "SoLuong");
        }

        private void CapNhatThanhTien()
        {
            double tongThanhTien = 0;
            foreach (DataGridViewRow row in guna2DataGridView2.Rows)
            {
                if (row.Cells[4].Value != null && row.Cells[2].Value != null)
                {
                    try
                    {
                        int soluong = Convert.ToInt32(row.Cells[4].Value);
                        double dongia = Convert.ToDouble(row.Cells[2].Value);

                        tongThanhTien += soluong * dongia;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi khi tính toán: " + ex.Message);
                        return;
                    }
                }
            }
            ThongTinHoaDon.tongTienHD = tongThanhTien;
            guna2TextBox5.Text = tongThanhTien.ToString("N0") + " VNĐ";
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            reset();
        }

    }
}