using System.IO;
using System.Xml.Serialization;

namespace ToolSL
{
    public class Options
    {
        public string HistoryFolder { get; set; }
        public bool AutostartImport { get; set; }

        public void Save(string filename)
        {
            try
            {
                var s = new XmlSerializer(typeof(Options));
                var w = new StreamWriter(filename);
                s.Serialize(w, this);
                w.Flush();
                w.Close();
            }
            catch
            {
                //do nothing
            }
        }

        public static Options Load(string filename)
        {
            try
            {
                var s = new XmlSerializer(typeof(Options));
                var r = new StreamReader(filename);
                var result = (Options)s.Deserialize(r);
                r.Close();
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
