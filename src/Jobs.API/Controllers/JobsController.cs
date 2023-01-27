using Jobs.API.Data;
using Jobs.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobs.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly ILogger<JobsController> logger;
    private readonly ApplicationContext context;

    public JobsController(ILogger<JobsController> logger, ApplicationContext context)
    {
        this.logger = logger;
        this.context = context;
    }

    [HttpGet]
    public async Task<IEnumerable<Job>> Get()
    {
        return await context.Jobs.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<Job> Get(int id)
    {
        return await context.Jobs.FindAsync(id);
    }
}
