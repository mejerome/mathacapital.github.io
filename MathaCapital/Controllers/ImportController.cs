using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathaCapital.Data;
using MathaCapital.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace MathaCapital.Controllers
{
    public class ImportController : Controller
    {
        private readonly AuctionContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ImportController(IHostingEnvironment hostingEnvironment, AuctionContext db)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UploadExcel(IFormFile file)
        {
            string message = string.Empty;
            int count = 0;
            await UploadFile(file);
            ImportData(out count);
            DeleteFiles();
            return Redirect("/Bids/Index");
        }


        private bool ImportData(out int count)
        {
            var result = false;
            count = 0;
            string folderName = "import";
            string rootFolder = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(rootFolder, folderName);
            string[] file = Directory.GetFiles(newPath, "*.xlsx");
            string fullPath = Path.Combine(newPath, file[0]);
            FileInfo fileInfo = new FileInfo(fullPath);
            string batchRef = DateTime.Now.ToString("yyyyMMddHHmmss");

            try
            {
                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    int totalRows = workSheet.Dimension.Rows;

                    List<AuctionBid> auctionBids = new List<AuctionBid>();

                    for (int i = 2; i <= totalRows; i++)
                    {
                        string fDate = workSheet.Cells[i, 1].Value.ToString();
                        double date = double.Parse(fDate);
                        auctionBids.Add(new AuctionBid
                        {
                            BankName = workSheet.Cells[i, 5].Value.ToString(),
                            FwdDate = DateTime.FromOADate(date),
                            FwdRate = Math.Round(double.Parse(workSheet.Cells[i, 3].Value.ToString()), 6),
                            AmountBid = decimal.Parse(workSheet.Cells[i, 4].Value.ToString()),
                            CouponAmount = decimal.Parse(workSheet.Cells[i, 2].Value.ToString()),
                            Pips = decimal.Parse(workSheet.Cells[i, 6].Value.ToString()),
                            BatchRef = batchRef

                        });
                    }

                    _context.AuctionBids.AddRange(auctionBids);
                    _context.SaveChanges();
                    count++;

                }

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length < 0)
            {
                return Content("file not selected.");
            }
            string fileName = file.FileName;
            string folderName = "import";
            string rootFolder = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(rootFolder, folderName);
            string path = Path.Combine(newPath, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
                return Ok();
        }

        public ActionResult DeleteFiles()
        {
            string folderName = "import";
            string rootFolder = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(rootFolder, folderName);
            DirectoryInfo d = new DirectoryInfo(newPath);
            FileInfo[] files = d.GetFiles("*.xlsx");
            foreach (FileInfo file in files)
            {
                file.Delete();
            }
            return Ok();
        }

    }
}