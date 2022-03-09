using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using XmlEditor.Models;

namespace XmlEditor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IWebHostEnvironment _environment;


        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }




        public IActionResult Index(bool MinifyBool)
        {
            XmlModel x = new XmlModel();


            //Get xml Content
            string filePath = Path.Combine(_environment.WebRootPath, "Temp");

            string[] allfiles = Directory.GetFiles((filePath), "*.xml", SearchOption.TopDirectoryOnly);

            List<string> list = new List<string>();

            foreach (string file in allfiles)
            {
                list.Add(Path.GetFileName(file));
            }

            if (list.Count > 0)
            {
                string pathFile = Path.Combine(filePath, list[0]);
                string fileContent;


                if (MinifyBool == true)
                {
                    XDocument doc = XDocument.Load(pathFile);

                    fileContent = doc.ToString(SaveOptions.DisableFormatting);
                    ViewBag.FileContent = fileContent;

                }
                else
                {
                    XDocument doc = XDocument.Load(pathFile);

                    fileContent = doc.ToString();
                    ViewBag.FileContent = fileContent;

                }


                //Get child nodes


                if (System.IO.File.Exists(pathFile))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(pathFile);
                    XmlElement elm = doc.DocumentElement;
                    List<string> s = new List<string>();

                    List<string> v = new List<string>();
                    XmlNode root = elm.FirstChild;
                    bool flag = true;

                    while (flag)
                    {
                        if (root.HasChildNodes)
                        {
                            root = root.FirstChild;
                        }

                        else
                        {
                            s.Add(root.ParentNode.LocalName);
                            v.Add(root.ParentNode.InnerText);
                            while (true)
                            {
                                if (root.ParentNode != null)
                                {
                                    root = root.ParentNode;
                                    if (root.NextSibling != null)
                                    {

                                        root = root.NextSibling;

                                        break;
                                    }

                                }

                                else
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }

                    }

                    ViewBag.NodesContent = v;

                    ViewBag.Nodes = s;
                }
            }


            return View();
        }




        [HttpPost]
        public async Task<IActionResult> testejson([FromBody] XmlModel x)
        {

            string[] vet = x.XmlFiles.Split("+");
            List<string> list = new List<string>();


            foreach (string v in vet)
            {
                list.Add(v);
            }

            string filePath = Path.Combine(_environment.WebRootPath, "Temp");

            string[] allfiles = Directory.GetFiles((filePath), "*.xml", SearchOption.TopDirectoryOnly);

            List<string> list2 = new List<string>();

            foreach (string file in allfiles)
            {
                list2.Add(Path.GetFileName(file));
            }



            string pathFile = Path.Combine(filePath, list2[0]);

            XmlDocument xmlDoc = new XmlDocument();
            XmlDocument doc = new XmlDocument();
            if (System.IO.File.Exists(pathFile))
            {
                //xmlDoc.Load(pathFile);
                doc.Load(pathFile);
                XmlElement elm = xmlDoc.DocumentElement;
                List<string> s = new List<string>();

                xmlDoc.Load(pathFile);

                //xmlDoc.SelectSingleNode("CATALOGO/CD/TITULO").InnerText = "Testando";                






                List<string> v = new List<string>();
                XmlNode root = elm.FirstChild;
                bool flag = true;

                while (flag)
                {
                    if (root.HasChildNodes)
                    {
                        root = root.FirstChild;
                    }

                    else
                    {                                              
                        s.Add(root.ParentNode.LocalName);
                        
                        for (int i = 0; i < s.Count; i++)
                        {
                            doc.SelectSingleNode(s[i]).InnerText = list[i];
                        }

                        while (true)
                        {
                            if (root.ParentNode != null)
                            {
                                root = root.ParentNode;
                                if (root.NextSibling != null)
                                {

                                    root = root.NextSibling;

                                    break;
                                }

                            }

                            else
                            {
                                flag = false;
                                break;
                            }
                        }
                    }

                }



                foreach (XmlNode node in elm.ChildNodes)
                {
                    if (node.HasChildNodes)
                    {
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            s.Add(childNode.LocalName);
                        }
                    }
                }


                for (int i = 0; i < s.Count; i++)
                {
                    foreach (XmlNode node in elm.ChildNodes)
                    {
                        if (node.HasChildNodes)
                        {
                            foreach (XmlNode childNode in node.ChildNodes)
                            {
                                if (list[i] != "")
                                {
                                    xmlDoc.SelectSingleNode("CATALOGO/CD/" + s[i]).InnerText = list[i];
                                }
                            }
                        }
                    }
                }
                doc.Save(pathFile);

            }


            return Json(new { result = "OK OK " });
        }






        public ActionResult DownloadDocument(bool MinifyBool)
        {
            string filePath = Path.Combine(_environment.WebRootPath, "Temp");

            string[] allfiles = Directory.GetFiles((filePath), "*.xml", SearchOption.TopDirectoryOnly);

            List<string> list = new List<string>();

            foreach (string file in allfiles)
            {
                list.Add(Path.GetFileName(file));
            }

            string pathFile = Path.Combine(filePath, list[0]);

            string fileName = list[0];


            string arquivo = Path.Combine(filePath, fileName);

            if (MinifyBool == false)
            {
                XDocument doc = XDocument.Load(pathFile);

                string fileContent = doc.ToString(SaveOptions.None);

                using (StreamWriter sw = System.IO.File.CreateText(arquivo))
                {
                    sw.WriteLine(fileContent);
                }

                byte[] fileBytes1 = System.IO.File.ReadAllBytes(pathFile);
                return File(fileBytes1, "application/force-download", fileName);
            }
            else
            {
                XDocument doc = XDocument.Load(pathFile);

                string fileContent = doc.ToString(SaveOptions.DisableFormatting);

                using (StreamWriter sw = System.IO.File.CreateText(arquivo))
                {
                    sw.WriteLine(fileContent);
                }
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(arquivo);

            return File(fileBytes, "application/force-download", fileName);

        }




        public async Task<IActionResult> XmlFileUpload(ICollection<IFormFile> files, XmlModel x)
        {
            string filePath = Path.Combine(_environment.WebRootPath, "Temp");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(filePath, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            return RedirectToAction("Index");
        }




        [HttpPost]
        //Ação de upar as informações na text area
        public async Task<IActionResult> XmlDataUpload(ICollection<IFormFile> files, XmlModel x)
        {
            string filePath = Path.Combine(_environment.WebRootPath, "Temp");

            if (files != null && x.InputXml != "")
            {
                string stringPath = Path.Combine(filePath, "texto.xml");
                using (StreamWriter sw = System.IO.File.CreateText(stringPath))
                {
                    sw.WriteLine(x.InputXml);
                }
            }

            return RedirectToAction(nameof(Index));
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
