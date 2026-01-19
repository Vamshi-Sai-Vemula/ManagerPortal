using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace _10X_ManagerPortal
{
    public class Helper
    {
        private string _connectionString;

        public Helper()
        {
            _connectionString = ConfigurationManager.AppSettings["HanaCon"];
        }

        private HanaConnection GetConnection()
        {
            return new HanaConnection(_connectionString);
        }

        // ================= ExecuteScalar =================
        public object ExecuteScalar(string query, List<HanaParameter> parameters = null)
        {
            using (var con = GetConnection())
            using (var cmd = new HanaCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters.ToArray());

                con.Open();
                return cmd.ExecuteScalar();
            }
        }

        // ================= ExecuteNonQuery =================
        public int ExecuteNonQuery(string query, List<HanaParameter> parameters = null)
        {
            using (var con = GetConnection())
            using (var cmd = new HanaCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters.ToArray());

                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // ================= ExecuteDataTable =================
        public DataTable ExecuteDataTable(string query, List<HanaParameter> parameters = null)
        {
            DataTable dt = new DataTable();

            using (var con = GetConnection())
            using (var cmd = new HanaCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters.ToArray());

                using (var da = new HanaDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        // ================= ExecuteDataSet =================
        public DataSet ExecuteDataSet(string query, List<HanaParameter> parameters = null)
        {
            DataSet ds = new DataSet();

            using (var con = GetConnection())
            using (var cmd = new HanaCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters.ToArray());

                using (var da = new HanaDataAdapter(cmd))
                {
                    da.Fill(ds);
                }
            }

            return ds;
        }
    }
}