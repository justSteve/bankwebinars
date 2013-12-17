using System.Web.Mvc;

namespace CUWebinars.Web.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult NotFound()
        {


            //try
            //{
            //    Type comLogQueryType = Type.GetTypeFromProgID("MSUtil.LogQuery", true);
            //    object comLogQueryObject = Activator.CreateInstance(comLogQueryType);

            //    // Get the IIS Input and XML output filters
            //    Type ws3LogType = Type.GetTypeFromProgID("MSUtil.LogQuery.IISW3CInputFormat", true);
            //    object ws3LogObject = Activator.CreateInstance(ws3LogType);
            //    Type xmlLogType = Type.GetTypeFromProgID("MSUtil.LogQuery.XMLOutputFormat", true);
            //    object xmlLogObject = Activator.CreateInstance(xmlLogType);

            //    // Setup input and output files
            //    string inPath = "someIISlog.log";
            //    string outpath = "temp.xml";

            //    // Create a SQL query to get the referers, count and uri-to. Order by total hits
            //    string query = "SELECT cs(Referer) as Referer,cs-uri-stem as To,COUNT(*) as Total from " + inPath + " TO " + outpath + "WHERE (sc-status=200) AND (Referer LIKE 'http:%') GROUP BY Referer,To ORDER BY Total DESC";

            //    // Invoke the ExcuteBatch method
            //    object[] inputArgs = { query, ws3LogObject, xmlLogObject };
            //    comLogQueryType.InvokeMember("ExecuteBatch", BindingFlags.InvokeMethod, null, comLogQueryObject, inputArgs);
            //}
            //catch (Exception e)
            //{
            //    string errorString = "An exception has occurred: " + e.Message;
            //    Console.WriteLine(errorString);
            //}
            //try
            //{
            //    Type comLogQueryType = Type.GetTypeFromProgID("MSUtil.LogQuery", true);
            //    object comLogQueryObject = Activator.CreateInstance(comLogQueryType);

            //    // Get the IIS Input and XML output filters
            //    Type ws3LogType = Type.GetTypeFromProgID("MSUtil.LogQuery.IISW3CInputFormat", true);
            //    object ws3LogObject = Activator.CreateInstance(ws3LogType);
            //    Type xmlLogType = Type.GetTypeFromProgID("MSUtil.LogQuery.XMLOutputFormat", true);
            //    object xmlLogObject = Activator.CreateInstance(xmlLogType);

            //    // Setup input and output files
            //    string inPath = "someIISlog.log";
            //    string outpath = "temp.xml";

            //    // Create a SQL query to get the referers, count and uri-to. Order by total hits
            //    string query = "SELECT cs(Referer) as Referer,cs-uri-stem as To,COUNT(*) as Total from " + inPath + " TO " + outpath + " WHERE (sc-status=200) AND (Referer LIKE 'http:%') GROUP BY Referer,To ORDER BY Total DESC";

            //    // Invoke the ExcuteBatch method
            //    object[] inputArgs = { query, ws3LogObject, xmlLogObject };
            //    comLogQueryType.InvokeMember("ExecuteBatch", BindingFlags.InvokeMethod, null, comLogQueryObject, inputArgs);
            //}
            //catch (Exception e)
            //{
            //    string errorString = "An exception has occurred: " + e.Message;
            //    Console.WriteLine(errorString);
            //}
            Response.StatusCode = 404;  //you may want to set this to 200
            return View("NotFound");
        }

    }
}
