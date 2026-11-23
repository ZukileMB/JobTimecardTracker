using JobTimecardTracker.Domain;

namespace JobTimecardTracker.Api.Contracts;

public record JobDto(
    Guid Id,
    string JobNumber,
    JobType JobType,
    DateTime JobDate,
    string CompanyName,
    string ContactName,
    string TelephoneNumber);

public record CreateJobRequest(
    string JobNumber,
    JobType JobType,
    DateTime JobDate,
    string CompanyName,
    string ContactName,
    string TelephoneNumber);

public record TimecardDto(
    Guid Id,
    string Employee,
    DateTime DateWorked,
    decimal HoursWorked,
    Guid JobId,
    string JobNumber,
    string CompanyName);

public record CreateTimecardRequest(
    string Employee,
    DateTime DateWorked,
    decimal HoursWorked,
    Guid JobId);