using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MathaCapital.Data;
using MathaCapital.Models;

namespace MathaCapital.Controllers
{
    public class WinResultsController : Controller
    {
        private readonly AuctionContext _context;

        public WinResultsController(AuctionContext context)
        {
            _context = context;
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

            if (!String.IsNullOrEmpty(bidBatch))
            {
                wins = wins.Where(x => x.BatchRef == bidBatch);
            }

            if (!String.IsNullOrEmpty(bankName))
            {
                wins = wins.Where(y => y.BankName == bankName);
            }

            if (!String.IsNullOrEmpty(fwdDate))
            {
                wins = wins.Where(z => z.FwdDate == Convert.ToDateTime(fwdDate));
            }

            switch (sortOrder)
            {
                case "sortByBank":
                    wins = wins.OrderBy(s => s.BankName).ThenBy(s=>s.FwdDate);
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
