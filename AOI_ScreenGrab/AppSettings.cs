using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AOI_ScreenGrab
{
    internal class AppSettings
    {
        static string _dirpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static AppSettings Current;

        public string Filepath = "D:/screens/{LINE}/{PROD}/{WO}/{NR}_{OPID}_{WO}_{PROD}_{DATE}";
        public int ScreenIndex = -1;
        public string LineId = "SMT01";
        public bool ShowMessage = false;
        public bool ShowPhoto = true;
        public string LogsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void Init()
        {
            Current = Load(_dirpath + "/config.json");
            Save();
        }

        public static AppSettings Load(string filepath)
        {
            if(File.Exists(filepath))
                try
                {
                    return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(filepath));
                }
                catch(Exception ex)
                {
                    File.WriteAllText(Assembly.GetExecutingAssembly().Location + "errors.log", ex.Message);
                }
            return new AppSettings();
        }

        public static void Save()
        {
            try
            {
                File.WriteAllText(_dirpath+"/config.json", JsonConvert.SerializeObject(Current));
            }
            catch(Exception ex)
            {
                
                File.WriteAllText(_dirpath + "/errors.log", ex.Message);                
            }
        }
    }
}
