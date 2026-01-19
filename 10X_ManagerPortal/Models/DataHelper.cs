using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using Sap.Data.Hana;

namespace _10X_ManagerPortal.Models
{
    public class DataHelper
    {
        HanaConnection hCon;
        public DataHelper()
        {
            hCon = new HanaConnection(System.Configuration.ConfigurationManager.AppSettings.Get("HanaCon").ToString());

        }
        public int ExecuteNonQuery(string sQry)
        {
            int i = 0;
            try
            {
                hCon.Open();
                HanaCommand cmd = new HanaCommand(sQry, hCon);
                i = cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (hCon.State == System.Data.ConnectionState.Open) hCon.Close();
            }

            return i;
        }
        public object ExecuteScalar(string sQry)
        {
            object objVar = null;
            try
            {
                hCon.Open();
                HanaCommand cmd = new HanaCommand(sQry, hCon);
                objVar = cmd.ExecuteScalar();

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (hCon.State == System.Data.ConnectionState.Open) hCon.Close();
            }

            return objVar;
        }
        public object ExecutesScalar(string sQry, int userId)
        {
            object objVar = null;
            try
            {
                hCon.Open();
                HanaCommand cmd = new HanaCommand(sQry, hCon);
                objVar = cmd.ExecuteScalar();

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (hCon.State == System.Data.ConnectionState.Open) hCon.Close();
            }

            return objVar;
        }
        public DataSet getDataSet(string sQry)
        {
            DataSet dsData = null;
            try
            {

                HanaCommand cmd = new HanaCommand(sQry, hCon);
                HanaDataAdapter hda = new HanaDataAdapter(cmd);
                dsData = new DataSet();
                hda.Fill(dsData, "ds");

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (hCon.State == System.Data.ConnectionState.Open) hCon.Close();
            }

            return dsData;
        }
        public DataSet getDataSetByCompany(string sQry, string companyKey)
        {
            hCon = new HanaConnection(System.Configuration.ConfigurationManager.AppSettings.Get(companyKey).ToString());
            DataSet dsData = null;
            try
            {

                HanaCommand cmd = new HanaCommand(sQry, hCon);
                HanaDataAdapter hda = new HanaDataAdapter(cmd);
                dsData = new DataSet();
                hda.Fill(dsData, "ds");

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (hCon.State == System.Data.ConnectionState.Open) hCon.Close();
            }

            return dsData;
        }

        public DataTable ExecuteDataTable(string sQry, List<Sap.Data.Hana.HanaParameter> parameters = null)
        {
            DataTable dt = new DataTable();

            try
            {
                hCon.Open();

                HanaCommand cmd = new HanaCommand(sQry, hCon);

                // 💡 ADD PARAMETERS: Attach the parameters to the command object
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                HanaDataAdapter da = new HanaDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (hCon.State == ConnectionState.Open)
                    hCon.Close();
            }

            return dt;
        }
        public List<decimal> ExecuteDecimalList(string query)
        {
            List<decimal> list = new List<decimal>();

            DataSet ds = getDataSet(query);

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(row[0] == DBNull.Value ? 0m : Convert.ToDecimal(row[0]));
                }
            }

            return list;
        }



    }
}