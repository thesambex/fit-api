using FitApi.Core.Domain.Assessments.DTOs;
using FitApi.Core.Domain.Common;
using FitApi.Core.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FitApi.Api.Controllers;

[ApiController]
[Route("api/assessments")]
public class AssessmentsController(IAssessmentService assessmentService) : ControllerBase
{
    /// <summary>
    /// Create new assessment
    /// </summary>
    /// <param name="requestBody">Request Body</param>
    /// <param name="validator"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("", Name = "CreateAssessment")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AssessmentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<AssessmentResponse>> Create(
        [FromBody] CreateAssessmentRequest requestBody,
        [FromServices] IValidator<CreateAssessmentRequest> validator
    )
    {
        var validationResult = await validator.ValidateAsync(requestBody);
        if (!validationResult.IsValid)
        {
            var problem = new ProblemDetails
            {
                Title = "Bad Request",
                Detail = string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)),
                Status = StatusCodes.Status400BadRequest,
                Instance = HttpContext.Request.Path,
                Type = "https://httpstatuses.com/400"
            };

            return BadRequest(problem);
        }

        var response = await assessmentService.Create(requestBody);

        return CreatedAtAction("FindById", new { id = response.Id }, response);
    }

    /// <summary>
    /// Find detailed assessment by Id
    /// </summary>
    /// <param name="id">Assessment Id</param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:guid}", Name = "FindAssessmentById")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AssessmentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<AssessmentResponse>> FindById(Guid id)
    {
        return await assessmentService.FindById(id);
    }

    /// <summary>
    /// Find all assessments from patient
    /// </summary>
    /// <param name="patientId">Patient Id</param>
    /// <param name="pageIndex">Page Index</param>
    /// <param name="pageSize">Page Size</param>
    /// <returns></returns>
    [HttpGet]
    [Route("patient/{patientId:guid}/all", Name = "FindAssessmentsByPatientId")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationResponse<AssessmentBriefResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<PaginationResponse<AssessmentBriefResponse>>> FindAllByPatient(
        Guid patientId,
        int pageIndex = 1,
        int pageSize = 25
    )
    {
        return await assessmentService.FindAllByPatient(patientId, pageIndex, pageSize);
    }

    /// <summary>
    /// Update Assessment
    /// </summary>
    /// <param name="id">Assessment Id</param>
    /// <param name="requestBody">Request Body</param>
    /// <param name="validator"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("{id:guid}", Name = "UpdateAssessment")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AssessmentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<AssessmentResponse>> Update(
        Guid id,
        [FromBody] UpdateAssessmentRequest requestBody,
        [FromServices] IValidator<UpdateAssessmentRequest> validator
    )
    {
        var validationResult = await validator.ValidateAsync(requestBody);
        if (!validationResult.IsValid)
        {
            var problem = new ProblemDetails
            {
                Title = "Bad Request",
                Detail = string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)),
                Status = StatusCodes.Status400BadRequest,
                Instance = HttpContext.Request.Path,
                Type = "https://httpstatuses.com/400"
            };

            return BadRequest(problem);
        }

        return await assessmentService.Update(id, requestBody);
    }

    /// <summary>
    /// Delete assessment
    /// </summary>
    /// <param name="id">Assessment Id</param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id:guid}", Name = "DeleteAssessmentById")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(Guid id)
    {
        await assessmentService.Delete(id);

        return NoContent();
    }
}