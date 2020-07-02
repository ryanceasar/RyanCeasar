using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WA001.Classes;
using WA001.Models;

namespace WA001.Controllers
{
    public class HomeController : Controller
    {
        private List<PatientModel> GetPatients()
        {
            SqlCommand _SqlCommand = null;
            SqlParameter _SqlParameter;
            SqlConnection _SqlConnection = null;
            List<PatientModel> _List = new List<PatientModel>();
            using (DataSet dsRec = new DataSet())
            {
                _SqlCommand = new SqlCommand();
                _SqlCommand.CommandText = "procEMR_IP_PatientList";

                //_SqlParameter = new SqlParameter("@param", "param_val");
                //_SqlCommand.Parameters.Add(_SqlParameter);

                try
                {
                    _SqlConnection = new SqlConnection(GlobalVariables.ConnectionStringHIS);
                    _SqlCommand.Connection = _SqlConnection;
                    _SqlCommand.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter dataAdaptor = new SqlDataAdapter(_SqlCommand))
                    {
                        dataAdaptor.Fill(dsRec);
                    }
                    if (dsRec != null && dsRec.Tables.Count > 0 && dsRec.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsRec.Tables[0].Rows.Count; i++)
                        {
                            _List.Add(new PatientModel()
                            {
                                PatientName = Convert.ToString(dsRec.Tables[0].Rows[i]["PatientName"]),
                                Registration_No = Convert.ToString(dsRec.Tables[0].Rows[i]["Registration_No"]),
                                Ward_name = Convert.ToString(dsRec.Tables[0].Rows[i]["Ward_name"])
                            });
                        }
                    }
                }
                catch (Exception expCommon)
                {
                    return null;
                }
                finally
                {
                    if (_SqlConnection.State != ConnectionState.Closed)
                    {
                        _SqlConnection.Close();
                    }
                    _SqlParameter = null;
                    _SqlCommand = null;
                    _SqlConnection = null;
                }
                return _List;
            }
        }


        public ActionResult Index()
        {
            string vName = Path.GetRandomFileName();
            //return Redirect("Home/TestPage");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        static List<PatientModel> _List = new List<PatientModel>();
        public ActionResult TestPage()
        {
            _List = GetPatients().OrderBy(a => a.PatientName).ToList();
            //Session["PatientList"] = _List;
            return View(_List);
        }

        public ActionResult PatientList(string vFilter, List<PatientModel> _ListA)
        {
            //List<PatientModel> _NewList = new List<PatientModel>();
            //_NewList = GetPatients().Where(x => x.PatientName.Contains(vFilter.ToUpper())).OrderBy(x => x.PatientName).ToList();
            //List<PatientModel> _NewList = new List<PatientModel>();
            //_NewList = (List<PatientModel>)Session["PatientList"];

            List<PatientModel> _NewList = new List<PatientModel>();
            if (!string.IsNullOrEmpty(vFilter))
            {
                _NewList = _NewList = _List.Where(x => x.PatientName.Contains(vFilter.ToUpper())).OrderBy(x => x.PatientName).ToList();
            }
            return PartialView("_PatientList", _NewList);
        }

    }
}