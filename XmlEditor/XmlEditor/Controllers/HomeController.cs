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
using System.Web;
using System.Xml.Linq;
using XmlEditor.Models;
using XmlEditor.Models.Services.Exceptions;

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




        public async Task<IActionResult> Index(bool MinifyBool)
        {
            XmlModel x = new XmlModel();
            string filePath = Path.Combine(_environment.WebRootPath, "Temp");
                
            var list = x.ListaArquivos(_environment);

            if (list.Count > 0)
            {
                try
                {
                    string pathFile = Path.Combine(filePath, list[0]);
                    string fileContent;
                    XElement f = XElement.Load(pathFile);
                    XDocument doc1 = XDocument.Load(pathFile);
                    if (MinifyBool == true)
                    {

                        fileContent = doc1.ToString(SaveOptions.DisableFormatting);
                        ViewBag.FileContent = fileContent; // Visualização text area                                                


                        string content = await System.IO.File.ReadAllTextAsync(pathFile);

                        ViewBag.FileContent2 = content; // Edição textarea
                    }
                    else
                    {
                        XmlDocument doc = new XmlDocument();

                        string content = await System.IO.File.ReadAllTextAsync(pathFile);

                        ViewBag.FileContent2 = content; // Edição textarea

                        fileContent = doc1.ToString();
                        ViewBag.FileContent = fileContent; // visualização textarea
                    }

                    if (System.IO.File.Exists(pathFile))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(pathFile);
                        XmlElement elm = doc.DocumentElement;

                        List<string> parentName = new List<string>();
                        List<string> nodeInnertext = new List<string>();

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
                                parentName.Add(root.ParentNode.LocalName);
                                nodeInnertext.Add(root.ParentNode.InnerText);
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
                        ViewBag.NodesContent = nodeInnertext;
                        ViewBag.Nodes = parentName;
                    }
                }
                catch (XmlException e)
                {
                    ViewBag.Errors = e.Message;
                }
              
            }
            return View();
        }






        [HttpPost]
        public async Task<IActionResult> InsertInput([FromBody] XmlModel x)
        {
            string[] vet = x.XmlFiles.Split("+");
            List<string> listaInputs = new List<string>();

            foreach (string v in vet)
            {                                
                listaInputs.Add(v);
            }
            for(int i = 0; i < listaInputs.Count; i++)
            {
                string pEncode = listaInputs[i];
                string myEncodedString = HttpUtility.HtmlAttributeEncode(pEncode);
                listaInputs[i] = pEncode;
            }
            string filePath = Path.Combine(_environment.WebRootPath, "Temp");
            
            var listaArquivos = x.ListaArquivos(_environment);
            
            string pathFile = Path.Combine(filePath, listaArquivos[0]);

            XmlDocument doc = new XmlDocument();
            if (System.IO.File.Exists(pathFile))
            {

                string content = await System.IO.File.ReadAllTextAsync(pathFile);
                doc.LoadXml(content);

                XmlElement elm = doc.DocumentElement;
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
                        root.InnerText = listaInputs[0];
                        listaInputs.Remove(listaInputs[0]);
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
                
                string a = doc.OuterXml;
                
                await System.IO.File.WriteAllTextAsync(pathFile, a);

                XDocument doc1 = XDocument.Load(pathFile);
                
                string fileContent = doc1.ToString(SaveOptions.None);
                
                using (StreamWriter sw = System.IO.File.CreateText(pathFile))
                {
                    sw.WriteLine(fileContent);
                }

            }
            return Json(new { result = "OK OK " });
        }




        public ActionResult DownloadDocument(bool MinifyBool)
        {
            string filePath = Path.Combine(_environment.WebRootPath, "Temp");

            string[] allfiles = Directory.GetFiles((filePath), "*.xml", SearchOption.TopDirectoryOnly);

            List<string> arquivoList = new List<string>();

            foreach (string file in allfiles)
            {
                arquivoList.Add(Path.GetFileName(file));
            }

            string arquivo = Path.Combine(filePath, arquivoList[0]);

            if (MinifyBool == false)
            {
                XDocument doc = XDocument.Load(arquivo);

                string fileContent = doc.ToString(SaveOptions.None);

                using (StreamWriter sw = System.IO.File.CreateText(arquivo))
                {
                    sw.WriteLine(fileContent);
                }
                byte[] fileBytes1 = System.IO.File.ReadAllBytes(arquivo);
                return File(fileBytes1, "application/force-download", arquivoList[0]);
            }
            else
            {
                XDocument doc = XDocument.Load(arquivo);

                string fileContent = doc.ToString(SaveOptions.DisableFormatting);

                using (StreamWriter sw = System.IO.File.CreateText(arquivo))
                {
                    sw.WriteLine(fileContent);
                }
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(arquivo);
            return File(fileBytes, "application/force-download", arquivoList[0]);
        }




        public async Task<IActionResult> XmlFileUpload(ICollection<IFormFile> files)
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
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public async Task<IActionResult> XmlDataUpload([FromBody] XmlModel x, ICollection<IFormFile> files)
        {
            string filePath = Path.Combine(_environment.WebRootPath, "Temp");
            var list = x.ListaArquivos(_environment);

            if (files != null && x.InputXml != "")
            {
                string stringPath;
                if (list.Count > 0)
                {
                    stringPath = Path.Combine(filePath, list[0]);
                    System.IO.File.Delete(list[0]);
                }
                else
                {
                    stringPath = Path.Combine(filePath, "texto.xml");
                }

                using (StreamWriter sw = System.IO.File.CreateText(stringPath))
                {
                    await sw.WriteLineAsync(x.InputXml);
                }
                return Json(new { result = "OK OK " });
            }

            return RedirectToAction(nameof(Index));
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            };
            return View(viewModel);
        }


    }
}
