using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PoliciesController : ControllerBase
{
    private static readonly List<Policy> Policies =
    [
        new Policy
        {
            Id = 1,
            CustomerId = 1,
            PolicyNumber = "POL1001",
            Type = "Health",
            Premium = 15000
        },
        new Policy
        {
            Id = 2,
            CustomerId = 2,
            PolicyNumber = "POL1002",
            Type = "Life",
            Premium = 25000
        }
    ];

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(Policies);
    }

    [HttpGet("customer/{customerId}")]
    public IActionResult GetByCustomer(int customerId)
    {
        return Ok(
            Policies.Where(
                x => x.CustomerId == customerId));
    }
}

public class Policy
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string PolicyNumber { get; set; } = "";

    public string Type { get; set; } = "";

    public decimal Premium { get; set; }
}