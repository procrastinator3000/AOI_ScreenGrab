using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace AOI_ScreenGrab
{
    internal class AppData
    {
        public string WO;
        public string OPID;
        public string PROD;
        public int NR;

        static string _filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\AppData.json";

        public void Save()
        {
            try
            {
                var jstr = JsonConvert.SerializeObject(this);
                File.WriteAllText(_filepath, jstr);
            } catch(Exception ex)
            {

            }
        }

        public static AppData Load()
        {
            try
            {
                return JsonConvert.DeserializeObject<AppData>(File.ReadAllText(_filepath));
            }
            catch (Exception ex)
            {

            }
            return new AppData();
        }
    }
}
