using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Classes;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiVersion("1.0")]
    //[Route("v{v:apiVersion}/[controller]")]
    [Route("products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsController(ShopContext shopContext)
        {
            _context = shopContext;
            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products;
            
            if(queryParameters.MinPrice.HasValue && queryParameters.MaxPrice.HasValue)
            {
                products = products.Where(p => p.Price >= queryParameters.MinPrice.Value && p.Price <= queryParameters.MaxPrice.Value);
            }

            if(!string.IsNullOrWhiteSpace(queryParameters.Sku))
            {
                products = products.Where(p => p.Sku == queryParameters.Sku.Trim());
            }

            if (!string.IsNullOrWhiteSpace(queryParameters.Name))
            {
                products = products.Where(p => p.Name.ToLower().Contains(queryParameters.Name.ToLower().Trim()));
            }

            if (!string.IsNullOrWhiteSpace(queryParameters.SortBy))
            {
                if(typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }

            products = products.Skip(queryParameters.Size * queryParameters.Page - 1).Take(queryParameters.Size);
            return Ok(await products.ToArrayAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(await product);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] Product product)
        {
            if(id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(_context.Products.Find(id) == null)
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }
    }

    [ApiVersion("2.0")]
    //[Route("v{v:apiVersion}/products")]
    [Route("products")]
    [ApiController]
    public class ProductsV2_0Controller : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsV2_0Controller(ShopContext shopContext)
        {
            _context = shopContext;
            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products.Where(x=> x.IsAvailable);

            if (queryParameters.MinPrice.HasValue && queryParameters.MaxPrice.HasValue)
            {
                products = products.Where(p => p.Price >= queryParameters.MinPrice.Value && p.Price <= queryParameters.MaxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(queryParameters.Sku))
            {
                products = products.Where(p => p.Sku == queryParameters.Sku.Trim());
            }

            if (!string.IsNullOrWhiteSpace(queryParameters.Name))
            {
                products = products.Where(p => p.Name.ToLower().Contains(queryParameters.Name.ToLower().Trim()));
            }

            if (!string.IsNullOrWhiteSpace(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }

            products = products.Skip(queryParameters.Size * queryParameters.Page - 1).Take(queryParameters.Size);
            return Ok(await products.ToArrayAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(await product);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Products.Find(id) == null)
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }
    }
}
