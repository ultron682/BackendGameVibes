using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Requests;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;
using BackendGameVibes.Models.User;

namespace BackendGameVibes.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        public ReviewController(IReviewService reviewService, IMapper mapper) {
            _reviewService = reviewService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews() {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(int id) {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null) {
                return NotFound();
            }
            return Ok(review);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation("Require authorization")]
        public async Task<IActionResult> AddReview([FromBody] ReviewDTO reviewRequest) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not authenticated, claim not found");

            Review newReview = _mapper.Map<Review>(reviewRequest);
            newReview.UserGameVibesId = userId;

            Review? review = await _reviewService.AddReviewAsync(newReview);
            if (review != null)
                return CreatedAtAction(nameof(GetReviewById), new { id = newReview.Id }, review);
            else
                return BadRequest("No gameId in db");
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateReview(int id, [FromBody] Review review) {
        //    if (id != review.Id) {
        //        return BadRequest();
        //    }

        //    await _reviewService.UpdateReviewAsync(review);
        //    return NoContent();
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id) {
            await _reviewService.DeleteReviewAsync(id);
            return NoContent();
        }
    }
}
