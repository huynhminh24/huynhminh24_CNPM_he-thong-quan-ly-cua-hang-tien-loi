using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace fr_Main
{
    public partial class frm_DangNhap : Form
    {
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        public frm_DangNhap()
        {
            InitializeComponent();
        }

        private void guna2TextBoxDN_Leave(object sender, EventArgs e)
        {
            if (guna2TextBoxDN.Text == "")
            {
                guna2TextBoxDN.Text = "Tài khoản";
                guna2TextBoxDN.ForeColor = Color.Silver;
            }
        }

        private void guna2TextBoxDN_Enter(object sender, EventArgs e)
        {
            if (guna2TextBoxDN.Text == "Tài khoản")
            {
                guna2TextBoxDN.Text = "";
                guna2TextBoxDN.ForeColor = Color.Black;
            }
        }

        private void guna2TextBoxMK_Leave(object sender, EventArgs e)
        {
            if (guna2TextBoxMK.Text == "")
            {
                guna2TextBoxMK.UseSystemPasswordChar = false;
                guna2TextBoxMK.Text = "Mật khẩu";
                guna2TextBoxMK.ForeColor = Color.Silver;
            }
        }

        private void guna2TextBoxMK_Enter(object sender, EventArgs e)
        {
            if (guna2TextBoxMK.Text == "Mật khẩu")
            {
                guna2TextBoxMK.Text = "";
                guna2TextBoxMK.UseSystemPasswordChar = true;
                guna2TextBoxMK.ForeColor = Color.Black;
            }
        }

        private void guna2Button3_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(guna2TextBoxDN.Text) || guna2TextBoxDN.Text == "Tài khoản" ||
                string.IsNullOrEmpty(guna2TextBoxMK.Text) || guna2TextBoxMK.Text == "Mật khẩu")
            {
                MessageBox.Show("Tài khoản hoặc mật khẩu chưa được nhập.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string str = "select * from TaiKhoan where UserName = '" + guna2TextBoxDN.Text + "' AND PassWord = '" + guna2TextBoxMK.Text + "'";
                try
                {
                    SqlDataReader reader = db.getDataReader(str);
                    DataTable table = new DataTable();
                    table.Load(reader);

                    if (table.Rows.Count > 0)
                    {
                        ShareVar.MaNV = (int)table.Rows[0][3];
                        frm_Main f = new frm_Main(table.Rows[0][1].ToString(), table.Rows[0][2].ToString(), (int)table.Rows[0][3], (int)table.Rows[0][4]);

                        f.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Tài khoản hoặc mật khẩu không đúng.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi trong quá trình đăng nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }           
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            DialogResult KQ;
            KQ = MessageBox.Show("Bạn có muốn thoát khỏi chương trình không?", "Thông Báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (KQ == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            frm_KhoiPhucTK mn = new frm_KhoiPhucTK();
            mn.ShowDialog();
            this.Hide();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Opacity += .2;
        }

        private void frm_DangNhap_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
