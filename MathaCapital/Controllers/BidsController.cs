using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MathaCapital.Data;
using MathaCapital.Models;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using MoreLinq;
using System.Data.SqlClient;
using System.Globalization;

namespace MathaCapital.Controllers
{
    public class BidsController : Controller
    {
        private readonly AuctionContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        public BidsController(IHostingEnvironment hostingEnvironment, AuctionContext db)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = db;
        }

        
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        


            // GET: Bids
            public async Task<IActionResult> Index(string searchString, string fwdDate, string bidBatch)
        {

            IQueryable<string> batchQuery = from b in _context.AuctionBids
                                            orderby b.BatchRef
                                            select b.BatchRef;

            var bids = from m in _context.AuctionBids
                       select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                bids = bids.Where(s => s.BankName.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(bidBatch))
            {
                bids = bids.Where(x => x.BatchRef == bidBatch);
            }

            if (!String.IsNullOrEmpty(fwdDate))
            {
                bids = bids.Where(x => x.FwdDate == Convert.ToDateTime(fwdDate));
            }
            var bidVM = new BidBatchViewModel();
            bidVM.batches = new SelectList(await batchQuery.Distinct().ToListAsync());
            bidVM.bids = await bids.OrderBy(b => b.FwdDate).ThenByDescending(b => b.FwdRate).ToListAsync();
            return View(bidVM);
        }

        // GET: Bids/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auctionBid = await _context.AuctionBids
                .SingleOrDefaultAsync(m => m.ID == id);
            if (auctionBid == null)
            {
                return NotFound();
            }

            return View(auctionBid);
        }

        // GET: Bids/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bids/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FwdDate,BankName,FwdRate,AmountBid,CouponAmount,Pips,BatchRef")] AuctionBid auctionBid)
        {
            if (ModelState.IsValid)
            {
                _context.Add(auctionBid);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(auctionBid);
        }

        // GET: Bids/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auctionBid = await _context.AuctionBids.SingleOrDefaultAsync(m => m.ID == id);
            if (auctionBid == null)
            {
                return NotFound();
            }
            return View(auctionBid);
        }

        // POST: Bids/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FwdDate,BankName,FwdRate,AmountBid,CouponAmount,Pips,BatchRef")] AuctionBid auctionBid)
        {
            if (id != auctionBid.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auctionBid);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuctionBidExists(auctionBid.ID))
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
            return View(auctionBid);
        }

        // GET: Bids/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auctionBid = await _context.AuctionBids
                .SingleOrDefaultAsync(m => m.ID == id);
            if (auctionBid == null)
            {
                return NotFound();
            }

            return View(auctionBid);
        }

        // POST: Bids/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auctionBid = await _context.AuctionBids.SingleOrDefaultAsync(m => m.ID == id);
            _context.AuctionBids.Remove(auctionBid);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuctionBidExists(int id)
        {
            return _context.AuctionBids.Any(e => e.ID == id);
        }




    }


}

