using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Net.Mail;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using static SampleAPI.Controllers.AllClassesController;

namespace SampleAPI.Controllers
{
    [RoutePrefix("api")]
    public class ValuesController : ApiController
    {
        // Entity Framework
        //DatabaseEntities dbMyDatabase = new DatabaseEntities();
        string customValue = System.Web.Configuration.WebConfigurationManager.AppSettings["CustomValue"].ToString();

        #region All HTTP Requests

        [HttpGet]
        [Route("datalist")]
        public IHttpActionResult DataList()
        {
            try
            {
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("STOREDPROCEDURENAME", con))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@value", customValue);
                            if (con.State == ConnectionState.Closed)
                                con.Open();
                            DataTable dt = new DataTable();
                            var reader = cmd.ExecuteReader();
                            dt.Load(reader);
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(">>> ERROR: " + ex.Message);
                        }
                    }
                }
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        //[Authorize]
        [Route("infolist")]
        public IHttpActionResult InfoList([FromBody] DetailsList _DetailsList)
        {
            try
            {
                List<dynamic> resultList = new List<dynamic>();
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("STOREDPROCEDURENAME", connection))
                    {
                        try
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@value", customValue);

                            if (connection.State == ConnectionState.Closed)
                                connection.Open();
                            DataTable dt = new DataTable();
                            var reader = command.ExecuteReader();
                            dt.Load(reader);
                            //if (dt.Rows.Count > 0)
                            //{
                            //    for (int i = 0; i < dt.Rows.Count; i++)
                            //    {
                                    // Function ...
                            //    }
                            //}
                            //else
                            //{
                            //    System.Diagnostics.Debug.WriteLine(">>> ERROR: no rows");
                            //}
                            connection.Close();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(">>> ERROR: " + ex.Message);
                        }
                    }
                }
                return Ok(resultList);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        //[Authorize]
        [Route("detailslist")]
        public IHttpActionResult DetailsList([FromBody] DetailsList _DetailsList)
        {
            try
            {
                // Using Entity Framework
                //var Result = dbMyDatabase.StoredProcedure_GetDetails().ToList();
                //return Json(Result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        //[Authorize]
        [Route("FileUpload")]
        public HttpResponseMessage FileUpload()
        {
            try
            {
                HttpResponseMessage result = null;
                var httpRequest = HttpContext.Current.Request;
                string user = httpRequest["User"];
                if (httpRequest.Files.Count > 0)
                {
                    string path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Files/");
                    bool exists = System.IO.Directory.Exists(path);
                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var filePath = HttpContext.Current.Server.MapPath(@"~/User/" + user);
                        postedFile.SaveAs(filePath);
                    }
                    result = Request.CreateResponse(HttpStatusCode.Created, "Success");
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        #endregion

        #region User Defined Functions

        public void SendEmail(string EmailAddress)
        {
            string mailSubject = "Test Email";
            DateTime dateTime = DateTime.UtcNow.Date;
            string path = HttpContext.Current.Server.MapPath(@"~/EmailTemplate.html");
            string mailBody = File.ReadAllText(path);
            mailBody = mailBody.Replace("[YYYY-MM-DD]", dateTime.ToString("yyyy/MM/dd"));
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient()
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000, //10 seconds
                Host = "EMAILSERVERIPADDRESS",
                Port = 25
            };
            mail.IsBodyHtml = true;
            mail.To.Add(EmailAddress);
            mail.From = new MailAddress("example@email.com");
            mail.Subject = mailSubject;
            mail.Body = mailBody;
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(">>> Email Error Message: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine(">>> Email Inner Error Message: " + ex.InnerException);
                }
            }
        }

        #endregion
    }
}
