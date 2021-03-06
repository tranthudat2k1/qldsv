using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;

// sử dụng kiểu kết nối với Database là sqlclient
using System.Data.SqlClient;
using System.Data;
using DevExpress.XtraEditors;

namespace QLDSV
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>


        // dùng để thực thi lệnh
        public static SqlCommand Sqlcmd = new SqlCommand();

        // tạo đối tượng kết nối Conn , kêt nối Database bằng mã lệnh
        public static SqlConnection Conn = new SqlConnection();

        // chuỗi kết nối connection string để kết nối với csdl , nó bước đầu tiên để thực hiện kết nối      
        public static String URL_Connect;

        public static SqlDataReader MyReader;

        // những dòng này dùng trong phần tạo connection string ở bên dưới
        public static String ServerName = string.Empty;
        public static String UserName = string.Empty;


        // lưu các login và password từ các form khi chương trình chạy.
        public static String MLogin = string.Empty;
        public static String MPassword = string.Empty;

        // RemoteLogin này là remote dùng để hỗ trợ kết nối ra ngoài ví dụ trong quá trình đăng nhập nó sẽ rẽ qua server 2
        // để đăng nhập truy vấn dữ liệu thì nó dùng login này để kết nối(hay là tạo link server)
        // vì nó giống nhau trên các phân mảnh là HTKN nối nó sẽ gán cứng vào.
        public static String RemoteLogin = "HTKN";
        public static String RemotePassword = "123";
        public static String Database = "QLDSV";

        //MLoginDN là mã login đăng nhập và mật khẩu của nó
        public static String MLoginDN = string.Empty;
        public static String PasswordDN = string.Empty;

        // 3 Mgroup , MHoten, MKhoa dùng để hiển thi thông tin login vào
        // MGroup là mã nhóm quyền khi của login đó đăng nhập vào.
        public static String MGroup = string.Empty;

        // MHoten là mã họ tên của login đăng nhập vào 
        public static String MHoten = string.Empty;

        //MKhoa cho biết hiện tại khoa ta đăng nhập vô là khoa nào.
        public static int MKhoa = 0;

        //biến dùng để chứa danh sách các phân mãnh từ viewDSPM (bằng code, ko kéo thả)
        public static BindingSource Bds_Dspm = new BindingSource(); //giu DSPM khi dang nhap

        // lưu các đối tượng form Main và form FrmDangNhap để thực hiển xử lý nghiệp vụ chuyển đổi từ frmMain sang frmDangNhap và ngược lại.
        public static frmMain frmMain;
        public static frmDangNhap FrmDangNhap;

        // lưu danh sách các nhóm quyền
        public static string[] NhomQuyen = new string[4] {"PGV", "KHOA", "PKeToan","ADMIN"};
       

        // hàm thực hiện kết nối tới Database
        public static int KetNoi()
        {
            if (Program.Conn != null && Program.Conn.State == ConnectionState.Open)
                // đóng đối tượng kết nối
                Program.Conn.Close();
            try
            {
                Program.URL_Connect = "Data Source= HP_PAVILION;Initial Catalog=" +
                      Program.Database + ";User ID=" +
                      Program.MLogin + ";Password=" + Program.MPassword;
                Program.Conn.ConnectionString = Program.URL_Connect;

                // mở đối tượng kết nối
                Program.Conn.Open();
                return 1;
            }

            catch (Exception e)
            {
                XtraMessageBox.Show("---> Lỗi kết nối cơ sở dữ liệu.\n---> Bạn xem lại Username và Password.\n " + e.Message, string.Empty, MessageBoxButtons.OK);
                return 0;
            }
        }


        // ExecSqlDataReader tôc độ tải về nhanh hơn ExecSqlDataTable vì đối tượng nó chỉ quam tân chỉ select
        // chỉ duyệt 1 chiều từ trên xuống
        // vì vậy trong nghiệp vụ form báo cáo thì dùng datareader
        // https://youtu.be/z8pgdIbtV3E?t=3233

        public static SqlDataReader ExecSqlDataReader(String strLenh)
        {
            SqlDataReader myReader;
            SqlCommand sqlcmd = new SqlCommand(strLenh, Program.Conn);

            //xác định kiểu lệnh cho sqlcmd là kiểu text.
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandTimeout = 600;
            if (Program.Conn.State == ConnectionState.Closed) Program.Conn.Open();
            try
            {
                myReader = sqlcmd.ExecuteReader();
                return myReader;
            }
            catch (SqlException ex)
            {
                Program.Conn.Close();
                XtraMessageBox.Show(ex.Message);
                return null;
            }
        }

        // tải về cho phép xem xóa sửa ==> tốc độ tải chậm hơn cái ở trên
        // duyệt 2 chiều dưới lên
        // form nhập liệu thì dùng datatable.

        public static DataTable ExecSqlDataTable(String cmd)
        {
            DataTable dt = new DataTable();
            if (Program.Conn.State == ConnectionState.Closed) Program.Conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd, Conn);
            da.Fill(dt);
            Conn.Close();
            return dt;
        }
        public static int ExecSqlNonQuery(String strlenh)
        {
            SqlCommand Sqlcmd = new SqlCommand(strlenh, Conn);
            Sqlcmd.CommandType = CommandType.Text;
            Sqlcmd.CommandTimeout = 600;// 10 phut
            if (Conn.State == ConnectionState.Closed) Conn.Open();
            try
            {
                Sqlcmd.ExecuteNonQuery();
                Conn.Close();
                return 0;
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message, "Error Message",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                Conn.Close();
                return ex.State;

            }
        }
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BonusSkins.Register();
            SkinManager.EnableFormSkins();
           
            Program.FrmDangNhap = new frmDangNhap();
            Application.Run(Program.FrmDangNhap);

        }
    }
}
