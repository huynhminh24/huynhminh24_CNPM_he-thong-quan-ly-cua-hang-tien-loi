using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace fr_Main.UserControls
{
    public partial class NV : UserControl
    {
        NhanVien nv = new NhanVien();
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        private bool isEmailValid = false;

        public NV()
        {
            InitializeComponent();
        }

        private void guna2TextBox3_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox3.Text == "")
            {
                guna2TextBox3.Text = "Tên, mã nhân viên";
                guna2TextBox3.ForeColor = Color.Silver;
            }
        }

        private void guna2TextBox3_Enter(object sender, EventArgs e)
        {
            if (guna2TextBox3.Text == "Tên, mã nhân viên")
            {
                guna2TextBox3.Text = "";
                guna2TextBox3.ForeColor = Color.Black;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (isEmailValid)
            {
                try
                {
                    DateTime selectedDate = guna2DateTimePicker1.Value;
                    DateTime currentDate = DateTime.Today;
                    int age = currentDate.Year - selectedDate.Year;

                    if (string.IsNullOrEmpty(guna2TextBoxTHH.Text) ||
                        string.IsNullOrEmpty(guna2TextBox4.Text) ||
                        string.IsNullOrEmpty(guna2TextBox1.Text) ||
                        string.IsNullOrEmpty(guna2TextBoxSLT.Text) ||
                        string.IsNullOrEmpty(guna2TextBox2.Text))
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string email2 = guna2TextBox2.Text.Trim();
                    string sdt2 = guna2TextBoxSLT.Text.Trim();

                    if (IsDuplicateEmail(email2))
                    {
                        MessageBox.Show("Email đã tồn tại trong danh sách. Vui lòng chọn email khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (IsDuplicateSDT(sdt2))
                    {
                        MessageBox.Show("Số điện thoại đã tồn tại trong danh sách. Vui lòng chọn số điện thoại khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else if (selectedDate.Date > currentDate.AddYears(-age))
                        age--;

                    if (age < 18 || selectedDate > currentDate)
                    {
                        MessageBox.Show("Ngày sinh không hợp lệ !" +
                            "\nVui lòng chọn lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    db.Open();

                    string MaNV = guna2TextBoxMHH.Text;
                    string TenNV = guna2TextBoxTHH.Text;

                    DateTime time = guna2DateTimePicker1.Value;
                    string NSinh = time.ToString("yyyy-MM-dd");

                    string GioiTinh = guna2TextBox4.Text;

                    string DiaChi = guna2TextBox1.Text;
                    string sdt = guna2TextBoxSLT.Text;
                    string email = guna2TextBox2.Text;

                    string sql = "INSERT INTO [NHANVIEN] VALUES ('" + MaNV + "', N'" + TenNV + "', N'" + NSinh + "', N'" + GioiTinh + "', N'" + DiaChi + "','" + sdt + "','" + email + "')";

                    using (SqlCommand cmd = new SqlCommand(sql, db.connect()))
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Thêm mới thành công!!");
                        }
                        else
                        {
                            MessageBox.Show("Thất bại.");
                        }

                        db.Close();
                        loadDS();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                guna2Button1.Enabled = false;
            }
            else
            {
                MessageBox.Show("Vui lòng nhập địa chỉ email.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void NV_Load(object sender, EventArgs e)
        {
            loadDS();
            guna2TextBoxMHH.Enabled = guna2Button1.Enabled = false;

            guna2DataGridView1.Columns["MaNV"].HeaderText = "Mã NV";
            guna2DataGridView1.Columns["TenNV"].HeaderText = "Tên NV";
            guna2DataGridView1.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
            guna2DataGridView1.Columns["GioiTinh"].HeaderText = "Giới Tính";
            guna2DataGridView1.Columns["DiaChi"].HeaderText = "Địa Chỉ";
            guna2DataGridView1.Columns["SDT"].HeaderText = "SĐT";
            guna2DataGridView1.Columns["Email"].HeaderText = "Email";
        }

        public void loadDS()
        {
            string str = "select *from NhanVien";
            SqlDataReader reader = db.getDataReader(str);
            DataTable table = new DataTable();
            table.Load(reader);
            guna2DataGridView1.DataSource = table;
            DataTable dt = (DataTable)guna2DataGridView1.DataSource;
            LKDL(dt);
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int indexSelect = e.RowIndex;
            DataGridViewRow data = guna2DataGridView1.Rows[indexSelect];
            guna2TextBoxMHH.Text = data.Cells[0].Value.ToString();
            guna2TextBoxTHH.Text = data.Cells[1].Value.ToString();

            guna2DateTimePicker1.Text = data.Cells[2].Value.ToString();
            guna2TextBox4.Text = data.Cells[3].Value.ToString();
            guna2TextBox1.Text = data.Cells[4].Value.ToString();
            guna2TextBoxSLT.Text = data.Cells[5].Value.ToString();
            guna2TextBox2.Text = data.Cells[6].Value.ToString();

            guna2Button1.Enabled = false;
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            guna2TextBoxMHH.DataBindings.Clear();
            guna2TextBoxTHH.DataBindings.Clear();

            guna2TextBox4.DataBindings.Clear();
            guna2TextBox1.DataBindings.Clear();
            guna2TextBoxSLT.DataBindings.Clear();
            guna2TextBox2.DataBindings.Clear();
            guna2DateTimePicker1.DataBindings.Clear();

            guna2Button1.Enabled = true;
            guna2TextBoxMHH.Text = string.Empty;
            int manv = (int)db.getScalar("select MaNV from NhanVien ORDER BY MaNV DESC") + 1;
            guna2TextBoxMHH.Text = manv.ToString();
            guna2TextBoxTHH.Text = string.Empty;
            guna2TextBox4.Text = string.Empty;

            guna2TextBoxSLT.Text = string.Empty;
            guna2TextBox1.Text = string.Empty;
            guna2TextBox2.Text = string.Empty;

            guna2DateTimePicker1.Value = DateTime.Parse("01-01-2000");
        }

        void LKDL(DataTable pDT)
        {
            guna2TextBoxMHH.DataBindings.Clear();
            guna2TextBoxTHH.DataBindings.Clear();

            guna2TextBox4.DataBindings.Clear();
            guna2TextBox1.DataBindings.Clear();
            guna2TextBoxSLT.DataBindings.Clear();
            guna2TextBox2.DataBindings.Clear();
            guna2DateTimePicker1.DataBindings.Clear();

            guna2TextBoxMHH.DataBindings.Add("Text", pDT, "MaNV");
            guna2TextBoxTHH.DataBindings.Add("Text", pDT, "TenNV");

            guna2DateTimePicker1.DataBindings.Add("Text", pDT, "NgaySinh");
            guna2TextBox4.DataBindings.Add("Text", pDT, "GioiTinh");
            guna2TextBox1.DataBindings.Add("Text", pDT, "DiaChi");
            guna2TextBoxSLT.DataBindings.Add("Text", pDT, "SDT");
            guna2TextBox2.DataBindings.Add("Text", pDT, "Email");
        }
        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(guna2TextBox4.Text) || string.IsNullOrEmpty(guna2TextBox1.Text) || string.IsNullOrEmpty(guna2TextBoxSLT.Text)
                || string.IsNullOrEmpty(guna2TextBoxTHH.Text) || string.IsNullOrEmpty(guna2TextBox2.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }
            string email2 = guna2TextBox2.Text.Trim();
            string sdt2 = guna2TextBoxSLT.Text.Trim();

            //if (IsDuplicateEmail(email2))
            //{
            //    MessageBox.Show("Email đã tồn tại trong danh sách. Vui lòng chọn email khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //if (IsDuplicateSDT(sdt2))
            //{
            //    MessageBox.Show("Số điện thoại đã tồn tại trong danh sách. Vui lòng chọn số điện thoại khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            DateTime selectedDate = guna2DateTimePicker1.Value;
            DateTime currentDate = DateTime.Today;
            int age = currentDate.Year - selectedDate.Year;

            if (selectedDate.Date > currentDate.AddYears(-age))
                age--;

            if (age < 18 || selectedDate > currentDate)
            {
                MessageBox.Show("Ngày sinh không hợp lệ !" +
                    "\nVui lòng chọn lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                db.Open();
                string MaNV = guna2TextBoxMHH.Text;
                string TenNV = guna2TextBoxTHH.Text;
                

                

                DateTime time = guna2DateTimePicker1.Value;
                string NSinh = time.ToString("yyyy-MM-dd");
                string GioiTinh = guna2TextBox4.Text;
                string DiaChi = guna2TextBox1.Text;
                string sdt = guna2TextBoxSLT.Text;
                string email = guna2TextBox2.Text;

                string sql = "UPDATE [NHANVIEN] SET TENNV = N'" + TenNV + "', NGAYSINH = '" + NSinh + "', GIOITINH = N'" + GioiTinh + "' , Email = N'" + email + "' , DIACHI = N'" + DiaChi + "', SDT = '" + sdt + "' WHERE MANV = '" + MaNV + "'";

                using (SqlCommand cmd = new SqlCommand(sql, db.connect()))
                {
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!!");
                    }
                    else
                    {
                        MessageBox.Show("Thất bại.");
                    }

                    db.Close();
                    loadDS();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (int.Parse(guna2TextBoxMHH.Text) == ShareVar.MaNV)
            {
                MessageBox.Show("Khong the xoa dang su dung");
                return;
            }

            if (guna2DataGridView1.SelectedRows.Count > 0)
            {

                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này không?", "Xác nhận xóa!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        string MaNV = guna2TextBoxMHH.Text;

                        string xoaTK = "DELETE FROM [TaiKhoan] WHERE MANV = '" + MaNV + "'";
                        db.GetNonQuery(xoaTK);

                        db.Open();
                        string sql = "DELETE FROM [NHANVIEN] WHERE MANV = '" + MaNV + "'";
                        using (SqlCommand cmd = new SqlCommand(sql, db.connect()))
                        {
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Xóa thành công!!");
                            }
                            else
                            {
                                MessageBox.Show("Thất bại.");
                            }

                            db.Close();
                            loadDS();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }

                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một 1 dòng để xóa");
            }
            loadDS();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            reset();
        }

        public void reset()
        {
            string str = "select * from Nhanvien";
            SqlDataReader reader = db.getDataReader(str);
            DataTable table = new DataTable();
            table.Load(reader);
            guna2DataGridView1.DataSource = table;
            LKDL(table);
        }

        private void HighlightRowAndCell(string searchText)
        {
            searchText = searchText.ToLower();
            guna2DataGridView1.ClearSelection();
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                bool foundInRow = false;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchText))
                    {
                        foundInRow = true;
                    }
                }
                if (foundInRow)
                {
                    row.Selected = true;
                    guna2DataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                    guna2DataGridView1.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {
            string searchText = guna2TextBox3.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                HighlightRowAndCell(searchText);
            }
            else
            {
                guna2DataGridView1.ClearSelection();
                foreach (DataGridViewRow row in guna2DataGridView1.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.BackColor = Color.Empty;
                    }
                }
            }
        }

        private void guna2DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = guna2DateTimePicker1.Value;
            DateTime currentDate = DateTime.Today;
            int age = currentDate.Year - selectedDate.Year;
            if (selectedDate.Date > currentDate.AddYears(-age))
                age--;

            if (age < 18 || selectedDate > currentDate)
            {
                MessageBox.Show(this, "Ngày không hợp lệ! \nNhân viên chưa đủ 18 tuổi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        private void guna2TextBoxSLT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Vui lòng chỉ nhập số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void guna2TextBox2_Leave(object sender, EventArgs e)
        {
            string email = guna2TextBox2.Text.Trim();

            if (!string.IsNullOrEmpty(email))
            {
                if (IsValidEmail(email))
                {
                    isEmailValid = true;
                }
                else
                {
                    MessageBox.Show("Địa chỉ email không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    guna2TextBox2.Focus();
                    isEmailValid = false;
                }
            }
        }
        private bool IsDuplicateEmail(string email)
        {
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (row.Cells["Email"].Value != null && row.Cells["Email"].Value.ToString().Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsDuplicateSDT(string sdt)
        {
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (row.Cells["SDT"].Value != null && row.Cells["SDT"].Value.ToString().Equals(sdt))
                {
                    return true;
                }
            }
            return false;
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataTable dt = (DataTable)guna2DataGridView1.DataSource;
                LKDL(dt);
                guna2Button1.Enabled = false;
            }
        }

        private void guna2TextBoxTHH_Enter(object sender, EventArgs e)
        {
            guna2TextBoxTHH.DataBindings.Clear();
        }

        private void guna2TextBox1_Enter(object sender, EventArgs e)
        {
            guna2TextBox1.DataBindings.Clear();

        }

        private void guna2TextBoxSLT_Enter(object sender, EventArgs e)
        {
            guna2TextBoxSLT.DataBindings.Clear();
        }

        private void guna2TextBox2_Enter(object sender, EventArgs e)
        {
            guna2TextBox2.DataBindings.Clear();
        }

        private void guna2TextBox4_Enter(object sender, EventArgs e)
        {
            guna2TextBox4.DataBindings.Clear();
        }
    }
}
