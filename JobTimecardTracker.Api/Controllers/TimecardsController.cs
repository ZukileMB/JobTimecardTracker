using JobTimecardTracker.Api.Contracts;
using JobTimecardTracker.Domain;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace JobTimecardTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimecardsController : ControllerBase
{
    private readonly ISessionFactory _sessionFactory;

    public TimecardsController(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    [HttpGet]
    public async Task<ActionResult<List<TimecardDto>>> GetAll(
        [FromQuery] string? employee,
        [FromQuery] DateTime? dateWorked,
        CancellationToken ct)
    {
        using var session = _sessionFactory.OpenSession();
        var query = session.Query<Timecard>();

        if (!string.IsNullOrWhiteSpace(employee))
        {
            var emp = employee.Trim();
            query = query.Where(t => t.Employee.Contains(emp));
        }

        if (dateWorked.HasValue)
        {
            var targetDate = dateWorked.Value.Date;
            query = query.Where(t => t.DateWorked == targetDate);
        }

        var items = await query
            .OrderByDescending(t => t.DateWorked)
            .ThenBy(t => t.Employee)
            .Select(t => new TimecardDto(
                t.Id,
                t.Employee,
                t.DateWorked,
                t.HoursWorked,
                t.Job != null ? t.Job.Id : Guid.Empty,
                t.Job != null ? t.Job.JobNumber : string.Empty,
                t.Job != null ? t.Job.CompanyName : string.Empty))
            .ToListAsync(ct);

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<TimecardDto>> Create([FromBody] CreateTimecardRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Employee))
        {
            return BadRequest("Employee is required.");
        }

        if (request.HoursWorked <= 0 || request.HoursWorked > 24)
        {
            return BadRequest("HoursWorked must be greater than 0 and up to 24.");
        }

        using var session = _sessionFactory.OpenSession();
        using var tx = session.BeginTransaction();

        var job = await session.GetAsync<Job>(request.JobId, ct);
        if (job is null)
        {
            return BadRequest($"Job with Id '{request.JobId}' was not found.");
        }

        var timecard = new Timecard
        {
            Id = Guid.NewGuid(),
            Employee = request.Employee.Trim(),
            DateWorked = request.DateWorked.Date,
            HoursWorked = request.HoursWorked,
            Job = job
        };

        await session.SaveAsync(timecard, ct);
        await tx.CommitAsync(ct);

        var dto = new TimecardDto(
            timecard.Id,
            timecard.Employee,
            timecard.DateWorked,
            timecard.HoursWorked,
            job.Id,
            job.JobNumber,
            job.CompanyName);

        return CreatedAtAction(nameof(GetAll), new { id = timecard.Id }, dto);
    }
}