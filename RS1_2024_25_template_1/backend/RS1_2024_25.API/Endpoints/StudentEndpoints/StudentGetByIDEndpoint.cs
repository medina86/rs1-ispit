using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    [Route("students")]
    public class StudentGetByIDEndpoint(ApplicationDbContext context): MyEndpointBaseAsync.WithRequest<int>.WithActionResult<StudentGetByIdResponse>
    {
        [HttpGet("id/{id}")]
        public override async Task<ActionResult<StudentGetByIdResponse>>HandleAsync(int id, CancellationToken cancellation = default)
        {
            await context.Municipalities.LoadAsync();
            await context.Cities.LoadAsync();
            await context.StudentsAll.LoadAsync();
            await context.Regions.LoadAsync();
            await context.Countries.LoadAsync();
            await context.MyAppUsers.LoadAsync();



            var std =await context.StudentsAll.FindAsync(id, cancellation);
            if (std == null)
            {
                return NotFound();
            }
            return Ok(new StudentGetByIdResponse()
            {
                Id = std.ID,
                FirstName = std.User.FirstName,
                LastName = std.User.LastName,
                StudentNumber = std.StudentNumber,
                BirthDate = std.BirthDate,
                MunicipalityId = std.BirthMunicipalityId,
                MunicipalityName = std.BirthMunicipality.Name,
                Phone = std.ContactMobilePhone,
                CountryId = context.Municipalities.Where(x => x.ID == std.BirthMunicipalityId).Select(x => x.City.Region.CountryId).FirstOrDefault(),
                IsDeleted = std.IsDeleted,
            });
        }


    }
    public class StudentGetByIdResponse
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string StudentNumber { get; set; }
        public string Phone { get; set; } = string.Empty;
        public DateOnly? BirthDate { get; set; }
        public int? MunicipalityId { get; set; }
        public string MunicipalityName { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
