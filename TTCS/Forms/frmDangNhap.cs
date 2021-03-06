using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLDSV
{
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {
            string chuoiketnoi = "Data Source=HP_PAVILION;Initial Catalog=" + Program.Database + ";Integrated Security=True";

            Program.Conn.ConnectionString = chuoiketnoi;
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {

            if (txtLogin.Text.Trim() == "" || txtPass.Text.Trim() == "")
            {
                XtraMessageBox.Show("Tài khoản đăng nhập không được trống", "Lỗi đăng nhập", MessageBoxButtons.OK);

               // trỏ con trỏ chuột về ô user...
               txtLogin.Focus();
                return;
            }

            Program.MLogin = txtLogin.Text;
            Program.MPassword = txtPass.Text;
            if (Program.KetNoi() == 0)
            {
                return;
            }
           
            //Program.MKhoa = cmbKhoa.SelectedIndex;// 0: CNTT ,  1: VT, 2: HỌC PHÍ

            Program.MLoginDN = Program.MLogin;
            Program.PasswordDN = Program.MPassword;

          
            String strLenh = "exec SP_DANGNHAP '" + Program.MLogin + "'";
            Program.MyReader = Program.ExecSqlDataReader(strLenh);
            if (Program.MyReader == null)
            {
                return;
            }
  
            Program.MyReader.Read();


            Program.UserName = Program.MyReader.GetString(0);     // Lay user name
            if (Convert.IsDBNull(Program.UserName))
            {
                XtraMessageBox.Show("Login bạn nhập không có quyền truy cập dữ liệu\nBạn xem lại username, password", "", MessageBoxButtons.OK);
                return;
            }

            try
            {
                Program.MHoten = Program.MyReader.GetString(1);
                Program.MGroup = Program.MyReader.GetString(2);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("---> Lỗi: " + ex.ToString());
                XtraMessageBox.Show("Login bạn nhập không có quyền truy cập vào chương trình", "", MessageBoxButtons.OK);
                return;
            }

            Program.MyReader.Close();
            Program.Conn.Close();

            // truy cập vào frm main 
            Program.frmMain = new frmMain();

            // hiện thông tin tài khoản
            Program.frmMain.lblMAGV.Text = "MÃ GIẢNG VIÊN : " + Program.UserName.Trim();
            Program.frmMain.lblHOTEN.Text = "HỌ VÀ TÊN : " + Program.MHoten.Trim();
            Program.frmMain.lblNHOM.Text = "NHÓM : " + Program.MGroup;

            Program.frmMain.Show();
            Program.FrmDangNhap.Visible = false;
        }

        private void chkShowPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = (chkShowPass.Checked) ? false : true;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
