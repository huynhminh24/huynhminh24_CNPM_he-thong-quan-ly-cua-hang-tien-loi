using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fr_Main.UserControls
{
    public partial class TK : UserControl
    {
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        DataTable dt = new DataTable();
        public TK()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT Nhanvien.MANV, UserName, PassWord, MaVaiTro, TENNV " + "FROM NHANVIEN INNER JOIN TaiKhoan ON Nhanvien.MANV = TaiKhoan.MaNV";
                guna2DataGridView1.DataSource = db.getDataTable(query);
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        void loadcbo()
        {
            string dvt = "select * from NhanVien";
            DataTable dt = db.getDataTable(dvt);
            guna2ComboBox1.DataSource = dt;
            guna2ComboBox1.DisplayMember = "MaNV";
            guna2ComboBox1.ValueMember = "MaNV";

            string dvt1 = "select * from VaiTro";
            DataTable dt1 = db.getDataTable(dvt1);
            guna2ComboBox2.DataSource = dt1;
            guna2ComboBox2.DisplayMember = "MaVaiTro";
            guna2ComboBox2.ValueMember = "MaVaiTro";
        }

        void XoaLKDL()
        {
            guna2ComboBox1.DataBindings.Clear();
            guna2ComboBox2.DataBindings.Clear();

            guna2TextBoxTNV.DataBindings.Clear();
            guna2TextBoxTTK.DataBindings.Clear();
            guna2TextBoxMK.DataBindings.Clear();
        }
        void LKDL(DataTable pDT)
        {
            XoaLKDL();
            guna2ComboBox1.DataBindings.Add("Text", pDT, "MaNV");
            guna2ComboBox2.DataBindings.Add("Text", pDT, "MaVaiTro");
            guna2TextBoxTNV.DataBindings.Add("Text", pDT, "TenNV");
            guna2TextBoxTTK.DataBindings.Add("Text", pDT, "UserName");
            guna2TextBoxMK.DataBindings.Add("Text", pDT, "PassWord");
        }

        private void TK_Load(object sender, EventArgs e)
        {
            guna2Button1.Enabled = false;
            guna2ComboBox1.Enabled = guna2TextBoxTNV.Enabled = false;
            Console.WriteLine("Đã vào sự kiện frm_TaiKhoan_Load");
            LoadData();
            loadcbo();
            guna2DataGridView1.Columns[0].HeaderText = "Mã NV";
            guna2DataGridView1.Columns[1].HeaderText = "Tên Tài Khoản";
            guna2DataGridView1.Columns[2].HeaderText = "Mật Khẩu";
            guna2DataGridView1.Columns[3].HeaderText = "Mã Vai Trò";
            guna2DataGridView1.Columns[4].HeaderText = "Tên nhân viên";
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);
                guna2Button1.Enabled = guna2ComboBox1.Enabled = false;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(guna2TextBoxMK.Text) ||
                    string.IsNullOrEmpty(guna2TextBoxTTK.Text) ||
                     string.IsNullOrEmpty(guna2ComboBox2.Text) ||
                     guna2ComboBox1.SelectedIndex == -1 ||
                     guna2ComboBox2.SelectedIndex == -1
                    )

            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!!");
                return;
            }
            string user = guna2TextBoxTTK.Text.Trim();
            if (IsDuplicateUsername(user))
            {
                MessageBox.Show("Tên tài khoản đã tồn tại trong danh sách. Vui lòng chọn tên khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int manv = (int)guna2ComboBox1.SelectedValue;
            int result = (int)db.getScalar("select count(*) from TaiKhoan where MaNV = '" + manv + "'");
            if (result > 0)
            {
                MessageBox.Show("Nhân viên này đã có tài khoản");
                return;
            }

            else
            {

                int matk = (int)db.getScalar("select TOP 1 MaTK from TaiKhoan ORDER BY MaTK DESC ") + 1;

                string sql = "select * from TaiKhoan";
                dt = db.getDataTable(sql);
                DataRow newrow = dt.NewRow();

                newrow["MaTK"] = matk;
                newrow["MaNV"] = guna2ComboBox1.SelectedValue;
                newrow["UserName"] = guna2TextBoxTTK.Text;
                newrow["PassWord"] = guna2TextBoxMK.Text;
                newrow["MaVaiTro"] = guna2ComboBox2.Text;
                dt.Rows.Add(newrow);
                int kq = db.updateDatabase(dt, sql);
                if (kq > 0)
                {
                    MessageBox.Show("Thêm thành công!");
                    guna2DataGridView1.DataSource = dt;
                }
                else
                {
                    MessageBox.Show("Thêm thất bại!");
                }
                LoadData();
            }
            guna2Button1.Enabled = false;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = guna2DataGridView1.SelectedRows[0];
                string maNV = selectedRow.Cells["MANV"].Value.ToString();
                guna2DataGridView1.Rows.Remove(selectedRow);
                MessageBox.Show("Xóa thành công");
                string deleteQuery = "DELETE FROM TaiKhoan WHERE MANV = '" + maNV + "'";
                db.GetNonQuery(deleteQuery);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để xóa.");
            }
            LoadData();
        }
        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(guna2TextBoxTNV.Text) || string.IsNullOrEmpty(guna2TextBoxTTK.Text) || string.IsNullOrEmpty(guna2TextBoxMK.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                string maNV = selectedRow.Cells["MANV"].Value.ToString();
                string sql = "SELECT * FROM TaiKhoan";
                dt = db.getDataTable(sql);
                dt.PrimaryKey = new DataColumn[] { dt.Columns["MANV"] };
                DataRow row = dt.Rows.Find(maNV);
                if (row != null)
                {
                    row["UserName"] = guna2TextBoxTTK.Text;
                    row["PassWord"] = guna2TextBoxMK.Text;
                    row["MaVaiTro"] = guna2ComboBox2.Text;
                    int kq = db.updateDatabase(dt, sql);
                    if (kq > 0)
                    {
                        MessageBox.Show("Sửa thành công!");
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Sửa thất bại!");
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khóa chính!");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để sửa!");
            }
            LoadData();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {

            guna2TextBoxTNV.ReadOnly = false;
            guna2TextBoxTTK.ReadOnly = false;
            guna2TextBoxMK.ReadOnly = false;

            guna2ComboBox1.Enabled = true;
            guna2Button1.Enabled = true;
            guna2ComboBox2.Enabled = true;

            guna2TextBoxMK.Text = string.Empty;

            guna2TextBoxTNV.Text = string.Empty;
            guna2TextBoxTTK.Text = string.Empty;
            guna2ComboBox1.SelectedIndex = -1;
            guna2ComboBox2.SelectedIndex = -1;
            guna2ComboBox1.SelectedIndex = -1;
            guna2ComboBox2.SelectedIndex = -1;
            XoaLKDL();
        }

        private void guna2ComboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int manv = (int)guna2ComboBox1.SelectedValue;
            guna2TextBoxTNV.Text = db.getScalar("select TenNV from NhanVien where MaNV = '" + manv + "'").ToString();
        }

        private void guna2TextBoxTTK_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;

                MessageBox.Show("Vui lòng chỉ nhập chữ cái và số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private bool IsDuplicateUsername(string email)
        {
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (row.Cells["UserName"].Value != null && row.Cells["UserName"].Value.ToString().Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private void guna2TextBoxTTK_Enter(object sender, EventArgs e)
        {
            guna2TextBoxTTK.DataBindings.Clear();
        }

        private void guna2TextBoxMK_Enter(object sender, EventArgs e)
        {
            guna2TextBoxMK.DataBindings.Clear();
        }
    }
}
