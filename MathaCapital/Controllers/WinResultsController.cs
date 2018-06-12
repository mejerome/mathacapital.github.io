using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MathaCapital.Data;
using MathaCapital.Models;
using System.Data.SqlClient;
using System.Data;

namespace MathaCapital.Controllers
{
    public class WinResultsController : Controller
    {
        private readonly AuctionContext _context;

        public WinResultsController(AuctionContext context)
        {
            _context = context;
        }

        // Run Auction for a Batch
        public async Task<IActionResult> RunAuction(string bidBatch)
        {
            //Get a list of dates in this Batch
            List<DateTime> res = (from a in _context.AuctionBids
                                  where a.BatchRef.ToString() == bidBatch
                                  orderby a.FwdDate
                                  select a.FwdDate).Distinct().ToList();

            // Delete wins before running auction again
            _context.WinResults.Where(w => w.BatchRef == bidBatch).ToList().ForEach(p => _context.WinResults.Remove(p));


            foreach (var date in res)
            {
                // Direct SQL to pick winners for a date
                string connectionString = "Server=JEROME-SBOOK\\SQLEXPRESS;Database=MathaRx;Trusted_Connection=True;MultipleActiveResultSets=true; Integrated Security=true;";
                string sqlQry = "select ID, FwdDate, CouponAmount, BankName, AmountBid, FwdRate, BatchRef, case when remainder < 0 then remainder_1 else " +
                    "AmountBid end awarded_amount from (select *, LAG(remainder) over(order by FwdRate desc) remainder_1 " +
                    "from (select ID, FwdDate, CouponAmount, BankName, AmountBid, FwdRate, BatchRef, SUM(AmountBid) over (order by FwdRate desc) rtotal, " +
                    "CouponAmount - SUM(AmountBid) over (order by FwdRate desc) as remainder from AuctionBid where FwdDate=@fwdDate and BatchRef=@batchRef) tb) a where case when remainder < 0 then remainder_1 else AmountBid end >= 0";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand(null, conn))
                    {
                        conn.Open();
                        comm.CommandText = sqlQry;
                        SqlParameter param1 = comm.Parameters.Add("@fwdDate", SqlDbType.DateTime2, 8);
                        SqlParameter param2 = comm.Parameters.Add("@batchRef", SqlDbType.NVarChar, 100);

                        param1.Value = date;
                        param2.Value = bidBatch;

                        comm.Prepare();
                        SqlDataReader reader = comm.ExecuteReader();
                        DataTable dt = new DataTable();

                        dt.Load(reader);
                        conn.Close();

                        // Convert Date wins to object and write to entity
                        foreach (DataRow row in dt.Rows)
                        {
                            WinResults convertedObject = ConvertRowToWinResult(row);

                            if (convertedObject.WinAmount != 0)
                            {
                                _context.WinResults.Add(convertedObject);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                }
            }

            return RedirectToAction("Index", "WinResults");

        }
        // Convert DataTable rows to Object
        public WinResults ConvertRowToWinResult(DataRow dr)
        {
            WinResults winResult = new WinResults();

            winResult.AuctionBidID = Convert.ToInt16(dr["ID"]);
            winResult.FwdDate = Convert.ToDateTime(dr["FwdDate"]);
            winResult.CouponAmount = Convert.ToDecimal(dr["CouponAmount"]);
            winResult.BankName = Convert.ToString(dr["BankName"]);
            winResult.AmountBid = Convert.ToDecimal(dr["AmountBid"]);
            winResult.FwdRate = Convert.ToDouble(dr["FwdRate"]);
            winResult.WinAmount = Convert.ToDecimal(dr["awarded_amount"]);
            winResult.BatchRef = Convert.ToString(dr["BatchRef"]);

            return winResult;
        }



        // GET: WinResults
        public async Task<IActionResult> Index(string bidBatch, string sortOrder)
        {


            IQueryable<string> batchQuery = from b in _context.AuctionBids
                                            orderby b.BatchRef
                                            select b.BatchRef;

            var bids = from m in _context.WinResults
                       select m;

            if (!String.IsNullOrEmpty(bidBatch))
            {
                bids = bids.Where(x => x.BatchRef == bidBatch);

            }
            switch (sortOrder)
            {
                case "sortByBank":
                    bids = bids.OrderBy(s => s.BankName).ThenBy(s=>s.FwdDate);
                    break;


                case "sortByDate":
                    bids = bids.OrderBy(s => s.FwdDate).ThenByDescending(s => s.FwdRate);
                    break;

            }

            var bidVM = new BidBatchViewModel();
            bidVM.batches = new SelectList(await batchQuery.Distinct().ToListAsync());
            bidVM.wins = await bids.ToListAsync();
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
