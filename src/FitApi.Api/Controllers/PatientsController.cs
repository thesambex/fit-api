using FitApi.Core.Domain.Common;
using FitApi.Core.Domain.Patients.DTOs;
using FitApi.Core.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FitApi.Api.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController(IPatientService patientService) : ControllerBase
{
    /// <summary>
    /// Create Patient
    /// </summary>
    /// <param name="requestBody">Request Body</param>
    /// <param name="validator"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("", Name = "CreatePatient")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PatientResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<PatientResponse>> Create(
        [FromBody] CreatePatientRequest requestBody,
        IValidator<CreatePatientRequest> validator
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

        var response = await patientService.Create(requestBody);

        return CreatedAtAction("FindById", new { id = response.Id }, response);
    }

    /// <summary>
    /// Find patient by Id
    /// </summary>
    /// <param name="id">Patient Id</param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:guid}", Name = "FindPatientById")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<PatientResponse>> FindById(Guid id)
    {
        return await patientService.FindById(id);
    }

    /// <summary>
    /// Delete patient by Id
    /// </summary>
    /// <param name="id">Patient Id</param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id:guid}", Name = "DeletePatientById")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        await patientService.Delete(id);

        return NoContent();
    }

    /// <summary>
    /// Update patient
    /// </summary>
    /// <param name="id">Patient Id</param>
    /// <param name="requestBody">Request Body</param>
    /// <param name="validator"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("{id:guid}", Name = "UpdatePatient")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<PatientResponse>> Update(
        Guid id,
        UpdatePatientRequest requestBody,
        IValidator<UpdatePatientRequest> validator
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

        return await patientService.Update(id, requestBody);
    }

    /// <summary>
    /// Find all patients
    /// </summary>
    /// <param name="pageIndex">Page Index</param>
    /// <param name="pageSize">Page Size</param>
    /// <returns></returns>
    [HttpGet]
    [Route("all", Name = "FindAllPatients")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationResponse<PatientResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<PaginationResponse<PatientResponse>>> FindAll(int pageIndex = 1, int pageSize = 25)
    {
        return await patientService.FindAll(pageIndex, pageSize);
    }

    /// <summary>
    /// Search Patients
    /// </summary>
    /// <param name="q">Query</param>
    /// <param name="pageIndex">Page Index</param>
    /// <param name="pageSize">Page Size</param>
    /// <returns></returns>
    [HttpGet]
    [Route("search", Name = "SearchPatients")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationResponse<PatientResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<PaginationResponse<PatientResponse>>> Search(
        string q,
        int pageIndex = 1,
        int pageSize = 25
    )
    {
        return await patientService.Search(q, pageIndex, pageSize);
    }
}