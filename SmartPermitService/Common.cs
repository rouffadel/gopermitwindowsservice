using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPermitService
{
    public class Common
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        public static void InfoLogs(string Message)
        {
            try
            {
                string appPath = AppDomain.CurrentDomain.BaseDirectory + "Logs";
                //if (System.Configuration.ConfigurationManager.AppSettings["LogErrorType"].ToString() == "File")
                //{
                if (!Directory.Exists(appPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(appPath);
                }
                string Filename = "APPServiceLog" + DateTime.Now.ToString("dd-MM-yyyy"); //dateAndTime.ToString("dd/MM/yyyy")
                string fullpath = appPath + "\\" + Filename + ".txt";

                if (!File.Exists(fullpath))
                {
                    System.IO.FileStream f = System.IO.File.Create(fullpath);
                    f.Close();
                    TextWriter tw = new StreamWriter(fullpath);
                    tw.WriteLine(DateTime.Now + " " + Message);
                    tw.Close();
                }
                else if (File.Exists(fullpath))
                {
                    using (StreamWriter w = File.AppendText(fullpath))
                    {
                        w.WriteLine(DateTime.Now + " " + Message);
                        w.Close();
                    }
                }
                // }
            }
            catch(Exception ex)
            {

            }

        }
    }
}
