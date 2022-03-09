using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using XmlEditor.Models;


namespace XmlEditor.Models
{
    public class XmlModel
    {
        public string XmlNodes { get; set; }
        public string InputXml { get; set; }
        public bool MinifyBool { get; set; }
        public string XmlFiles { get; set; }

        public List<string> XmList { get; set; } = new List<string>();


        public void AddXmlList(XmlModel x)
        {
            XmList.Add(x.XmlFiles);
        }

        public void MinifyXml(string pathFile)
        {

        }
    }
}
