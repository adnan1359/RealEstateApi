using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RealEstateApi.Data;
using RealEstateApi.Models;
using System.Security.Claims;

namespace RealEstateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        ApiDbContext context = new ApiDbContext();


        [HttpGet]
        [Authorize]
        public IActionResult GetProperties(int categoryId)
        {

            var propertiesResult = context.Properties.Where(c => c.CategoryId == categoryId);

            if (propertiesResult == null)
                return NotFound();

            return Ok(propertiesResult);
        }


        // Routing Link
        [HttpGet("PropertyDetail")]
        [Authorize]
        public IActionResult GetPropertyDetail(int id)
        {
            var propertyDetail = context.Properties.FirstOrDefault(c => c.CategoryId == id);

            if (propertyDetail == null)
                return NotFound();

            return Ok(propertyDetail);
        }


        [HttpGet("TrendingProperties")]
        [Authorize]
        public IActionResult GetTrendingProperties()
        {
            var trendingProperties = context.Properties.Where(p => p.IsTrending == true);

            if (trendingProperties == null)
                return NotFound();

            return Ok(trendingProperties);
        }



        [HttpGet("SearchProperties")]
        [Authorize]
        public IActionResult GetSearchProperties(string address)
        {
            var trendingProperties = context.Properties.Where(p => p.Address == address);

            if (trendingProperties == null)
                return NotFound();

            return Ok(trendingProperties);
        }




        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok(context.Categories);
        }



        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] Models.Property property)
        {

            if (property == null)
                return NoContent();

            var userEmail = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Email)?.Value;

            var user = context.Users.First(u => u.Email == userEmail);

            if (user == null) return NotFound();

            property.IsTrending = false;
            property.UserId = user.Id;

            context.Properties.Add(property);
            context.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }




        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Put(int id, [FromBody] Models.Property property)
        {

            var propertyResult = context.Properties.First(u => u.Id == id);
            if (propertyResult == null)
                return NotFound();

            var userEmail = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Email)?.Value;

            var user = context.Users.First(u => u.Email == userEmail);

            if (user == null) return NotFound();

            if(propertyResult.Id == id)
            {
            propertyResult.Name = property.Name;
            propertyResult.Detail = property.Detail;
            propertyResult.Price = property.Price;
            propertyResult.Address = property.Address;
            propertyResult.IsTrending = false;
            propertyResult.UserId = user.Id;

            context.SaveChanges();
            return Ok("Record Updated Successfully!!");
            }

            return BadRequest();
        }



        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {

            var propertyResult = context.Properties.First(u => u.Id == id);
            if (propertyResult == null)
                return NotFound();

            var userEmail = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Email)?.Value;

            var user = context.Users.First(u => u.Email == userEmail);

            if (user == null) return NotFound();

            if (propertyResult.Id == id)
            {
                context.Properties.Remove(propertyResult);
                context.SaveChanges();
                return Ok("Record Updated Successfully!!");
            }

            return BadRequest();
        }


    }
}
