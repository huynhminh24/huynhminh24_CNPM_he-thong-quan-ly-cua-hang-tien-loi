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
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using Guna.UI2.WinForms;

namespace fr_Main.UserControls
{
    public partial class SP : UserControl
    {
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        DataTable dt = new DataTable();
        public SP()
        {
            InitializeComponent();
        }

        void HienThi_SanPham()
        {
            string SP = "SELECT * FROM SanPham";
            DataTable dt = db.getDataTable(SP);
            guna2DataGridView1.DataSource = dt;
            dataBinding(dt);
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

        public void dataBinding(DataTable pDT)
        {
            guna2TextBoxMHH.DataBindings.Clear();
            guna2TextBoxTHH.DataBindings.Clear();
            guna2TextBoxDG.DataBindings.Clear();
            guna2TextBoxSLT.DataBindings.Clear();

            guna2TextBoxMHH.DataBindings.Add("Text", pDT, "MaSP");
            guna2TextBoxTHH.DataBindings.Add("Text", pDT, "TenSP");
            guna2TextBoxDG.DataBindings.Add("Text", pDT, "DonGia");
            guna2TextBoxSLT.DataBindings.Add("Text", pDT, "SoLuong");
        }

        private void guna2DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                object selectedMaDonVi = guna2DataGridView1.SelectedRows[0].Cells["MaDonVi"].Value;
                guna2ComboBoxDVT.SelectedValue = selectedMaDonVi;

                object selectedMDanhMuc = guna2DataGridView1.SelectedRows[0].Cells["MaDanhMuc"].Value;
                guna2ComboBoxDM.SelectedValue = selectedMDanhMuc;

                guna2Button1.Enabled = false;
            }
        }

        private void SP_Load(object sender, EventArgs e)
        {
            guna2Button1.Enabled = false;
            HienThi_SanPham();
            LoadCombobox();
            guna2ComboBox1.SelectedValue = -1;
            guna2ComboBox1.SelectedValue = -1;
            guna2DataGridView1.SelectionChanged += guna2DataGridView1_SelectionChanged;
            guna2DataGridView1.Columns["MaSP"].HeaderText = "Mã SP";
            guna2DataGridView1.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
            guna2DataGridView1.Columns["DonGia"].HeaderText = "Giá Bán";
            guna2DataGridView1.Columns["MaDonVi"].HeaderText = "Đơn Vị Tính";
            guna2DataGridView1.Columns["SoLuong"].HeaderText = "Số Lượng";
            guna2DataGridView1.Columns["MaDanhMuc"].HeaderText = "Danh Mục";

            guna2DataGridView1.Columns["MaSP"].Width = 100;
            guna2DataGridView1.Columns["TenSP"].Width = 230;
            guna2DataGridView1.Columns["DonGia"].Width = 130;
            guna2DataGridView1.Columns["MaDonVi"].Width = 150;
            guna2DataGridView1.Columns["SoLuong"].Width = 100;
            guna2DataGridView1.Columns["MaDanhMuc"].Width = 150;
            guna2TextBoxSLT.ReadOnly = true;

            guna2TextBoxMHH.Enabled = false;          
        }
        
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string TenSP = guna2TextBoxSLT.Text.Trim();
            if (IsDuplicateTenSP(TenSP))
            {
                MessageBox.Show("Tên sản phẩm bị trùng. Vui lòng chọn tên sản phẩm khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sql = "SELECT * FROM SanPham";
            dt = db.getDataTable(sql);
            DataRow newrow = dt.NewRow();
            int lastMaSP = int.Parse(guna2TextBoxMHH.Text);

            newrow["MaSP"] = lastMaSP;
            newrow["TenSP"] = guna2TextBoxTHH.Text;
            newrow["DonGia"] = double.Parse(guna2TextBoxDG.Text);
            newrow["MaDonVi"] = guna2ComboBoxDVT.SelectedValue;
            newrow["SoLuong"] = 0;
            newrow["MaDanhMuc"] = guna2ComboBoxDM.SelectedValue;
            dt.Rows.Add(newrow);

            int kq = db.updateDataBase(dt, sql);
           

            
            if (kq > 0)
            {
                MessageBox.Show("Thêm thành công.");
                guna2DataGridView1.DataSource = dt;
            }
            else
            {
                MessageBox.Show("Thêm không thành công.");
            }
            HienThi_SanPham();
            
            guna2Button1.Enabled = false;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                int maHH = (int)guna2DataGridView1.SelectedRows[0].Cells["MaSP"].Value;
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này không?","Xác nhận xóa!",MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        string sqlDeleteHH = "DELETE FROM SanPham WHERE MaSP = @maHH";
                        db.GetNonQuery(sqlDeleteHH, new SqlParameter("@maHH", maHH));

                        MessageBox.Show("Xóa thành công");
                        guna2DataGridView1.Rows.RemoveAt(guna2DataGridView1.SelectedRows[0].Index);
                        guna2DataGridView1.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Xóa thất bại: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để xóa");
            }
            HienThi_SanPham();
            
        }
        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(guna2TextBoxTHH.Text) || string.IsNullOrEmpty(guna2TextBoxDG.Text) ||
                guna2ComboBoxDVT.SelectedValue == null || string.IsNullOrEmpty(guna2TextBoxSLT.Text) ||
                guna2ComboBoxDM.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if(!ValidateInput())
            {
                return;
            }
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = guna2DataGridView1.SelectedRows[0];
                int maSP = (int)selectedRow.Cells[0].Value;

                string sql = "select * from SanPham";
                db.updateDataBase(dt, sql);
                dt = db.getDataTable(sql);
                dt.PrimaryKey = new DataColumn[] { dt.Columns["MaSP"] };
                DataRow row = dt.Rows.Find(maSP);

                if (row != null)
                {
                    string TenSP = guna2TextBoxSLT.Text.Trim();

                    if (IsDuplicateTenSP(TenSP))
                    {
                        MessageBox.Show("Tên sản phẩm bị trùng. Vui lòng chọn tên sản phẩm khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                        row["TenSP"] = guna2TextBoxTHH.Text;
                        row["DonGia"] = guna2TextBoxDG.Text;
                        row["MaDonVi"] = guna2ComboBoxDVT.SelectedValue;
                        row["SoLuong"] = guna2TextBoxSLT.Text;
                        row["MaDanhMuc"] = guna2ComboBoxDM.SelectedValue;
                        int kq = db.updateDataBase(dt, sql);
                        if (kq > 0)
                        {
                            MessageBox.Show("Sửa thành công");
                            guna2DataGridView1.DataSource = dt;
                        }
                        else
                        {
                            MessageBox.Show("Sửa thất bại");
                        }
                        dataBinding(dt);
                    
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khóa chính");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để sửa");
            }
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            guna2TextBoxMHH.Text = string.Empty;
            guna2TextBoxTHH.Text = string.Empty;
            guna2ComboBoxDVT.Text = string.Empty;
            guna2ComboBoxDM.Text = string.Empty;
            guna2TextBoxDG.Text = string.Empty;
            guna2TextBoxSLT.Text = string.Empty;
            guna2TextBoxSLT.Enabled = false;
            guna2TextBoxMHH.Enabled = false;
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            frm_NhapHang1 fromnhaphang = new frm_NhapHang1();
            fromnhaphang.ShowDialog();
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                    FileName = "DanhSachSanPham.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToExcel(guna2DataGridView1, saveFileDialog.FileName);
                    MessageBox.Show("Dữ liệu đã được xuất thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToExcel(DataGridView dataGridView, string filePath)
        {

             Excel.Application excelApp = new Excel.Application();
             excelApp.Visible = false;
             Excel.Workbook workbook = excelApp.Workbooks.Add();
             Excel.Worksheet worksheet = workbook.Sheets[1];
             for (int i = 0; i < dataGridView.Columns.Count; i++)
             {
                 worksheet.Cells[1, i + 1] = dataGridView.Columns[i].HeaderText;
             }
             for (int i = 0; i < dataGridView.Rows.Count; i++)
             {
                 for (int j = 0; j < dataGridView.Columns.Count; j++)
                 {
                     worksheet.Cells[i + 2, j + 1] = dataGridView.Rows[i].Cells[j].Value;
                 }
             }
             workbook.SaveAs(filePath);
             workbook.Close();
             excelApp.Quit();
        }

        private void guna2TextBox1_Leave(object sender, EventArgs e)
        {
            guna2TextBox1.Clear();
            if (guna2TextBox1.Text == "")
            {
                guna2TextBox1.Text = "Tên, mã sản phẩm";
                guna2TextBox1.ForeColor = Color.Silver;
            }
            HienThi_SanPham();
        }

        private void guna2TextBox1_Enter(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text == "Tên, mã sản phẩm")
            {
                guna2TextBox1.Text = "";
                guna2TextBox1.ForeColor = Color.Black;
            }
            guna2ComboBox1.SelectedValue = -1;
            guna2ComboBox1.SelectedValue = -1;
            guna2ComboBox2.SelectedIndex = -1;
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            guna2Button1.Enabled = true;
            guna2ComboBoxDVT.SelectedValue = -1;
            guna2ComboBoxDM.SelectedValue = -1;
            guna2ComboBoxDM.SelectedValue = -1;


            guna2TextBoxMHH.Text = string.Empty;
            int masp = (int)db.getScalar("select TOP 1 MaSP from SanPham ORDER BY MaSP DESC") + 1;
            guna2TextBoxMHH.Text = masp.ToString();


            guna2TextBoxTHH.Text = string.Empty;
            
            guna2TextBoxDG.Text = string.Empty;
            guna2TextBoxSLT.Text = "0";
            guna2TextBoxSLT.Enabled = false;
            guna2TextBoxMHH.Enabled = false;
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
                bool foundInRow = false;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchText))
                    {
                        foundInRow = true;
                        break;
                    }
                }

                if (foundInRow)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.BackColor = Color.LightBlue;
                    }
                }
                else
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.BackColor = Color.Empty;
                    }
                }
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            guna2DataGridView1.CurrentCell = null;

            string searchText = guna2TextBox1.Text.Trim();
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

        void ClearBindings()
        {
            guna2TextBoxMHH.DataBindings.Clear();
            guna2TextBoxTHH.DataBindings.Clear();
            guna2TextBoxDG.DataBindings.Clear();
            guna2TextBoxSLT.DataBindings.Clear();
        }

        void LKDL(DataTable pDT)
        {
            ClearBindings();
            guna2TextBoxMHH.DataBindings.Add("Text", pDT, "MaSP");
            guna2TextBoxTHH.DataBindings.Add("Text", pDT, "TenSP");
            guna2TextBoxDG.DataBindings.Add("Text", pDT, "DonGia");
            guna2TextBoxSLT.DataBindings.Add("Text", pDT, "SoLuong");
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

        private void guna2Button5_Click_1(object sender, EventArgs e)
        {
            reset();
            guna2ComboBox2.SelectedIndex = -1;
            guna2ComboBox1.SelectedIndex = -1;
            guna2ComboBox1.SelectedIndex = -1;
        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            frm_GiamGia mn = new frm_GiamGia();
            mn.ShowDialog();
        }

        private void guna2TextBoxDG_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;

                MessageBox.Show("Vui lòng chỉ nhập số vào ô này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private bool IsDuplicateTenSP(string TenSP)
        {
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (row.Cells["TenSP"].Value != null && row.Cells["TenSP"].Value.ToString().Equals(TenSP, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
