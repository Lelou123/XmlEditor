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
using XmlEditor.Controllers;
using XmlEditor.Models;


namespace XmlEditor.Models
{
    public class XmlModel
    {        
        public string InputXml { get; set; }        
        public string XmlFiles { get; set; }

        

        

        public List<string> ListaArquivos(IWebHostEnvironment _environment)
        {
            string filePath = Path.Combine(_environment.WebRootPath, "Temp");

            string[] allfiles = Directory.GetFiles((filePath), "*.xml", SearchOption.TopDirectoryOnly);

            List<string> list = new List<string>();

            foreach (string file in allfiles)
            {
                list.Add(Path.GetFileName(file));
            }
            return list;
        }
       
    }
}
