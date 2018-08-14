using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MathaCapital.Data;
using MathaCapital.Models;

namespace MathaCapital.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionBidsController : ControllerBase
    {
        private readonly AuctionContext _context;

        public AuctionBidsController(AuctionContext context)
        {
            _context = context;
        }

        // GET: api/AuctionBids
        [HttpGet]
        public IEnumerable<AuctionBid> GetAuctionBids()
        {
            return _context.AuctionBids;
        }

        // GET: api/AuctionBids/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuctionBid([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var auctionBid = await _context.AuctionBids.FindAsync(id);

            if (auctionBid == null)
            {
                return NotFound();
            }

            return Ok(auctionBid);
        }

        // PUT: api/AuctionBids/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuctionBid([FromRoute] int id, [FromBody] AuctionBid auctionBid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != auctionBid.ID)
            {
                return BadRequest();
            }

            _context.Entry(auctionBid).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuctionBidExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AuctionBids
        [HttpPost]
        public async Task<IActionResult> PostAuctionBid([FromBody] AuctionBid auctionBid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AuctionBids.Add(auctionBid);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuctionBid", new { id = auctionBid.ID }, auctionBid);
        }

        // DELETE: api/AuctionBids/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuctionBid([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var auctionBid = await _context.AuctionBids.FindAsync(id);
            if (auctionBid == null)
            {
                return NotFound();
            }

            _context.AuctionBids.Remove(auctionBid);
            await _context.SaveChangesAsync();

            return Ok(auctionBid);
        }

        private bool AuctionBidExists(int id)
        {
            return _context.AuctionBids.Any(e => e.ID == id);
        }
    }
}