using CustomerAPI.Data;
using CustomerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CustomerAPI.Events;
using CustomerAPI.Messaging;

namespace CustomerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly CustomerDbContext _db;
    private readonly RabbitMqPublisher _rabbitMqPublisher;

    public CustomersController(CustomerDbContext db, RabbitMqPublisher rabbitMqPublisher)
    {
        _db = db;
        _rabbitMqPublisher = rabbitMqPublisher;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _db.Customers.ToListAsync();

        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var customer =
            await _db.Customers.FindAsync(id);

        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        _db.Customers.Add(customer);

        await _db.SaveChangesAsync();

        // Publish the CustomerCreatedEvent to RabbitMQ

        var customerCreatedEvent = new CustomerCreatedEvent
        {
            CustomerId = customer.Id,
            Name = customer.Name,
            Email = customer.Email
        };

        await _rabbitMqPublisher.PublishCustomerCreatedAsync(
            customerCreatedEvent);

        return CreatedAtAction(
            nameof(Get),
            new { id = customer.Id },
            customer);
    }
}