using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
namespace door_ver2_1
{
    
    public class info
    {
        JObject json = null;
        public bool userinfo(string id,string pwd)
        {
            try
            {
                json = new JObject(
                new JProperty("Id", id),
                new JProperty("pwd", pwd)
                );
                File.WriteAllText(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\doorhelp.json",json.ToString());

            }
            catch
            {
                return false;
            }

            return true;
            

        }
        //public string[] getInfo()
        //{

        //}
    }
    


}
