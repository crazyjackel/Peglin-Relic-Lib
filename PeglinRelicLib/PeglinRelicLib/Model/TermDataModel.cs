using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeglinRelicLib.Model
{
    public class TermDataModel
    {
        public string Key { get; set; }
        public string Type { get; set; } = "Text";
        public string Description { get; set; }
        public string English { get; set; }
        public string French { get; set; }
        public string Spanish { get; set; }
        public string Deutsch { get; set; }
        public string Nederlands { get; set; }
        public string Italian { get; set; }
        public string Portuguese { get; set; }
        public string Russian { get; set; }
        public string Chinese { get; set; }
        public string Taiwanese { get; set; }
        public string Japanese { get; set; }
        public string Korean { get; set; }
        //Ugh the Swedes :)
        public string Swedish { get; set; }
        public string Polish { get; set; }
        public string Turkish { get; set; }

        public TermDataModel(string Key)
        {
            this.Key = Key;
        }

        public string GetCSVLine()
        {
            return $"{Key},{Type},{Description},{English},{French},{Spanish},{Deutsch},{Nederlands},{Italian},{Portuguese},{Russian},{Chinese},{Taiwanese},{Japanese},{Korean},{Swedish},{Polish},{Turkish}\n";
        }
    }
}
