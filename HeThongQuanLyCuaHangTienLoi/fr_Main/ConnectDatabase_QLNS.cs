using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace fr_Main
{
    class ConnectDatabase_QLNS
    {
        SqlConnection Con;
        public ConnectDatabase_QLNS()
        {
            string strConnect = "Data Source=DESKTOP-OKUSEB8;Initial Catalog=SkyPOS_DATA;Integrated Security=True";
            Con = new SqlConnection(strConnect);
        }
        public SqlConnection connect()
        {
            return Con;
        }

        public void Open()
        {
            if (Con.State == ConnectionState.Closed)
                Con.Open();
        }

        public void Close()
        {
            if (Con.State == ConnectionState.Open)
                Con.Close();
        }

        public void GetNonQuery(string sql, params SqlParameter[] parameters)
        {
                Open();
                SqlCommand cmd = new SqlCommand(sql, Con);
                cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();
                Close();
        }

        public DataTable getDataTable(string sql)
        {
            Open();
            DataTable table = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, Con);
            da.Fill(table);
            Con.Close();
            return table;
        }

        public SqlDataReader getDataReader(string sql)
        {
            Open();
            SqlCommand cmd = new SqlCommand(sql, Con);
            SqlDataReader reader = cmd.ExecuteReader();
            DataTable table = new DataTable();
            return reader;
        }

        public int updateDataBase(DataTable dt, string sql)
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, Con);
            SqlCommandBuilder cb = new SqlCommandBuilder(da);
            int kq = da.Update(dt);
            return kq;
        }

        public int updateDatabase(DataTable dt, string query)
        {
            SqlDataAdapter da = new SqlDataAdapter(query, Con);
            SqlCommandBuilder cB = new SqlCommandBuilder(da);
            int kq = da.Update(dt);
            return kq;
        }
        public object getScalar(string sql)
        {
            Open();
            SqlCommand cmd = new SqlCommand(sql, Con);
            object kq = cmd.ExecuteScalar();
            Close();
            return kq;
        }
    }
}
