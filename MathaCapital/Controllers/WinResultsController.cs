using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MathaCapital.Data;
using MathaCapital.Models;
using System.Text;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace MathaCapital.Controllers
{
    public class WinResultsController : Controller
    {
        private readonly AuctionContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        public WinResultsController(AuctionContext context, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        // Export Wins to Excel
        public IActionResult ExportWinData(string bidBatch)
        {
            var sb = new StringBuilder();
            var data = from s in _context.WinResults
                       where s.BatchRef == bidBatch
                       select new
                       {
                           s.AuctionBidID,
                           s.FwdDate,
                           s.BankName,
                           s.FwdRate,
                           s.AmountBid,
                           s.WinAmount,
                           s.CouponAmount,
                           s.BatchRef
                       };
            var list = data.ToList();
            byte[] fileContents;
            var memory = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet wSheet = package.Workbook.Worksheets.Add("Win");
                int totalRows = list.Count();

                wSheet.Cells[1, 1].Value = "Auction ID";
                wSheet.Cells[1, 2].Value = "Fwd Date";
                wSheet.Cells[1, 3].Value = "Bank Name";
                wSheet.Cells[1, 4].Value = "Fwd Rate";
                wSheet.Cells[1, 5].Value = "Amount Bid";
                wSheet.Cells[1, 6].Value = "Amount Won";
                wSheet.Cells[1, 7].Value = "Coupon Amount";
                wSheet.Cells[1, 8].Value = "Batch Reference";
                wSheet.Cells[1, 1].Style.Font.Bold = true;
                wSheet.Cells[1, 2].Style.Font.Bold = true;
                wSheet.Cells[1, 3].Style.Font.Bold = true;
                wSheet.Cells[1, 4].Style.Font.Bold = true;
                wSheet.Cells[1, 5].Style.Font.Bold = true;
                wSheet.Cells[1, 6].Style.Font.Bold = true;
                wSheet.Cells[1, 7].Style.Font.Bold = true;
                wSheet.Cells[1, 8].Style.Font.Bold = true;
                int i = 0;
                for (int row = 2; row < totalRows + 1; row++)
                {
                    wSheet.Cells[row, 1].Value = list[i].AuctionBidID;
                    wSheet.Cells[row, 2].Value = list[i].FwdDate;
                    wSheet.Cells[row, 3].Value = list[i].BankName;
                    wSheet.Cells[row, 4].Value = list[i].FwdRate;
                    wSheet.Cells[row, 5].Value = list[i].AmountBid;
                    wSheet.Cells[row, 6].Value = list[i].WinAmount;
                    wSheet.Cells[row, 7].Value = list[i].CouponAmount;
                    wSheet.Cells[row, 8].Value = list[i].BatchRef;
                    i++;
                }
                fileContents = package.GetAsByteArray();
            }
            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "ExportWins.xlsx");
        }


        // GET: WinResults
        public async Task<IActionResult> Index(string bidBatch, string sortOrder, string bankName, string fwdDate)
        {
            IQueryable<string> batchQuery = from b in _context.AuctionBids
                                            orderby b.BatchRef
                                            select b.BatchRef;
            IQueryable<string> bankQuery = from d in _context.WinResults
                                           orderby d.BankName
                                           select d.BankName;

            var wins = from m in _context.WinResults
                       select m;

            if (!string.IsNullOrEmpty(bidBatch))
            {
                wins = wins.Where(x => x.BatchRef == bidBatch);
            }

            if (!string.IsNullOrEmpty(bankName))
            {
                wins = wins.Where(y => y.BankName == bankName);
            }

            if (!string.IsNullOrEmpty(fwdDate))
            {
                wins = wins.Where(z => z.FwdDate == Convert.ToDateTime(fwdDate));
            }

            switch (sortOrder)
            {
                case "sortByBank":
                    wins = wins.OrderBy(s => s.BankName).ThenBy(s => s.FwdDate);
                    break;

                case "sortByDate":
                    wins = wins.OrderBy(s => s.FwdDate).ThenByDescending(s => s.FwdRate);
                    break;
            }

            var bidVM = new BidBatchViewModel();
            bidVM.banks = new SelectList(await bankQuery.Distinct().ToListAsync());
            bidVM.batches = new SelectList(await batchQuery.Distinct().ToListAsync());
            bidVM.wins = await wins.ToListAsync();
            return View(bidVM);
        }



        // GET: WinResults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var winResults = await _context.WinResults
                .Include(w => w.AuctionBid)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (winResults == null)
            {
                return NotFound();
            }

            return View(winResults);
        }

        // GET: WinResults/Create
        public IActionResult Create()
        {
            ViewData["AuctionBidID"] = new SelectList(_context.AuctionBids, "ID", "ID");
            return View();
        }

        // POST: WinResults/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FwdDate,BankName,AuctionBidID,FwdRate,AmountBid,CouponAmount,WinAmount")] WinResults winResults)
        {
            if (ModelState.IsValid)
            {
                _context.Add(winResults);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuctionBidID"] = new SelectList(_context.AuctionBids, "ID", "ID", winResults.AuctionBidID);
            return View(winResults);
        }

        // GET: WinResults/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var winResults = await _context.WinResults.SingleOrDefaultAsync(m => m.ID == id);
            if (winResults == null)
            {
                return NotFound();
            }
            ViewData["AuctionBidID"] = new SelectList(_context.AuctionBids, "ID", "ID", winResults.AuctionBidID);
            return View(winResults);
        }

        // POST: WinResults/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FwdDate,BankName,AuctionBidID,FwdRate,AmountBid,CouponAmount,WinAmount")] WinResults winResults)
        {
            if (id != winResults.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(winResults);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WinResultsExists(winResults.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuctionBidID"] = new SelectList(_context.AuctionBids, "ID", "ID", winResults.AuctionBidID);
            return View(winResults);
        }

        // GET: WinResults/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var winResults = await _context.WinResults
                .Include(w => w.AuctionBid)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (winResults == null)
            {
                return NotFound();
            }

            return View(winResults);
        }

        // POST: WinResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var winResults = await _context.WinResults.SingleOrDefaultAsync(m => m.ID == id);
            _context.WinResults.Remove(winResults);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WinResultsExists(int id)
        {
            return _context.WinResults.Any(e => e.ID == id);
        }
    }
}
