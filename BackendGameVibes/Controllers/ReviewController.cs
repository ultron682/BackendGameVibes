using Microsoft.AspNetCore.Mvc;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;
using BackendGameVibes.Models.DTOs.Reported;
using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.Reviews;
using System.ComponentModel.DataAnnotations;


namespace BackendGameVibes.Controllers;
[ApiController]
[Route("api/reviews")]
public class ReviewController : ControllerBase {
    private readonly IReviewService _reviewService;
    private readonly IMapper _mapper;

    public ReviewController(IReviewService reviewService, IMapper mapper) {
        _reviewService = reviewService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllReviews(int pageNumber = 1, int resultSize = 10) {
        var reviews = await _reviewService.GetAllReviewsAsync(pageNumber, resultSize);
        return Ok(reviews);
    }

    [HttpPost("search-phrase")]
    public async Task<IActionResult> GetFilteredReviews([Required] ValueModel searchPhrase, int pageNumber = 1, int resultSize = 10) {
        var reviews = await _reviewService.GetFilteredReviewsAsync(searchPhrase.Value!, pageNumber, resultSize);
        if (reviews == null) {
            return NotFound();
        }
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

    [HttpGet("game/{gameId:int}")]
    public async Task<ActionResult> GetGameReviews(int gameId, int pageNumber = 1, int resultSize = 10) {
        var gameReviews = await _reviewService.GetGameReviewsAsync(gameId, pageNumber, resultSize);
        if (gameReviews == null) {
            return NotFound();
        }
        else {
            return Ok(gameReviews);
        }
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

    [HttpPost("report")]
    [Authorize]
    public async Task<IActionResult> ReportReview([FromBody] ReportReviewDTO reportRequest) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized("User not authenticated, claim not found");

        ReportedReview? reportedReview = await _reviewService.ReportReviewAsync(userId, reportRequest);
        if (reportedReview != null)
            return Ok(new {
                reportedReview.Id,
                reportedReview.ReviewId,
                reportedReview.ReporterUserId,
                reportedReview.Reason
            });
        else
            return BadRequest("ErrorOnReportReview");
    }

    [HttpDelete("{id}")]
    [Authorize]
    [SwaggerResponse(200, "deleted")]
    [SwaggerResponse(404, "no review or review not belongs to user")]
    public async Task<IActionResult> DeleteReview(int id) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized("User not authenticated, claim not found");

        bool isSuccess = await _reviewService.DeleteReviewAsync(userId, id);
        if (isSuccess)
            return Ok();
        else
            return NotFound();
    }

    [HttpPatch("{reviewId}")]
    [Authorize]
    [SwaggerResponse(404, "no review or review doesnt belong to user")]
    [SwaggerResponse(200, "updated")]
    public async Task<IActionResult> UpdateReview(int reviewId, ReviewUpdateDTO reviewUpdateDTO) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized("User not authenticated, claim not found");

        Review? review = await _reviewService.UpdateReviewByIdAsync(reviewId, userId, reviewUpdateDTO);
        if (review == null) {
            return NotFound();
        }

        return Ok(review);
    }
}
