using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoServer
{
    public class AppSettings
    {
        public string D2DBSPath { get; set; }
        public string D2CSPath { get; set; }
        public string D2GSPath { get; set; }
        public string PVPGNPath { get; set; }
        public string StorePath { get; set; }

        public static AppSettings LoadSettings(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new AppSettings();
            }

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<AppSettings>(json);
        }
    }

}
