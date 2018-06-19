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

        // Run Auction for a Batch
        public async Task<IActionResult> RunAuction(string bidBatch, string auctionType)
        {

            //Get a list of dates in this Batch
            List<DateTime> res = (from a in _context.AuctionBids
                                  where a.BatchRef.ToString() == bidBatch
                                  orderby a.FwdDate
                                  select a.FwdDate).Distinct().ToList();

            // Delete wins before running auction again
            _context.WinResults.Where(w => w.BatchRef == bidBatch).ToList().ForEach(p => _context.WinResults.Remove(p));

            if (auctionType == "bestrate")
            {
                foreach (var date in res)
                {
                    // Direct SQL to pick winners for a date
                    string connectionString = "Server=JEROME-SBOOK\\SQLEXPRESS01;Database=MathaRx;Trusted_Connection=True;MultipleActiveResultSets=true;";
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
            }
            else if (auctionType == "waverage")
            {
                foreach (var date in res)
                {
                    IQueryable<AuctionBid> bids = from j in _context.AuctionBids
                                                  where j.FwdDate == date
                                                  select j;
                    var totalAmt = bids.Select(x => x.AmountBid).Sum();
                    var bidArray = bids.ToArray();

                    DataTable dTable = new DataTable();
                    dTable.Columns.Add("ID", typeof(int));
                    dTable.Columns.Add("FwdDate", typeof(DateTime));
                    dTable.Columns.Add("CouponAmount", typeof(decimal));
                    dTable.Columns.Add("BankName", typeof(string));
                    dTable.Columns.Add("AmountBid", typeof(decimal));
                    dTable.Columns.Add("FwdRate", typeof(double));
                    dTable.Columns.Add("awarded_amount", typeof(decimal));
                    dTable.Columns.Add("BatchRef", typeof(string));

                    foreach (var row in bids)
                    {
                        DataRow dRow = dTable.NewRow();
                        dRow[0] = row.ID;
                        dRow[1] = row.FwdDate;
                        dRow[2] = row.CouponAmount;
                        dRow[3] = row.BankName;
                        dRow[4] = row.AmountBid;
                        dRow[5] = row.FwdRate;
                        dRow[6] = row.AmountBid / totalAmt * row.CouponAmount;
                        dRow[7] = row.BatchRef;

                        dTable.Rows.Add(dRow);
                    }

                    // Convert Date wins to object and write to entity
                    foreach (DataRow row in dTable.Rows)
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
