using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    [Route("municiplaities")]
    public class MunicipalitiesGetAllEndpoint(ApplicationDbContext db):MyEndpointBaseAsync.WithRequest<int>.WithActionResult<List<MunicipalitiesResponse>>
    {
        [HttpGet("all/{id}")]
        public override async Task<ActionResult<List<MunicipalitiesResponse>>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            await db.Municipalities.LoadAsync();
            await db.Cities.LoadAsync();
            await db.StudentsAll.LoadAsync();
            await db.Regions.LoadAsync();
            await db.Countries.LoadAsync();
            var result = await db.Municipalities.Where(x=>x.City.Region.CountryId==id)
                            .Select(c => new MunicipalitiesResponse
                            {
                                ID = c.ID,
                                Name = c.Name
                            })
                            .ToListAsync(cancellationToken);

            return result;
        }
    }
        public class MunicipalitiesResponse
        {
            public required int ID { get; set; }
            public required string Name { get; set; }
        }
    

}
