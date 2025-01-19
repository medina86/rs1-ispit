using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    [Route("students")]
    public class StudentUpdateEndpoint(ApplicationDbContext context): MyEndpointBaseAsync.WithRequest<StudentUpdateRequest>.WithActionResult<StudentUpdateResponse>
    {
        [HttpPut("update")]
        public override async Task<ActionResult<StudentUpdateResponse>>HandleAsync(StudentUpdateRequest request, CancellationToken cancellation=default)
        {
            await context.Municipalities.LoadAsync();
            await context.Cities.LoadAsync();   
            await context.StudentsAll.LoadAsync();
            var std=await context.StudentsAll.FindAsync(request.Id, cancellation);

            if (std == null)
            {
                return NotFound();
            }
            std.ContactMobilePhone=request.Phone;
            std.BirthDate=DateOnly.FromDateTime(request.BirthDate);
            std.ID=request.Id;
            std.BirthMunicipalityId=request.MunicipalityId;

            await context.SaveChangesAsync(cancellation);

            return Ok(new StudentUpdateResponse()
            {
                Id=std.ID,
                BirthDate=std.BirthDate,
                MunicipalityId=std.BirthMunicipalityId,
                MunicipalityName=std.BirthMunicipality.Name,
                Phone=std.ContactMobilePhone
            });
        }
    }
    public class StudentUpdateRequest
    {
        public int Id { get; set; }
        public string Phone { get; set; }=string.Empty;
        public DateTime BirthDate { get; set; }
        public int MunicipalityId { get; set; }
    }
    public class StudentUpdateResponse
    {
        public int Id { get; set; }
        public string Phone { get; set; } = string.Empty;
        public DateOnly? BirthDate { get; set; }
        public int? MunicipalityId { get; set; }
        public string MunicipalityName { get; set; } = string.Empty;
    }
}
