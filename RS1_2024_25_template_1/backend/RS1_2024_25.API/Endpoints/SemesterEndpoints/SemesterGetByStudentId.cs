using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SemesterEndpoints
{
    [Route("years")]
    public class SemesterGetByStudentId(ApplicationDbContext context): MyEndpointBaseAsync.WithRequest<int>.WithActionResult<List<SemesterForStudentsResponse>>
    {
        [HttpGet("all/{id}")]
        public override async Task<ActionResult<List<SemesterForStudentsResponse>>>HandleAsync(int id, CancellationToken cancellation = default)
        {
            await context.AcademicYears.LoadAsync();
            await context.StudentsAll.LoadAsync();
            await context.MyAppUsers.LoadAsync();

            var std= await context.StudentsAll.FindAsync(id,cancellation);

            if (std == null)
            {
                return NotFound();
            }

            return Ok(await context.StudyYears.Where(x=>x.StudentId==id). Select(x=> new SemesterForStudentsResponse()
            {
                Id = x.Id,
                AkademskaGodina = x.AkademskaGodina.ID,
                DatumUpisa = x.DatumUpisa,
                GodinaStudija = x.GodinaStudija,
                Obnova = x.Obnova,
                Snimio = x.Snimio.Email,
                DatumOvjere = x.DatumOvjere,
                Komentar = x.NapomenaZaOvjeru
            }).ToListAsync(cancellation));
        }
    }

    public class SemesterForStudentsResponse
    {
        public int Id { get; set; }
        public int AkademskaGodina { set; get; }
        public int GodinaStudija { set; get; }
        public bool Obnova { get; set; }
        public DateTime DatumUpisa { set; get; }
        public string Snimio { set; get; } = string.Empty;
        public DateTime? DatumOvjere { set; get; }
        public string? Komentar { set; get; }

    }
}
