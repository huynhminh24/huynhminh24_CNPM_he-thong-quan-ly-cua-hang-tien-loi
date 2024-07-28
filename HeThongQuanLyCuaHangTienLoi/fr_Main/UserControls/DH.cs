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

namespace fr_Main.UserControls
{
    public partial class DH : UserControl
    {
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        DataTable dt = new DataTable();
        public DH()
        {
            InitializeComponent();
        }

        void LoadCombobox()
        {
            string hd = "select DISTINCT MaNV from HoaDon";
            DataTable dt2 = db.getDataTable(hd);
            guna2ComboBox1.DataSource = dt2;
            guna2ComboBox1.ValueMember = "MaNV";
        }

        private void DH_Load(object sender, EventArgs e)
        {
            LoadCombobox();
            guna2ComboBox1.SelectedValue = -1;
            guna2ComboBox2.SelectedIndex = -1;
            string str = "select *from HoaDon";
            SqlDataReader reader = db.getDataReader(str);
            DataTable table = new DataTable();
            table.Load(reader);
            guna2DataGridView1.DataSource = table;
            DataTable dt = (DataTable)guna2DataGridView1.DataSource;
            LKDL(dt);

            guna2DataGridView1.Columns["MaHD"].HeaderText = "Mã HĐ";
            guna2DataGridView1.Columns["NgayBan"].HeaderText = "Ngày Bán";
            guna2DataGridView1.Columns["TongTien"].HeaderText = "Tổng Tiền";
            guna2DataGridView1.Columns["MaNV"].HeaderText = "Mã NV";
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                    FileName = "DanhSachDonHang.xlsx"
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

        private void FormatNgayBan(object sender, ConvertEventArgs e)
        {
            if (e.Value is DateTime)
            {
                e.Value = ((DateTime)e.Value).ToString("MM-dd-yyyy");
            }
        }


        void LKDL(DataTable pDT)
        {
            ClearBindings();
            guna2TextBoxMHH.DataBindings.Add("Text", pDT, "MaHD");
            guna2TextBoxTHH.DataBindings.Add("Text", pDT, "MaNV");

            Binding ngayBanBinding = new Binding("Text", pDT, "NgayBan");
            ngayBanBinding.Format += new ConvertEventHandler(FormatNgayBan);
            guna2TextBoxDG.DataBindings.Add(ngayBanBinding);



            guna2TextBox2.DataBindings.Add("Text", pDT, "TongTien");
        }

        void ClearBindings()
        {
            guna2TextBoxMHH.DataBindings.Clear();
            guna2TextBoxTHH.DataBindings.Clear();
            guna2TextBoxDG.DataBindings.Clear();
            guna2TextBox2.DataBindings.Clear();
        }

        private void guna2ComboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            guna2DataGridView1.DataSource = null;
            guna2ComboBox1.SelectedIndex = -1;
            guna2ComboBox1.SelectedIndex = -1;

            string chon = guna2ComboBox2.SelectedItem.ToString();

            if (chon == "Tổng tiền cao nhất")
            {
                string str = "select * from HoaDon ORDER BY TongTien DESC";
                SqlDataReader reader = db.getDataReader(str);
                DataTable table = new DataTable();
                table.Load(reader);
                guna2DataGridView1.DataSource = table;
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);

            }
            else if (chon == "Tổng tiền thấp nhất")
            {
                string str = "select * from HoaDon ORDER BY TongTien ASC";
                SqlDataReader reader = db.getDataReader(str);
                DataTable table = new DataTable();
                table.Load(reader);
                guna2DataGridView1.DataSource = table;
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);
            }
            else if (chon == "Cũ nhất")
            {
                string str = "select * from HoaDon ORDER BY NgayBan ASC";
                SqlDataReader reader = db.getDataReader(str);
                DataTable table = new DataTable();
                table.Load(reader);
                guna2DataGridView1.DataSource = table;
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);
            }

            else if (chon == "Mới nhất")
            {
                string str = "select * from HoaDon ORDER BY NgayBan DESC";
                SqlDataReader reader = db.getDataReader(str);
                DataTable table = new DataTable();
                table.Load(reader);
                guna2DataGridView1.DataSource = table;
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);
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

        public void dataBinding(DataTable pDT)
        {
            guna2TextBoxMHH.DataBindings.Clear();
            guna2TextBoxTHH.DataBindings.Clear();
            guna2TextBoxDG.DataBindings.Clear();
            guna2TextBox2.DataBindings.Clear();

            guna2TextBoxMHH.DataBindings.Add("Text", pDT, "MaHD");
            guna2TextBoxTHH.DataBindings.Add("Text", pDT, "MANV");

            Binding ngayBanBinding = new Binding("Text", pDT, "NgayBan");
            ngayBanBinding.Format += new ConvertEventHandler(FormatNgayBan);
            guna2TextBoxDG.DataBindings.Add(ngayBanBinding);



            guna2TextBox2.DataBindings.Add("Text", pDT, "TongTien");
        }

        public void reset()
        {
            string str = "select * from HoaDon";
            SqlDataReader reader = db.getDataReader(str);
            DataTable table = new DataTable();
            table.Load(reader);
            guna2DataGridView1.DataSource = table;
            LKDL(table);
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            reset();
            guna2ComboBox1.SelectedIndex = -1;
            guna2ComboBox2.SelectedIndex = -1;
            guna2ComboBox1.SelectedIndex = -1;
        }

        private void guna2TextBox1_Enter(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text == "Mã hoá đơn")
            {
                guna2TextBox1.Text = "";
                guna2TextBox1.ForeColor = Color.Black;
            }
            guna2ComboBox1.SelectedIndex = -1;
            guna2ComboBox2.SelectedIndex = -1;
            guna2ComboBox1.SelectedIndex = -1;
        }

        private void guna2TextBox1_Leave(object sender, EventArgs e)
        {
            guna2TextBox1.Clear();
            if (guna2TextBox1.Text == "")
            {
                guna2TextBox1.Text = "Mã hoá đơn";
                guna2TextBox1.ForeColor = Color.Silver;
            }
            reset();
        }

        private void HighlightRowAndCell(string searchText)
        {
            searchText = searchText.ToLower();
            bool found = false;

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Equals(searchText))
                    {
                        foreach (DataGridViewCell cellToColor in row.Cells)
                        {
                            cellToColor.Style.BackColor = Color.LightBlue;
                        }
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    break;
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

        private void guna2ComboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            guna2DataGridView1.DataSource = null;

            guna2ComboBox2.SelectedIndex = -1;

            if (guna2ComboBox1.SelectedValue != null)
            {
                int manv = (int)guna2ComboBox1.SelectedValue;
                string str = "select * from HoaDon where MaNV = '" + manv + "' ";
                SqlDataReader reader = db.getDataReader(str);
                DataTable table = new DataTable();
                table.Load(reader);
                guna2DataGridView1.DataSource = table;
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);
            }
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                int maHD = (int)guna2DataGridView1.SelectedRows[0].Cells["MaHD"].Value;
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa đơn hàng này không?", "Xác nhận xóa!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        string sqlDeleteCTHH = "DELETE FROM ChiTietHoaDon WHERE MaHD = @maHD";
                        db.GetNonQuery(sqlDeleteCTHH, new SqlParameter("@maHD", maHD));

                        string sqlDeleteHH = "DELETE FROM HoaDon WHERE MaHD = @maHD";
                        db.GetNonQuery(sqlDeleteHH, new SqlParameter("@maHD", maHD));

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
                MessageBox.Show("Vui lòng chọn một hoá đơn để xóa");
            }
            reset();
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
