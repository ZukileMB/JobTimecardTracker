using JobTimecardTracker.Api.Contracts;
using JobTimecardTracker.Domain;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace JobTimecardTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly ISessionFactory _sessionFactory;

    public JobsController(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    [HttpGet]
    public async Task<ActionResult<List<JobDto>>> GetAll(CancellationToken ct)
    {
        using var session = _sessionFactory.OpenSession();
        var jobs = await session.Query<Job>()
            .OrderByDescending(j => j.JobDate)
            .ThenBy(j => j.JobNumber)
            .Select(j => new JobDto(
                j.Id,
                j.JobNumber,
                j.JobType,
                j.JobDate,
                j.CompanyName,
                j.ContactName,
                j.TelephoneNumber))
            .ToListAsync(ct);

        return Ok(jobs);
    }

    [HttpPost]
    public async Task<ActionResult<JobDto>> Create([FromBody] CreateJobRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.JobNumber) ||
            string.IsNullOrWhiteSpace(request.CompanyName) ||
            string.IsNullOrWhiteSpace(request.ContactName) ||
            string.IsNullOrWhiteSpace(request.TelephoneNumber))
        {
            return BadRequest("JobNumber, CompanyName, ContactName, and TelephoneNumber are required.");
        }

        using var session = _sessionFactory.OpenSession();
        using var tx = session.BeginTransaction();

        var exists = await session.Query<Job>()
            .AnyAsync(j => j.JobNumber == request.JobNumber.Trim(), ct);
        if (exists)
        {
            return Conflict($"A job with JobNumber '{request.JobNumber.Trim()}' already exists.");
        }

        var job = new Job
        {
            Id = Guid.NewGuid(),
            JobNumber = request.JobNumber.Trim(),
            JobType = request.JobType,
            JobDate = request.JobDate.Date,
            CompanyName = request.CompanyName.Trim(),
            ContactName = request.ContactName.Trim(),
            TelephoneNumber = request.TelephoneNumber.Trim()
        };

        await session.SaveAsync(job, ct);
        await tx.CommitAsync(ct);

        var dto = new JobDto(
            job.Id,
            job.JobNumber,
            job.JobType,
            job.JobDate,
            job.CompanyName,
            job.ContactName,
            job.TelephoneNumber);

        return CreatedAtAction(nameof(GetAll), new { id = job.Id }, dto);
    }
}