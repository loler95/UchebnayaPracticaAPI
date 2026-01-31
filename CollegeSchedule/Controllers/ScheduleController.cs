using CollegeSchedule.Data;
using CollegeSchedule.Models;
using CollegeSchedule.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CollegeSchedule.Controllers
{
    [ApiController]
    [Route("api/schedule")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _service;
        public ScheduleController(IScheduleService service, AppDbContext db)
        {
            _service = service;
        }

        // GET: api/schedule/group/{groupName}?start=start.Date&end=end.Date
        [HttpGet("group/{groupName}")]
        public async Task<IActionResult> GetSchedule(
            [FromRoute] string groupName,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            // вызываем логику из сервиса
            if (start == DateTime.MinValue && end == DateTime.MinValue)
            {
                // Если оба периода не указаны, показываем текущую неделю
                start = DateTime.Today;
                end = DateTime.Today.AddDays(6);
            }
            else if (start == DateTime.MinValue)
            {
                // Если указана только конечная дата, показываем один день
                start = end.Date;
            }
            else if (end == DateTime.MinValue)
            {
                // Если указана только начальная дата, показываем неделю от неё
                end = start.Date.AddDays(6);
            }
            var result = await _service.GetScheduleForGroup(groupName, start.Date, end.Date);
            return Ok(result);
        }
    }
}