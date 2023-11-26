using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Data;
using System.CodeDom.Compiler;

namespace CodeGenerate.ViewModel
{
    public class MainVM
    {
        private SqlConnection cnn;
        private ObservableCollection<string> DBNames;
        private ObservableCollection<string> TableNames;

        public MainVM() 
        {
        }

        private void OpenDatabase()
        {
            try
            {
                cnn = new SqlConnection("server=LAPTOP-S65PUQCG\\SQLEXPRESS01;integrated security=true");
                cnn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database bağlanırken sorun oluştu!.\n{ex.Message}", "Error");
            }
        }

        private void OpenDatabaseWithDBname(string dbName)
        {
            try
            {
                cnn = new SqlConnection($"server=LAPTOP-S65PUQCG\\SQLEXPRESS01; database={dbName}; integrated security=true");
                cnn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database bağlanırken sorun oluştu!.\n{ex.Message}", "Error");
            }
        }

        public ObservableCollection<string> GetDatabaseName()
        {

            DBNames = new ObservableCollection<string>();
            OpenDatabase();
            if (cnn.State == ConnectionState.Open)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT name from sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb');";
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    string tableName = dr["name"].ToString();
                    DBNames.Add(tableName);
                }
                cnn.Close();
            }
            return DBNames;
        }

        public ObservableCollection<string> GetTablesNames(string dbNames, string shema)
        {

            TableNames = new ObservableCollection<string>();
            OpenDatabaseWithDBname(dbNames);
            if (cnn.State == ConnectionState.Open)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                foreach (var tableName in cnn.GetSchema("Tables").AsEnumerable().Where(x => x[1].ToString() == shema).Select(s => s[2].ToString()).ToList())
                {
                    TableNames.Add(tableName);
                }
                cnn.Close();
            }
            return TableNames;
        }



        public ObservableCollection<string> GetShemasNames(string dbNames)
        {

            TableNames = new ObservableCollection<string>();
            OpenDatabaseWithDBname(dbNames);
            if (cnn.State == ConnectionState.Open)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                foreach (var tableName in cnn.GetSchema("Tables").AsEnumerable().Select(s => s[1].ToString()).Distinct().ToList())
                {
                    TableNames.Add(tableName);
                }
                cnn.Close();
            }
            return TableNames;
        }



        public string CodeGenerate(string dbNames,string shemaName, string tableName)
        {
            string codeGenerate = "";
            OpenDatabaseWithDBname(dbNames);
            if (cnn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cnn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = $"[{shemaName}].[ModelCodeGenerate]";
                    cmd.Parameters.Add("@TableName", SqlDbType.VarChar).Value = tableName;
                    cmd.Parameters.Add("@TableSchema", SqlDbType.VarChar).Value = shemaName;
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        codeGenerate = dr["result"].ToString();
                    }
                    cnn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return codeGenerate;
        }


        public string VMCodeGenerate(string dbNames, string shemaName, string tableName)
        {
            string codeGenerate = "";
            OpenDatabaseWithDBname(dbNames);
            if (cnn.State == ConnectionState.Open)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cnn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = $"[{shemaName}].[VMModelCodeGenerate]";
                    cmd.Parameters.Add("@TableName", SqlDbType.VarChar).Value = tableName;
                    cmd.Parameters.Add("@TableSchema", SqlDbType.VarChar).Value = shemaName;
                    cmd.Parameters.Add("@Database", SqlDbType.VarChar).Value = dbNames;
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        codeGenerate = dr["result"].ToString();
                    }
                    cnn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return codeGenerate;
        }
    }
}
