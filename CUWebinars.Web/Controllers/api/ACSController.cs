//using System.Configuration;
//using System.Data;
//using System.Data.RSSBus.Gmail;

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Web.Http;
//using CUWebinars.Business.AccountService;
//using CUWebinars.Business.Models;
//using CUWebinars.Business.Services;
//using CUWebinars.Web.Core.Orchestrators;
//using CUWebinars.Web.Helpers;
//using HtmlAgilityPack;
//using log4net.Repository.Hierarchy;
//using Ninject.Extensions.Logging;

//namespace CUWebinars.Web.Controllers.api
//{
//    public class ACSController : ApiController
//    {
//        private readonly IMembershipService _or;
//        private readonly ILogger _logger;
//        private bool _disposed;

//        private readonly IOrderControllerOrchestrator _orderControllerOrchestrator;
//        private IOrderManagementService _orderManagementService;
//        private IWebinarManagementService _webinarManagementService;
//        private readonly IAppHelper _appHelper;

//        private readonly GmailConnection _conn = new GmailConnection(
//                    ConfigurationManager.ConnectionStrings["RSSBUS_Affiliate"].ConnectionString);


//        public ACSController(ILogger logger, IOrderControllerOrchestrator orderControllerOrchestrator, IOrderManagementService orderManagementService, IWebinarManagementService webinarManagementService, IAppHelper appHelper)
//            : base()
//        {
//            _logger = logger;
//            _orderControllerOrchestrator = orderControllerOrchestrator;
//            _orderManagementService = orderManagementService;
//            _webinarManagementService = webinarManagementService;
//            _appHelper = appHelper;
//        }

//        // GET: api/ACS
//        public IEnumerable<string> Get()
//        {
//            var mLic = new GmailConnection().RuntimeLicense;
//            ParseEmails_ACS();
//            return new string[] { "value1", "value2" };
//        }

//        // GET: api/ACS/5
//        public string Get(int id)
//        {
//            return "value";
//        }

//        public virtual int ParseMsgBody(string msgBody, string orderDate)
//        {

            
//            var doc = new HtmlAgilityPack.HtmlDocument();
//            doc.LoadHtml(msgBody);

//            var root = doc.DocumentNode;

//            var keyRows = root.SelectNodes("//*//tr[contains(@bgcolor, 'FFFFFF')]/td");
//            var valueRows = root.SelectNodes("//*//tr[contains(@bgcolor, 'EAF2FA')]/td");
            

//            //var keyRows = root.SelectNodes("//*[table]/child::table[1]/tbody[1]/tr/td/table/tbody/tr[@bgcolor='#EAF2FA']/td");
//            //var valueRows = root.SelectNodes("//*[table]/child::table[1]/tbody[1]/tr/td/table/tbody/tr[@bgcolor='#FFFFFF']/td");

//            var keys = keyRows.Select(row => row.InnerText.Trim()).ToList();

//            var values = (from row in valueRows where !row.InnerText.Trim().Equals("&nbsp;") select row.InnerText.Trim()).ToList();

//            var kvs = keys.Zip(values, (k, v) => new KeyValuePair<string, string>(k, v));
//            var dic = kvs.ToDictionary(b => b.Key);

//            var sb = new StringBuilder();
//            foreach (var keyValuePair in kvs)
//            {
//                sb.Append(string.Format("{0} : {1}{2}", keyValuePair.Key, keyValuePair.Value, Environment.NewLine));
//                Console.WriteLine("Key: {0}, Value: {1}", keyValuePair.Key, keyValuePair.Value);
//            }



//            //keys.ForEach(Console.WriteLine);
//            //values.ForEach(Console.WriteLine);
//            DataTable dataTable = new DataTable();
//            dataTable.Columns.Add("AffiliateID", typeof(string)); //
//            dataTable.Columns.Add("WebinarID", typeof(string)); //
//            dataTable.Columns.Add("idRegType", typeof(string)); //
//            dataTable.Columns.Add("FirstName", typeof(string));
//            dataTable.Columns.Add("LastName", typeof(string));
//            dataTable.Columns.Add("Title", typeof(string));
//            dataTable.Columns.Add("Institution", typeof(string));
//            dataTable.Columns.Add("Email", typeof(string));
//            dataTable.Columns.Add("Phone", typeof(string));
//            dataTable.Columns.Add("Address", typeof(string));
//            dataTable.Columns.Add("Address2", typeof(string));
//            dataTable.Columns.Add("City", typeof(string));
//            dataTable.Columns.Add("State", typeof(string));
//            dataTable.Columns.Add("Zip", typeof(string));
//            dataTable.Columns.Add("DiscountCode", typeof(string)); //
//            dataTable.Columns.Add("AdditionalLocations", typeof(string));//
//            dataTable.Columns.Add("firstname", typeof(string));
//            dataTable.Columns.Add("lastname", typeof(string));
//            dataTable.Columns.Add("St", typeof(string));
//            dataTable.Columns.Add("total", typeof(string));
//            dataTable.Columns.Add("shipmentDate", typeof(string)); //
//            dataTable.Columns.Add("StoreComments", typeof(string)); //
//            dataTable.Columns.Add("idOrder", typeof(string));//
//            dataTable.Columns.Add("orderDate", typeof(string));//
//            dataTable.Columns.Add("status", typeof(string));//

//            var newRow = dataTable.NewRow();
//            newRow["FirstName"] = dic["First Name"].Value;
//            newRow["LastName"] = dic["Last Name"].Value;
//            newRow["Title"] = dic["Title"].Value;
//            newRow["Institution"] = dic["Company"].Value;
//            newRow["Email"] = dic["Email"].Value;
//            newRow["Phone"] = dic["Phone"].Value;
//            newRow["Address"] = dic["Street or P.O. Box"].Value;
//            newRow["Address2"] = string.Empty;
//            newRow["City"] = dic["City"].Value;
//            newRow["State"] = dic["State"].Value;
//            newRow["Zip"] = dic["Zip / Postal Code"].Value;
//            newRow["St"] = dic["Street or P.O. Box"].Value;
//            newRow["total"] = dic["Order"].ToString().Substring(dic["Order"].ToString().IndexOf("Total:") + 6).Trim().Replace("]", "");


//            int idRegType = _webinarManagementService.GetRegTypeByACS(dic["Delivery Type"].Value,
//                    Convert.ToInt32(dic["BankWebID"].Value));


//            using (var sw = new StreamWriter("OrderMigrator.csv", true))
//            {
//                sw.AutoFlush = true;

//                sw.Write("AffiliateID,WebinarID,idRegType,FirstName,LastName,Title,Institution,Email,Phone,Address,Address2,	City,State,Zip,DiscountCode,AdditionalLocations,firstname,lastname,phone,address,city,St,Zip,total," +
//                    "shipmentDate,StoreComments,idOrder,orderDate,status{0}", Environment.NewLine
//                    );

//                sw.Write(19); // AffiliateId
//                sw.Write(dic["BankWebID"].Value); // WebinarId
//                sw.Write(idRegType); // idRegType
//                sw.Write(newRow["FirstName"] + ",");
//                sw.Write(newRow["LastName"] + ",");
//                sw.Write(newRow["Title"] + ",");
//                sw.Write(newRow["Institution"] + ",");
//                sw.Write(newRow["Email"] + ",");
//                sw.Write(newRow["Phone"] + ",");
//                sw.Write(newRow["Address"] + ",");
//                sw.Write(newRow["Address2"] + ",");
//                sw.Write(newRow["City"] + ",");
//                sw.Write(newRow["State"] + ",");
//                sw.Write(newRow["Zip"] + ",");
//                sw.Write(" , "); //DiscountCode
//                sw.Write(" , "); //AdditionalLocations
//                sw.Write(newRow["FirstName"] + ",");
//                sw.Write(newRow["LastName"] + ",");
//                sw.Write(newRow["Phone"] + ",");
//                sw.Write(newRow["address"] + ",");
//                sw.Write(newRow["City"] + ",");
//                sw.Write(newRow["St"] + ",");
//                sw.Write(newRow["Zip"] + ",");
//                sw.Write(newRow["total"] + ",");
//                sw.Write(" , "); // shipmentDate
//                sw.Write(" , "); // StoreComments
//                sw.Write(" , "); // idOrder
//                sw.Write(orderDate); // OrderDate
//                sw.Write(" , "); // status

//            }

//            Console.ReadKey();
//            return 0;
//        }

//        public virtual int ParseEmails_ACS()
//        {

//            var gda = new GmailDataAdapter
//            {
//                SelectCommand = new GmailCommand(
//                    //"SELECT * FROM sys_tables", 
//                    "SELECT id from [Gmail/All Mail] where Date > '1-1-2015' and SEARCHCRITERIA = 'SUBJECT \"Webinar Registration\"'",
//                    //AND ([From] = test1@email.com OR [From] = test2@email.com) AND Date > '1-1-2012'
//                    //"SELECT id from MailMessages where SEARCHCRITERIA " +
//                    //"= 'UNSEEN SUBJECT \"Webinar Registration\"' ",
//                    _conn)
//            };
//            var AreErrors = "";
//            var MyGuid = "";
//            var msgThreadID = "";
//            var msgDate = "";
//            try
//            {
//                var reader = gda.SelectCommand.ExecuteReader();

//                while (reader.Read())
//                {
//                    AreErrors = "";
//                    //
//                    int readerID = Convert.ToInt32(reader["id"]);

//                    if (readerID < 1)
//                    {
//                        break;
//                    }
//                    var gda1 = new GmailDataAdapter()
//                    {

//                        SelectCommand = new GmailCommand(
//                            "SELECT * from [Gmail/All Mail] where id = " + readerID,
//                            _conn)
//                    };
//                    var reader1 = gda1.SelectCommand.ExecuteReader();
//                    var sb = new StringBuilder();
//                    while (reader1.Read())
//                    {
//                        {
//                            Console.WriteLine("=================================================");
//                            for (int i = 0; i < reader1.FieldCount; i++)
//                            {
//                                Console.WriteLine(reader1.GetName(i) + ": " + reader1.GetValue(i));
//                                sb.Append(reader1.GetName(i) + ": " + reader1.GetValue(i) + Environment.NewLine);
//                            }
//                        }

//                        ParseMsgBody("<html><body>" + reader1["MessageBody"].ToString() + "</html></body>", reader1["Date"].ToString());
//                    }
//                }
//            }
//            catch
//            {
//                throw;
//            }
//            return 0;
//        }



//        // POST: api/ACS
//        public
//        void Post([FromBody]string value)
//        {


//        }

//        // PUT: api/ACS/5
//        public void Put(int id, [FromBody]string value)
//        {
//        }

//        // DELETE: api/ACS/5
//        public void Delete(int id)
//        {
//        }
//    }
//}
