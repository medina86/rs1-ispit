using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SemesterEndpoints
{
    [Route("years")]
    public class AcademicYearsGetAllEndpoint(ApplicationDbContext context): MyEndpointBaseAsync.WithoutRequest.WithActionResult<List<AcademicYearGetResponse>>
    {
        [HttpGet("academic")]
        public override async Task<ActionResult<List<AcademicYearGetResponse>>> HandleAsync(CancellationToken cancellationToken)
        {
            return Ok(await context.AcademicYears.Select(ay => new AcademicYearGetResponse
            {
                Id = ay.ID,
                Name = ay.Description,
                StartDate = new DateTime(ay.StartDate, TimeOnly.MinValue),
                EndDate = new DateTime(ay.EndDate, TimeOnly.MinValue)
            }).ToListAsync(cancellationToken));
        }

    }
    public class AcademicYearGetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
