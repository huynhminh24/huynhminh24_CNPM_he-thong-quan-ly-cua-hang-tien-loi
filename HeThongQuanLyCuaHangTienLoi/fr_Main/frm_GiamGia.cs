using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fr_Main
{
    public partial class frm_GiamGia : Form
    {
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        DataTable dt = new DataTable();
        public frm_GiamGia()
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

        private void frm_GiamGia_Load(object sender, EventArgs e)
        {
            loadDS();
            LoadCombobox();
            guna2DataGridView1.Columns[0].HeaderText = "Mã Giảm Giá";
            guna2DataGridView1.Columns[1].HeaderText = "% Giảm Giá";
            guna2DataGridView1.Columns[2].HeaderText = "Ngày Bắt Đầu";
            guna2DataGridView1.Columns[3].HeaderText = "Ngày Kết Thúc";
            guna2DataGridView1.Columns[4].HeaderText = "Mã SP";
            guna2DateTimePicker3.Enabled = guna2DateTimePicker1.Enabled = true;
        }

        void LoadCombobox()
        {
            string sp = "select * from SanPham";
            DataTable dt = db.getDataTable(sp);
            guna2ComboBox1.DataSource = dt;
            guna2ComboBox1.DisplayMember = "TenSP";
            guna2ComboBox1.ValueMember = "MaSP";
        }

        public void loadDS()
        {
            guna2Button1.Enabled = false;
            string str = "select * from GiamGia";
            SqlDataReader reader = db.getDataReader(str);
            DataTable table = new DataTable();
            table.Load(reader);
            guna2DataGridView1.DataSource = table;
            DataTable dt = (DataTable)guna2DataGridView1.DataSource;
            LKDL(dt);
        }

        void LKDL(DataTable pDT)
        {
            guna2DateTimePicker3.DataBindings.Clear();
            guna2DateTimePicker1.DataBindings.Clear();
            guna2TextBoxMHH.DataBindings.Clear();

            guna2DateTimePicker3.DataBindings.Add("Text", pDT, "NgayBD");
            guna2DateTimePicker1.DataBindings.Add("Text", pDT, "NgayKT");
            guna2TextBoxMHH.DataBindings.Add("Text", pDT, "PhanTramGiamGia");
        }

        private void guna2DataGridView1_SelectionChanged(object sender, EventArgs e)
        {

            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);
                object selectedMaDonVi = guna2DataGridView1.SelectedRows[0].Cells["MaSP"].Value;
                guna2ComboBox1.SelectedValue = selectedMaDonVi;
                guna2Button1.Enabled = false;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (guna2ComboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Chọn sản phẩm cần giảm giá", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DateTime selectedDate = guna2DateTimePicker1.Value;
            DateTime currentDate = DateTime.Now;

            if (!string.IsNullOrEmpty(guna2TextBoxMHH.Text))
            {
                try
                {
                    double enteredValue = double.Parse(guna2TextBoxMHH.Text);

                    if (enteredValue < 1 || enteredValue > 100)
                    {
                        MessageBox.Show("Giá trị không nằm trong khoảng từ 1 đến 100.");
                        return;
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Giá trị không phải là số");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Chưa nhập phần trăm.");
                return;
            }
            if (selectedDate.Date < currentDate.Date)
            {
                MessageBox.Show("Ngày kết thúc không hợp lệ. Vui lòng chọn sau ngày hiện tại");
                return;
            }
            else
            {
                try
                {
                    int phanTram = int.Parse(guna2TextBoxMHH.Text);
                    int masp = (int)guna2ComboBox1.SelectedValue;
                    int magg = (int)db.getScalar("SELECT TOP 1 MaGiamGia FROM GiamGia ORDER BY MaGiamGia DESC") + 1;
                    string ngaykt = guna2DateTimePicker1.Value.ToString("yyyy-MM-dd");
                    string strGiamGia = "INSERT INTO GiamGia VALUES('" + magg + "','" + phanTram + "', GETDATE(),'" + ngaykt + "','" + masp + "')";
                    db.GetNonQuery(strGiamGia);

                    MessageBox.Show("Thêm mã giảm giá thành công!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                loadDS();
                guna2Button1.Enabled = false;
            }
        }

        private void guna2TextBoxMHH_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                int magg = (int)guna2DataGridView1.SelectedRows[0].Cells["MaGiamGia"].Value;
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa mã giảm giá này không?",
                                                    "Xác nhận xóa!",
                                                    MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        string sqlDeleteGG = "DELETE FROM GiamGia WHERE MaGiamGia = '" + magg + "' ";
                        db.GetNonQuery(sqlDeleteGG);

                        MessageBox.Show("Xóa thành công");
                        guna2DataGridView1.Rows.RemoveAt(guna2DataGridView1.SelectedRows[0].Index);
                        loadDS();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Xóa thất bại: " + ex.Message);
                    }
                }
            }
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            guna2DateTimePicker3.DataBindings.Clear();
            guna2DateTimePicker1.DataBindings.Clear();
            guna2TextBoxMHH.DataBindings.Clear();

            guna2ComboBox1.SelectedIndex = -1;
            guna2ComboBox1.SelectedIndex = -1;
            guna2TextBoxMHH.Clear();
            guna2DateTimePicker3.Value = DateTime.Today;
            guna2DateTimePicker3.Enabled = false;
            guna2DateTimePicker1.Enabled = true;
            guna2Button1.Enabled = true;
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {

            string ngayBD = guna2DateTimePicker3.Value.ToString("yyyy-MM-dd");
            string ngayKT = guna2DateTimePicker1.Value.ToString("yyyy-MM-dd");

            DateTime ngayBatDau = guna2DateTimePicker3.Value;
            DateTime ngayKetThuc = guna2DateTimePicker1.Value;

            if (!string.IsNullOrEmpty(guna2TextBoxMHH.Text))
            {
                try
                {
                    double enteredValue = double.Parse(guna2TextBoxMHH.Text);

                    if (enteredValue < 1 || enteredValue > 100)
                    {
                        MessageBox.Show("Giá trị không nằm trong khoảng từ 1 đến 100.");
                        return;
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Giá trị không phải là số");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Chưa nhập phần trăm.");
                return;
            }
            if (ngayKetThuc < ngayBatDau)
            {
                MessageBox.Show("Ngày kết thúc phải lớn hơn ngày bắt đầu.");
                return;
            }
            else
            {
                if (guna2DataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = guna2DataGridView1.SelectedRows[0];
                    int magg = (int)selectedRow.Cells[0].Value;
                    string sql = "select * from GiamGia";
                    db.updateDataBase(dt, sql);
                    dt = db.getDataTable(sql);
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["MaGiamGia"] };
                    DataRow row = dt.Rows.Find(magg);
                    if (row != null)
                    {
                        row["PhanTramGiamGia"] = double.Parse(guna2TextBoxMHH.Text);
                        row["NgayBD"] = ngayBD;
                        row["NgayKT"] = ngayKT;
                        row["MaSP"] = guna2ComboBox1.SelectedValue;
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
                loadDS();
            }

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void guna2TextBoxMHH_Enter(object sender, EventArgs e)
        {
            guna2TextBoxMHH.DataBindings.Clear();
        }
    }
}
