using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace fr_Main
{
    public partial class frm_KhoiPhucTK : Form
    {
        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        private int timeLeft;
        public frm_KhoiPhucTK()
        {
            InitializeComponent();
            waitingTimer = new Timer();
            waitingTimer.Interval = 5000;
            waitingTimer.Tick += timer2_Tick;
        }

        Random random = new Random();
        int code;
        List<string> validGmailAddresses = new List<string>();
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
            KQ = MessageBox.Show("Bạn có muốn quay lại không?", "Thông Báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (KQ == DialogResult.OK)
            {
                frm_DangNhap mn = new frm_DangNhap();
                this.Hide();
                mn.ShowDialog();
            }
        }

        private bool IsGmailInDatabase(string email)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-OKUSEB8;Initial Catalog=SkyPOS_DATA;Integrated Security=True"))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM NhanVien WHERE Email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        private void guna2TextBoxDN_Leave(object sender, EventArgs e)
        {
            if (guna2TextBoxE.Text == "")
            {
                guna2TextBoxE.Text = "Nhập email của bạn";
                guna2TextBoxE.ForeColor = Color.Silver;
            }
        }

        private void guna2TextBoxDN_Enter(object sender, EventArgs e)
        {
            if (guna2TextBoxE.Text == "Nhập email của bạn")
            {
                guna2TextBoxE.Text = "";
                guna2TextBoxE.ForeColor = Color.Black;
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {

            if (code.ToString().Equals(guna2TextBox1.Text))
            {
                DialogResult KQ;
                KQ = MessageBox.Show("Chúc mừng!! Bạn đã xác minh thành công!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (KQ == DialogResult.OK)
                {
                    string email = guna2TextBoxE.Text;
                    frm_DoiMK mn = new frm_DoiMK(email);
                    this.Hide();
                    mn.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Mã xác minh không chính xác!", "Thông Báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void guna2TextBox1_Leave(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text == "")
            {
                float newSize = 10.0f;
                guna2TextBox1.Font = new Font(guna2TextBox1.Font.FontFamily, newSize);
                guna2TextBox1.Font = new Font(guna2TextBox1.Font, FontStyle.Regular);
                guna2TextBox1.Text = "Nhập mã";
                guna2TextBox1.ForeColor = Color.Silver;
            }
        }

        private void guna2TextBox1_Enter(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text == "Nhập mã")
            {
                float newSize = 12.0f;
                guna2TextBox1.Font = new Font(guna2TextBox1.Font.FontFamily, newSize);
                guna2TextBox1.Font = new Font(guna2TextBox1.Font, FontStyle.Bold);
                guna2TextBox1.Text = "";
                guna2TextBox1.ForeColor = Color.Black;
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(guna2TextBoxE.Text) || guna2TextBoxE.Text == "Nhập email của bạn")
            {
                MessageBox.Show("Vui lòng nhập email");
                return;
            }
            string enteredEmail = guna2TextBoxE.Text.Trim();

            if (!IsGmailInDatabase(enteredEmail))
            {
                MessageBox.Show("Không tìm thấy địa chỉ email. Vui lòng nhập địa chỉ email hợp lệ!");
                guna2TextBoxE.Clear();
                return;
            }
            try
            {
                code = random.Next(100000, 1000000);
                var fromAddress = new MailAddress("noreply.skypos@gmail.com", "SkyPOS Support");
                var toAddress = new MailAddress(guna2TextBoxE.Text.Trim());
                const string frompass = "oqlyishinvbgqhru";
                const string subject = "Mã xác minh SkyPOS";
                string body = $"Mã xác minh của bạn là: <strong>{code}</strong>. Sử dụng nó để truy cập vào tài khoản của bạn.<br>Trân trọng,<br>The SkyPOS Team<br>huynhminh24";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, frompass),
                    Timeout = 200000
                };
                using (var mess = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(mess);
                }
                MessageBox.Show("Mã xác minh đã được gửi!");
                guna2Button2.Enabled = false;
                timeLeft = 30;
                time30s.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void guna2TextBoxE_TextChanged(object sender, EventArgs e)
        {
            string enteredEmail = guna2TextBoxE.Text.Trim();

            if (IsGmailInDatabase(enteredEmail))
            {
                guna2TextBoxE.BorderColor = Color.Green;
            }
            else
            {
                guna2TextBoxE.BorderColor = Color.Red;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Opacity += .2;
        }

        private void frm_KhoiPhucTK_Load(object sender, EventArgs e)
        {
            time30s.Start();
        }

        private Timer waitingTimer;

        private void timer2_Tick(object sender, EventArgs e)
        {
            waitingTimer.Stop();
            guna2Button2.Enabled = true;
        }

        private void time30s_Tick_1(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                this.label3.Text = timeLeft + "s";
            }
            else
            {
                time30s.Stop();
                guna2Button2.Enabled = true;
                label3.Text = null;
            }
        }
    }
}
