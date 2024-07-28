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
using fr_Main.UserControls;

namespace fr_Main
{
    public partial class frm_Main : Form
    {
        int MaNV;

        public int MaNV1
        {
            get { return MaNV; }
            set { MaNV = value; }
        }
        string TenDN;

        public string TenDN1
        {
            get { return TenDN; }
            set { TenDN = value; }
        }
        string MatKhau;

        public string MatKhau1
        {
            get { return MatKhau; }
            set { MatKhau = value; }
        }
        int Quyen;

        public int Quyen1
        {
            get { return Quyen; }
            set { Quyen = value; }
        }

        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        public frm_Main(string tentk, string mk, int MaNV, int quyen)
        {
            this.MaNV = MaNV;
            this.TenDN = tentk;
            this.MatKhau = mk;
            this.Quyen = quyen;

            InitializeComponent();
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {
            timer1.Start();
            guna2TextBoxPQ.Text = TenDN1;

            TQ mn = new TQ();
            addUserControl(mn);

            if (Quyen1 == 2)
            {
                guna2Button8.Visible = false;
                guna2Button7.Visible = false;
                guna2Button6.Visible = false;
                guna2Button2.Visible = false;
                guna2Button4.Location = new Point(132, 0);
                guna2Button5.Location = new Point(264, 0);
            }

            if (Quyen1 == 3)
            {
                guna2Button8.Visible = false;
                guna2Button2.Visible = true;
                guna2Button7.Visible = false;
                guna2Button6.Visible = false;
                guna2Button4.Visible = false;
                guna2Button5.Visible = false;
            }
        }    
        
        private void addUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            metroPanel1.Controls.Clear();
            metroPanel1.Controls.Add(userControl);
            userControl.BringToFront();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            SP mn = new SP();
            addUserControl(mn);
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            BH mn = new BH(MaNV1,Quyen1);
            addUserControl(mn);
        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            NV mn = new NV();
            addUserControl(mn);
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            TK mn = new TK();
            addUserControl(mn);
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            TKE mn = new TKE();
            addUserControl(mn);
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            DH mn = new DH();
            addUserControl(mn);
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            DialogResult result1 = MessageBox.Show("Bạn có muốn đăng xuất không?", "Xác nhận", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result1 == DialogResult.OK)
            {
                this.Hide();
                frm_DangNhap f = new frm_DangNhap();
                f.Show();
            }
        }

        private void frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult KQ;
            KQ = MessageBox.Show("Bạn có muốn thoát khỏi chương trình không?", "Thông Báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (KQ == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Opacity += .2;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            TQ mn = new TQ();
            addUserControl(mn);
        }

        private void frm_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
