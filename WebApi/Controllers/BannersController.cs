using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        private readonly BannersContext _context;

        public BannersController(BannersContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns banner list
        /// </summary>
        /// <returns></returns>        
        [HttpGet]
        public IEnumerable<Banner> GetBanners()
        {
            return _context.Banners;
        }

        /// <summary>
        /// Returns banner details
        /// </summary>
        /// <param name="id">The banner id</param>
        /// <returns>A banner details</returns>
        /// <response code="200">Returns the banner details</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If the banner is not available</response>       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBanner([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var banner = await _context.Banners.FindAsync(id);

            if (banner == null)
            {
                return NotFound();
            }

            return Ok(banner);
        }

        /// <summary>
        /// Returns banner html
        /// </summary>
        /// <param name="id">The banner id</param>
        /// <returns>A banner html</returns>
        /// <response code="200">Returns the banner html</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If the banner is not available</response>        
        [HttpGet("Html/{id}")]
        [Produces("text/html")]
        public async Task<ContentResult> GetBannerHtml([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return new ContentResult { StatusCode = (int)HttpStatusCode.BadRequest };                
            }

            var banner = await _context.Banners.FindAsync(id);

            if (banner == null)
            {
                return new ContentResult { StatusCode = (int)HttpStatusCode.NotFound };
            }

            return new ContentResult
            {                
                StatusCode = (int)HttpStatusCode.OK,
                Content = banner.Html
            };
        }

        /// <summary>
        /// Updates a specific banner
        /// </summary>
        /// <param name="id">The banner id</param>
        /// <param name="banner">The banner object</param>
        /// <returns></returns>       
        /// <response code="400">If the banner object is invalid</response>
        /// <response code="404">If the banner is not available</response>        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBanner([FromRoute] int id, [FromBody] Banner banner)
        {
            if (!ModelState.IsValid || !IsValidHtml(banner.Html))
            {
                return BadRequest(ModelState);
            }

            if (id != banner.Id)
            {
                return BadRequest();
            }

            banner.Modified = DateTime.Today;
            _context.Entry(banner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BannerExists(id))
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

        /// <summary>
        /// Creates a banner
        /// </summary>
        /// <param name="banner">The banner object</param>
        /// <returns>A newly created banner</returns>
        /// <response code="200">Returns the newly created banner</response>
        /// <response code="400">If the banner object is null or invalid</response>                
        [HttpPost]
        public async Task<IActionResult> PostBanner([FromBody] Banner banner)
        {
            if (!ModelState.IsValid || !IsValidHtml(banner.Html))
            {
                return BadRequest(ModelState);
            }            

            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBanner", new { id = banner.Id }, banner);
        }

        /// <summary>
        /// Deletes a specific banner
        /// </summary>
        /// <param name="id">The banner id</param>
        /// <returns></returns>
        /// <response code="200">Returns the deleted banner</response>
        /// <response code="400">If the banner object is invalid</response>
        /// <response code="404">If the banner is null</response>        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanner([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound();
            }

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();

            return Ok(banner);
        }

        private bool BannerExists(int id)
        {
            return _context.Banners.Any(e => e.Id == id);
        }

        /// <summary>
        /// this method is used to validate html content
        /// </summary>
        /// <param name="html">the html value</param>
        /// <returns></returns>
        private bool IsValidHtml(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return !htmlDocument.ParseErrors.Any();
        }
    }
}