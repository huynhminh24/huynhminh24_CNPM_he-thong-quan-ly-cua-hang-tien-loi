using fr_Main.UserControls;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;

namespace fr_Main
{
    public partial class frm_HoaDon : Form
    {
        private BH _bhForm;

        ConnectDatabase_QLNS db = new ConnectDatabase_QLNS();
        public frm_HoaDon()
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

        public frm_HoaDon(BH bhForm, DataTable data, string MaHD, string MaNV, string TongTien)
        {
            InitializeComponent();
            _bhForm = bhForm;
            guna2TextBox1.Text = MaHD;
            guna2TextBox6.Text = MaNV;
            guna2TextBox4.Text = TongTien;

            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.DataSource = data;
            guna2DataGridView1.Columns[0].HeaderText = "Mã sản phẩm";
            guna2DataGridView1.Columns[1].HeaderText = "Tên sản phẩm";
            guna2DataGridView1.Columns[2].HeaderText = "Giá bán";
            guna2DataGridView1.Columns[3].HeaderText = "Đơn vị tính";
            guna2DataGridView1.Columns[4].HeaderText = "Số lượng";
            //guna2DataGridView1.Columns["GiaBan"].HeaderText = "Giá bán";
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            DateTime str = guna2DateTimePicker1.Value;
            string ngayHD = str.ToString("yyyy-MM-dd");
            try
            {
                string strHoaDon = "INSERT INTO HOADON VALUES('" + int.Parse(guna2TextBox1.Text) + "', GETDATE(),'" + ThongTinHoaDon.tongTienHD + "', '" + ShareVar.MaNV + "')";
                db.GetNonQuery(strHoaDon);

                foreach (DataGridViewRow row in guna2DataGridView1.Rows)
                {
                    string str1 = "Select TOP 1 MaCTHD from ChiTietHoaDon ORDER BY MaCTHD DESC";
                    int mact = (int)db.getScalar(str1) + 1;
                    if (!row.IsNewRow)
                    {
                        int maHD = int.Parse(guna2TextBox1.Text);
                        int maSP = Convert.ToInt32(row.Cells[0].Value);
                        double giaBan = Convert.ToDouble(row.Cells[2].Value);
                        int SoLuong = Convert.ToInt32(row.Cells[4].Value);
                        string strCTHD = "INSERT INTO ChiTietHoaDon VALUES('" + mact + "','" + maSP + "','" + SoLuong + "','" + giaBan + "', '" + maHD + "')";
                        db.GetNonQuery(strCTHD);
                    }
                }
                MessageBox.Show("Tạo hóa đơn thành công");
                _bhForm.LamMoi();
                this.Close();
            }
            catch (Exception ex)
            {
                string errorMessage = "Lỗi: " + ex.Message;
                errorMessage += "\nTại: " + ex.StackTrace;

                MessageBox.Show(errorMessage, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_HoaDon_Load(object sender, EventArgs e)
        {
            timer1.Start();
            guna2DateTimePicker1.Value = DateTime.Now;
            guna2TextBox1.Enabled = guna2TextBox6.Enabled = false;
            timer1.Stop();
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {


        }

        private bool isHandlingTextChanged = false;

        private string RemoveCurrencyText(string input)
        {
            return new string(input.Where(c => Char.IsDigit(c) || c == ',').ToArray());
        }

        private string FormatAsCurrency(double value)
        {
            return string.Format("{0:N0} VNĐ", value);
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(PrintHoaDon);

                PaperSize customSize = new PaperSize("CustomSize", 360, 600);
                pd.DefaultPageSettings.PaperSize = customSize;

                PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
                printPreviewDialog.UseAntiAlias = true;
                printPreviewDialog.PrintPreviewControl.AutoZoom = false;
                printPreviewDialog.PrintPreviewControl.Zoom = 1;
                printPreviewDialog.Size = new Size(400, 640);
                printPreviewDialog.Document = pd;
                printPreviewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xem trước hoá đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PrintHoaDon(object sender, PrintPageEventArgs e)
        {
            StringFormat formatLeft = new StringFormat() { Alignment = StringAlignment.Near };
            StringFormat formatRight = new StringFormat() { Alignment = StringAlignment.Far };
            StringFormat formatCenter = new StringFormat() { Alignment = StringAlignment.Center };

            Font fontTitle = new Font("Arial", 25, FontStyle.Bold);
            Font fontHeader = new Font("Arial", 12);
            Font fontContent = new Font("Arial", 12);
            Font fontChange = new Font("Arial", 15, FontStyle.Bold);

            int tongSoLuong = 0;

            SolidBrush brush = new SolidBrush(Color.Black);
            float yPos = 30f;
            float xPos = 30f;

            e.Graphics.DrawString("SKYPOS", fontTitle, brush, xPos, yPos);
            yPos += 35;
            e.Graphics.DrawString("© 2023 huynhminh24", fontContent, brush, xPos, yPos);
            yPos += 50;

            e.Graphics.DrawString($"Ma hoa don: {guna2TextBox1.Text}", fontHeader, brush, xPos, yPos);
            yPos += 20;
            e.Graphics.DrawString($"Ngay lap: {guna2DateTimePicker1.Value.ToString("dd/MM/yyyy")}", fontHeader, brush, xPos, yPos);
            yPos += 20;
            e.Graphics.DrawString($"Ma nhan vien: {guna2TextBox6.Text}", fontHeader, brush, xPos, yPos);
            yPos += 20;
            e.Graphics.DrawString($"Ghi chu: {guna2TextBox5.Text}", fontHeader, brush, xPos, yPos);
            yPos += 20;
            yPos += 10;

            Pen dashedPen = new Pen(Color.Black, 2)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
            };

            e.Graphics.DrawLine(dashedPen, xPos, yPos, xPos + 300, yPos);
            yPos += 5;
            e.Graphics.DrawLine(dashedPen, xPos, yPos, xPos + 300, yPos);
            yPos += 10;
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    int soLuong = Convert.ToInt32(row.Cells[4].Value);
                    double giaBan = Convert.ToDouble(row.Cells[2].Value);
                    double thanhTien = soLuong * giaBan;

                    tongSoLuong += soLuong;

                    e.Graphics.DrawString($"{soLuong} {((string)row.Cells[1].Value).PadRight(30)}{thanhTien:N0} VNĐ", fontContent, brush, xPos, yPos);
                    yPos += 30;
                }
            }
            e.Graphics.DrawLine(dashedPen, xPos, yPos, xPos + 300, yPos);
            yPos += 10;

            e.Graphics.DrawString($"({tongSoLuong}) San pham", fontContent, brush, xPos, yPos);
            e.Graphics.DrawString($"{guna2TextBox4.Text}", fontContent, brush, xPos + 205, yPos, formatLeft);
            yPos += 20;

            e.Graphics.DrawString("TIEN MAT", fontContent, brush, xPos, yPos);
            e.Graphics.DrawString($"{guna2TextBox3.Text}", fontContent, brush, xPos + 205, yPos, formatLeft);
            yPos += 20;

            e.Graphics.DrawString("TIEN THUA", fontContent, brush, xPos, yPos);
            e.Graphics.DrawString($"{guna2TextBox2.Text}", fontChange, brush, xPos + 195, yPos, formatLeft);
            yPos += 30;

            e.Graphics.DrawString("Chi xuat hoa don trong ngay", fontContent, brush, xPos + 150, yPos, formatCenter);
            yPos += 20;
            e.Graphics.DrawString("Xin cam on quy khach !", fontContent, brush, xPos + 150, yPos, formatCenter);
            e.HasMorePages = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Opacity += .2;
        }




        private void guna2TextBox3_Enter(object sender, EventArgs e)
        {

        }

        private void guna2TextBox3_Leave(object sender, EventArgs e)
        {
            if (!isHandlingTextChanged)
            {
                isHandlingTextChanged = true;

                string cleanedText = RemoveCurrencyText(guna2TextBox3.Text);

                if (double.TryParse(cleanedText, out double khachDua))
                {
                    guna2TextBox3.Text = FormatAsCurrency(khachDua);

                    if (double.TryParse(RemoveCurrencyText(guna2TextBox4.Text), out double tongThanhToan))
                    {
                        double tienThua = khachDua - tongThanhToan;
                        guna2TextBox2.Text = FormatAsCurrency(tienThua);
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng nhập tổng thanh toán hợp lệ.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    //guna2TextBox3.Text = guna2TextBox3.Text.Substring(0, guna2TextBox3.Text.Length - 1);
                }
                guna2TextBox3.SelectionStart = guna2TextBox3.Text.Length;
                isHandlingTextChanged = false;
            }
        }

        private void guna2TextBox3_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string cleanedText = RemoveCurrencyText(guna2TextBox3.Text);

            if (string.IsNullOrEmpty(guna2TextBox3.Text))
            {
                MessageBox.Show("Chưa nhập số tiền khách trả!");
                e.Cancel = true;
            }

            if (double.TryParse(cleanedText, out double khachDua))
            {


                if (ThongTinHoaDon.tongTienHD > khachDua)
                {
                    errorProvider1.SetError(guna2TextBox3, "Số tiền khách đưa không đủ!");                  
                    MessageBox.Show("Số tiền khách đưa không đủ!");
                    e.Cancel = true;


                }
                else
                    errorProvider1.SetError(guna2TextBox3, "");
            }
        }

        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == guna2DataGridView1.Columns["Column3"].Index && e.RowIndex != -1)
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
