using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace SmartPermitService
{
    public class Xml1
    {
        public string @version { get; set; }
        public string @encoding { get; set; }
    }

    public class Xml
    {
        public string error { get; set; }
        public string status_code { get; set; }
        public string message { get; set; }
        public string reference_no { get; set; }
    }

    public class Root
    {
        public Xml1 xml1 { get; set; }
        public Xml xml { get; set; }
    }


    public partial class SmartPermitService : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)  
        private static object _intervalSync = new object();
        public SmartPermitService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            //UpdateZatparkLogs("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile("Service is recall at " + DateTime.Now);
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    //sw.WriteLine(Message);
                    Common.InfoLogs("Before fetching sites");
                    DataSet dssites = null;
                    try
                    {
                        dssites = Util.GetSites();
                    }
                    catch (Exception e)
                    {
                        Common.InfoLogs("error fetching sites" + e.ToString());
                    }

                    //  UpdateZatparkLogs("");
                }
            }
        }
        public void UpdateZatparkLogs(string Message)
        {
           
                try
                {
                    Common.InfoLogs("ZatparkUpdate method was started");
                    // timer.Enabled = false;
                    //Timer.Stop();
                    Common.InfoLogs("Config files read");
                    DateTime dt = DateTime.Now;
                   
                    string apikey = string.Empty; string apiurl = string.Empty;

                    try
                    {
                         apikey = System.Configuration.ConfigurationManager.AppSettings["ZatparkApiKey"];
                         apiurl = System.Configuration.ConfigurationManager.AppSettings["ZatparkUrl"];
                    }
                    catch (Exception e)
                    {
                        Common.InfoLogs("error fetching sites" + e.ToString());
                    }
                   
                    DataSet ds = new DataSet();
                    Common.InfoLogs("Config files read");
                    string referenceno = string.Empty;
                    string referencenew = string.Empty;
                    string sitecode = string.Empty;
                    string startdate = string.Empty;
                    string enddate = string.Empty;
                    string vrm = string.Empty;
                    Common.InfoLogs("Before fetching sites");
                    DataSet dssites = null;
                    try
                    {
                        dssites = Util.GetSites();
                    }
                    catch(Exception e)
                    {
                        Common.InfoLogs("error fetching sites" + e.ToString());
                    }
                   
                    Common.InfoLogs("site Fetched");
                    if (dssites != null && dssites.Tables.Count > 0 && dssites.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow item in dssites.Tables[0].Rows)
                        {
                            int SiteId = Convert.ToInt32(item["Id"]);
                            bool zatparkhours = Convert.ToBoolean(item["Zatparklogs24hrs"]);

                            ArrayList array = new ArrayList();
                            array.Add(SiteId);
                            //if (zatparkhours)
                            //{
                            array.Add(1);
                            ds = Util.GetVehicleDetails(array);
                            Common.InfoLogs("Getvehicles was excuted " );
                            //}
                            //else
                            //{
                            //    array.Add(2);
                            //    ds = Util.GetVehicleDetails(array);
                            //}
                            if (ds != null && ds.Tables[0].Rows.Count > 0 && ds.Tables.Count > 0)
                            {

                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    referenceno = ds.Tables[0].Rows[i]["Id"].ToString();
                                    //referencenew = ds.Tables[0].Rows[i]["ReferenceNo"].ToString();
                                    sitecode = ds.Tables[0].Rows[i]["Sitecode"].ToString();
                                    //sitecode = "eur0011-500";
                                    startdate = Convert.ToDateTime(ds.Tables[0].Rows[i]["FromDate"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ds.Tables[0].Rows[i]["FromDate"]).ToString("HH:mm");
                                    enddate = Convert.ToDateTime(ds.Tables[0].Rows[i]["ToDate"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ds.Tables[0].Rows[i]["ToDate"]).ToString("HH:mm");
                                    vrm = ds.Tables[0].Rows[i]["VRM"].ToString();
                                    Common.InfoLogs("ZatparkUpdate method was started vrm= " + vrm + " referencenew= " + referenceno + " startdate= " + startdate + " enddate= " + enddate);

                                    string requeststr = "{\"reference_no\":\"" + referenceno + "\",\"site_code\":\"" + sitecode + "\",\"start_date\":\"" + startdate + "\",\"end_date\":\"" + enddate + "\",\"vehicle_details\":{\"vrm\":\"" + vrm + "\"}}";
                                    //if (Convert.ToDateTime(ds.Tables[0].Rows[i]["FromDate"]).ToString("HH:mm") == Convert.ToDateTime(ds.Tables[0].Rows[i]["ToDate"]).ToString("HH:mm"))
                                    //{
                                    //    Common.InfoLogs("FromTime and ToTime are equal");
                                    //}
                                    //else
                                    //{
                                    RestClient client = new RestClient(apiurl);
                                    RestRequest request = new RestRequest("add_permit", Method.POST);
                                    request.AddParameter("permit_data", "{\"reference_no\":\"" + referenceno + "\",\"site_code\":\"" + sitecode + "\",\"start_date\":\"" + startdate + "\",\"end_date\":\"" + enddate + "\",\"vehicle_details\":{\"vrm\":\"" + vrm + "\"}}");
                                    request.AddHeader("HTTPS_AUTH", apikey);
                                    var response = client.Execute(request);
                                    if (response.IsSuccessful)
                                    {
                                        string xml = response.Content;
                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml(xml);
                                        string JsonValue = JsonConvert.SerializeXmlNode(doc);
                                        Common.InfoLogs("Zatparkapi was excuted " + JsonValue);
                                        string myJsonResponse = JsonValue.Replace("?xml", "xml1");

                                        Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
                                        if (myDeserializedClass.xml.status_code == "0" && myDeserializedClass.xml.error == "0")
                                        {
                                            ArrayList list = new ArrayList();
                                            list.Add(Convert.ToInt32(referenceno));
                                            list.Add(true);
                                            list.Add(requeststr);
                                            list.Add(JsonValue);
                                            list.Add(myDeserializedClass.xml.message);
                                            DataSet dataset = Util.UpdateVehicleDetails(list);
                                            Common.InfoLogs("UpdateVehicleDetails was excuted ");
                                            if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                                            {
                                                Common.InfoLogs("ZatparkUpdate method was executed success Request= " + requeststr + " Response= " + response);
                                            }
                                            else
                                            {
                                                Common.InfoLogs("ZatparkUpdate method was executed failure while requesting Request= " + requeststr + " Response= " + response);
                                            }
                                        }
                                        else if (myDeserializedClass.xml.status_code == "25" && myDeserializedClass.xml.error == "0")
                                        {
                                            ArrayList list = new ArrayList();
                                            list.Add(Convert.ToInt32(referenceno));
                                            list.Add(false);
                                            list.Add(requeststr);
                                            list.Add(JsonValue);
                                            list.Add(myDeserializedClass.xml.message);
                                            DataSet dataset = Util.UpdateVehicleDetails(list);
                                            if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                                            {
                                                Common.InfoLogs("ZatparkUpdate method was executed failure Request= " + requeststr + " Response= " + response);
                                            }
                                            else
                                            {
                                                Common.InfoLogs("ZatparkUpdate method was executed failure while inserting Request= " + requeststr + " Response= " + response);
                                            }
                                        }
                                        else if (myDeserializedClass.xml.status_code == "1" && myDeserializedClass.xml.error == "1")
                                        {
                                            ArrayList list = new ArrayList();
                                            list.Add(Convert.ToInt32(referenceno));
                                            list.Add(false);
                                            list.Add(requeststr);
                                            list.Add(JsonValue);
                                            list.Add(myDeserializedClass.xml.message);
                                            DataSet dataset = Util.UpdateVehicleDetails(list);
                                            if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                                            {
                                                Common.InfoLogs("ZatparkUpdate method was executed failure Request= " + requeststr + " Response= " + response);
                                            }
                                            else
                                            {
                                                Common.InfoLogs("ZatparkUpdate method was executed failure while inserting Request= " + requeststr + " Response= " + response);
                                            }
                                        }

                                    }
                                    //}
                                }
                            }

                            ds = new DataSet();
                            array = new ArrayList();
                            array.Add(SiteId);
                            //if (zatparkhours)
                            //{
                            array.Add(1);
                            ds = Util.GetVisitorVehicleDetails(array);
                            //}
                            //else
                            //{
                            //    array.Add(2);
                            //    ds = Util.GetVisitorVehicleDetails(array);
                            //}

                            if (ds != null && ds.Tables[0].Rows.Count > 0 && ds.Tables.Count > 0)
                            {
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    referenceno = ds.Tables[0].Rows[i]["Id"].ToString();
                                    //referencenew = ds.Tables[0].Rows[i]["ReferenceNo"].ToString();
                                    sitecode = ds.Tables[0].Rows[i]["Sitecode"].ToString();
                                    //sitecode = "eur0011-500";
                                    startdate = Convert.ToDateTime(ds.Tables[0].Rows[i]["FromDate"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ds.Tables[0].Rows[i]["FromDate"]).ToString("HH:mm");
                                    enddate = Convert.ToDateTime(ds.Tables[0].Rows[i]["ToDate"]).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(ds.Tables[0].Rows[i]["ToDate"]).ToString("HH:mm");
                                    vrm = ds.Tables[0].Rows[i]["VRM"].ToString();
                                    Common.InfoLogs("ZatparkUpdate method for Visitor was started vrm= " + vrm + " referencenew= " + referenceno + " startdate= " + startdate + " enddate= " + enddate);

                                    string requeststr = "{\"reference_no\":\"" + referenceno + "\",\"site_code\":\"" + sitecode + "\",\"start_date\":\"" + startdate + "\",\"end_date\":\"" + enddate + "\",\"vehicle_details\":{\"vrm\":\"" + vrm + "\"}}";
                                    //if (Convert.ToDateTime(ds.Tables[0].Rows[i]["FromDate"]).ToString("HH:mm") == Convert.ToDateTime(ds.Tables[0].Rows[i]["ToDate"]).ToString("HH:mm"))
                                    //{
                                    //    Common.InfoLogs("FromTime and ToTime are equal");
                                    //}
                                    //else
                                    //{
                                    RestClient client = new RestClient(apiurl);
                                    RestRequest request = new RestRequest("add_permit", Method.POST);
                                    request.AddParameter("permit_data", "{\"reference_no\":\"" + referenceno + "\",\"site_code\":\"" + sitecode + "\",\"start_date\":\"" + startdate + "\",\"end_date\":\"" + enddate + "\",\"vehicle_details\":{\"vrm\":\"" + vrm + "\"}}");
                                    request.AddHeader("HTTPS_AUTH", apikey);
                                    var response = client.Execute(request);
                                    if (response.IsSuccessful)
                                    {
                                        string xml = response.Content;
                                        XmlDocument doc = new XmlDocument();
                                        doc.LoadXml(xml);
                                        string JsonValue = JsonConvert.SerializeXmlNode(doc);
                                        Common.InfoLogs("Zatparkapi for Visitor was excuted " + JsonValue);
                                        string myJsonResponse = JsonValue.Replace("?xml", "xml1");

                                        Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
                                        if (myDeserializedClass.xml.status_code == "0" && myDeserializedClass.xml.error == "0")
                                        {
                                            ArrayList list = new ArrayList();
                                            list.Add(Convert.ToInt32(referenceno));
                                            list.Add(true);
                                            list.Add(requeststr);
                                            list.Add(JsonValue);
                                            list.Add(myDeserializedClass.xml.message);
                                            DataSet dataset = Util.UpdateVisitorVehicleDetails(list);
                                            if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                                            {
                                                Common.InfoLogs("ZatparkUpdate for Visitor method was executed success Request= " + requeststr + " Response= " + response);
                                            }
                                            else
                                            {
                                                Common.InfoLogs("ZatparkUpdate for Visitor method was executed failure while requesting Request= " + requeststr + " Response= " + response);
                                            }
                                        }
                                        else if (myDeserializedClass.xml.status_code == "25" && myDeserializedClass.xml.error == "0")
                                        {
                                            ArrayList list = new ArrayList();
                                            list.Add(Convert.ToInt32(referenceno));
                                            list.Add(false);
                                            list.Add(requeststr);
                                            list.Add(JsonValue);
                                            list.Add(myDeserializedClass.xml.message);
                                            DataSet dataset = Util.UpdateVehicleDetails(list);
                                            if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                                            {
                                                Common.InfoLogs("ZatparkUpdate for Visitor method was executed failure Request= " + requeststr + " Response= " + response);
                                            }
                                            else
                                            {
                                                Common.InfoLogs("ZatparkUpdate for Visitor method was executed failure while inserting Request= " + requeststr + " Response= " + response);
                                            }
                                        }
                                        else if (myDeserializedClass.xml.status_code == "1" && myDeserializedClass.xml.error == "1")
                                        {
                                            ArrayList list = new ArrayList();
                                            list.Add(Convert.ToInt32(referenceno));
                                            list.Add(false);
                                            list.Add(requeststr);
                                            list.Add(JsonValue);
                                            list.Add(myDeserializedClass.xml.message);
                                            DataSet dataset = Util.UpdateVehicleDetails(list);
                                            if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                                            {
                                                Common.InfoLogs("ZatparkUpdate for Visitor method was executed failure Request= " + requeststr + " Response= " + response);
                                            }
                                            else
                                            {
                                                Common.InfoLogs("ZatparkUpdate method was executed failure while inserting Request= " + requeststr + " Response= " + response);
                                            }
                                        }

                                    }
                                    // }
                                }
                            }
                            //timer.Enabled = true;
                            Common.InfoLogs("ZatparkUpdate method was Done");
                        }
                    }



                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                    Common.InfoLogs("ZatparkUpdate Method Error " + str.ToString());
                    timer.Enabled = true;
                }
                
            

        }
    }

public partial class SmartPermitService21212 : ServiceBase
    {
        System.Timers.Timer timer;
        public SmartPermitService21212()
        {
            //ZatparkUpdate();
            //Common.InfoLogs("SmartPermitService Method Started " + DateTime.Now);
           // InitializeComponent();
            //Common.InfoLogs("SmartPermitService Method Ended " + DateTime.Now);

        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;
            timer.Start();
        }
        protected override void OnStop()
        {
            Common.InfoLogs("SmartPermitService service stopped " + DateTime.Now);
        }
       

        //protected override void OnStart(string[] args)
        //{
        //    Common.InfoLogs("OnStart Method Started " + DateTime.Now);
        //    DateTime dt = DateTime.Now;
        //    Common.InfoLogs("OnStart Method Started time= " + dt);

        //    Timer = new System.Timers.Timer();
        //    Timer.Elapsed += new System.Timers.ElapsedEventHandler(OnElapsedTime);
        //    Timer.Enabled = true;
        //    Timer.Interval = 1000;
        //    Timer.Start();




        //    Common.InfoLogs("OnStart Method Ended " + DateTime.Now);

        //    // callback();
        //}

        //protected override void OnStop()
        //{
        //    Common.InfoLogs("OnStop Method Ended " + DateTime.Now);
        //}
        //public void ZatparkUpdate(object sender, System.Timers.ElapsedEventArgs args)
        private void OnElapsedTime(object sender, ElapsedEventArgs args)
        {
          
        }
    }
}
