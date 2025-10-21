using FitApi.Core.Domain.Common;
using FitApi.Core.Domain.Professionals.DTOs;
using FitApi.Core.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FitApi.Api.Controllers;

[ApiController]
[Route("api/professionals")]
public class ProfessionalsController(IProfessionalService professionalService) : ControllerBase
{
    /// <summary>
    /// Create professional
    /// </summary>
    /// <param name="requestBody">Request Body</param>
    /// <param name="validator"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("", Name = "CreateProfessional")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProfessionalResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<ProfessionalResponse>> Create(
        [FromBody] CreateProfessionalRequest requestBody,
        IValidator<CreateProfessionalRequest> validator
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

        var response = await professionalService.Create(requestBody);

        return CreatedAtAction("FindById", new { id = response.Id }, response);
    }

    /// <summary>
    /// Find professional by id
    /// </summary>
    /// <param name="id">Professional Id</param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:guid}", Name = "FindProfessionalById")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfessionalResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<ProfessionalResponse>> FindById(Guid id)
    {
        return await professionalService.FindById(id);
    }

    /// <summary>
    /// Find all professionals
    /// </summary>
    /// <param name="pageIndex">Page Index</param>
    /// <param name="pageSize">Page Size</param>
    /// <returns></returns>
    [HttpGet]
    [Route("all", Name = "FindAllProfessionals")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationResponse<ProfessionalResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<PaginationResponse<ProfessionalResponse>>> FindAll(
        int pageIndex = 1,
        int pageSize = 25
    )
    {
        return await professionalService.FindAll(pageIndex, pageSize);
    }

    /// <summary>
    /// Search professionals
    /// </summary>
    /// <param name="q">Query</param>
    /// <param name="pageIndex">Page Index</param>
    /// <param name="pageSize">Page Size</param>
    /// <returns></returns>
    [HttpGet]
    [Route("search", Name = "SearchProfessionals")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationResponse<ProfessionalResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<PaginationResponse<ProfessionalResponse>>> Search(
        string q,
        int pageIndex = 1,
        int pageSize = 25
    )
    {
        return await professionalService.Search(q, pageIndex, pageSize);
    }
}