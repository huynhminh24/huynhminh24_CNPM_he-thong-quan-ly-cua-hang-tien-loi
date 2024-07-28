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

namespace fr_Main
{ 
    public partial class frm_DoiMK : Form
    {
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        TaiKhoan tk = new TaiKhoan();
        public frm_DoiMK()
        {
            
            InitializeComponent();
            tk = new TaiKhoan();
        }

        public frm_DoiMK(string Email)
        {
            tk.Email1 = Email;
            InitializeComponent();

        }

        public frm_DoiMK(int MaNV,string MaKhau)
        {
            tk.MaNV1 = MaNV;
            tk.MatKhau1 = MaKhau;
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

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            DialogResult KQ;
            KQ = MessageBox.Show("Bạn có muốn huỷ bỏ quy trình tạo mật khẩu mới không?", "Thông Báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (KQ == DialogResult.OK)
            {
                frm_DangNhap mn = new frm_DangNhap();
                this.Hide();
                mn.ShowDialog();
            }
        }

        private void guna2TextBoxDN_Leave(object sender, EventArgs e)
        {
            if (guna2TextBoxDN.Text == "")
            {
                guna2TextBoxDN.UseSystemPasswordChar = false;
                guna2TextBoxDN.Text = "Nhập mật khẩu mới";
                guna2TextBoxDN.ForeColor = Color.Silver;
            }
        }

        private void guna2TextBoxDN_Enter(object sender, EventArgs e)
        {
            if (guna2TextBoxDN.Text == "Nhập mật khẩu mới")
            {
                guna2TextBoxDN.Text = "";
                guna2TextBoxDN.UseSystemPasswordChar = true;
                guna2TextBoxDN.ForeColor = Color.Black;              
            }
        }

        private void guna2TextBox1_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text == "")
            {
                guna2TextBox1.UseSystemPasswordChar = false;
                guna2TextBox1.Text = "Xác nhận lại mật khẩu";
                guna2TextBox1.ForeColor = Color.Silver;
            }
        }

        private void guna2TextBox1_Enter(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text == "Xác nhận lại mật khẩu")
            {
                guna2TextBox1.Text = "";
                guna2TextBox1.UseSystemPasswordChar = true;
                guna2TextBox1.ForeColor = Color.Black;
            }
        }

        private void DoiMatKhauGmail(string gmailEmail, string newPassword)
        {
            string newPasswordHash = BamMatKhau(newPassword);
            string currentPasswordHash = DatabaseHelper.GetHashedPassword(gmailEmail);

            if (XacThucMatKhau(newPassword, currentPasswordHash))
            {
                DatabaseHelper.UpdatePassword(gmailEmail, newPasswordHash);

                MessageBox.Show("Mật khẩu đã được thay đổi thành công!");
            }
            else
            {
                MessageBox.Show("Mật khẩu hiện tại không chính xác!");
            }
        }

        private string BamMatKhau(string matKhau)
        {
            return matKhau;
        }

        private bool XacThucMatKhau(string matKhauNhap, string matKhauDaBam)
        {
            return matKhauNhap == matKhauDaBam;
        }
        public class DatabaseHelper
        {
            public static string GetHashedPassword(string gmailEmail)
            {
                using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-OKUSEB8;Initial Catalog=SkyPOS_DATA;Integrated Security=True"))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(
                        "SELECT PassWord FROM TaiKhoan WHERE Email = @Email", connection))
                    {
                        command.Parameters.AddWithValue("@Email", gmailEmail);
                        return (string)command.ExecuteScalar();
                    }
                }
            }

            public static void UpdatePassword(string gmailEmail, string newPasswordHash)
            {
                using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-OKUSEB8;Initial Catalog=SkyPOS_DATA;Integrated Security=True"))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(
                        "UPDATE Taikhoan SET PassWord = @NewPasswordHash WHERE Email = Email", connection))
                    {
                        command.Parameters.AddWithValue("@NewPasswordHash", newPasswordHash);
                        command.Parameters.AddWithValue("@GmailEmail", gmailEmail);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private void frm_DoiMK_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string matKhau = guna2TextBoxDN.Text;
            string xacNhanMatKhau = guna2TextBox1.Text;
            if (matKhau != xacNhanMatKhau)
            {
                MessageBox.Show("Các mật khẩu đã nhập không khớp. Hãy thử lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox1.Clear();
                return;
            }
            try
            {
                db.Open();

                string email = tk.Email1;
                string sql = "UPDATE [TaiKhoan] SET PassWord = @matKhau WHERE MANV = (SELECT MaNV FROM NhanVien WHERE Email = @email)";

                using (SqlCommand cmd = new SqlCommand(sql, db.connect()))
                {
                    cmd.Parameters.AddWithValue("@matKhau", matKhau);
                    cmd.Parameters.AddWithValue("@email", email);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!!");
                        frm_DangNhap f = new frm_DangNhap();
                        f.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Thất bại.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.Close();
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            string matKhau = guna2TextBoxDN.Text;
            string xacNhanMatKhau = guna2TextBox1.Text;
            if (matKhau != xacNhanMatKhau)
            {
                guna2TextBox1.BorderColor = Color.Red;
            }
            else
            {
                guna2TextBox1.BorderColor = Color.Green;
                guna2TextBoxDN.BorderColor = Color.Green;
            }
        }
    }
}
