using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.SemesterEndpoints
{
    [Route("years")]
    public class SemesterInsertEndpoint(ApplicationDbContext context): MyEndpointBaseAsync.WithRequest<SemesterInsertRequest>.WithActionResult<SemesterForStudentsResponse>
    {
        [HttpPost("insert")]
        public override async Task<ActionResult<SemesterForStudentsResponse>> HandleAsync(SemesterInsertRequest request, CancellationToken cancellationToken = default)
        {
            var std = await context.StudentsAll.FindAsync(request.StudentId, cancellationToken);

            if (std == null)
            {
                return NotFound();
            }
            bool renew = await context.StudyYears.Where(x => x.GodinaStudija == request.GodinaStudija && 
                                                        x.StudentId == request.StudentId).FirstOrDefaultAsync(cancellationToken) != null;

            var academic=await context.AcademicYears.FindAsync(request.AkademskaGodinaId, cancellationToken);

            if (academic == null)
            {
                return BadRequest($"AcademicYear with ID {request.AkademskaGodinaId} does not exist.");
            }

            var nova = new StudyYears()
            {
                StudentId = request.StudentId,
                GodinaStudija = request.GodinaStudija,
                AkademskaGodinaId = request.AkademskaGodinaId,
                DatumUpisa = request.DatumUpisa,
                SnimioId = request.SnimioId,

            };
            nova.Obnova = renew;
            nova.CijenaSkolarine = renew ? 400f : 1800f;
            try
            {
                await context.AddAsync(nova);
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }

            return Ok(new SemesterForStudentsResponse
            {
                Id = nova.Id,
                AkademskaGodina = nova.AkademskaGodinaId,
                DatumUpisa = nova.DatumUpisa,
                GodinaStudija = nova.GodinaStudija,
                Obnova = nova.Obnova,
                Snimio = nova.Snimio?.Email ?? "",
                DatumOvjere = nova.DatumOvjere,
                Komentar = nova.NapomenaZaOvjeru
            });

        }
        
    }
    public class SemesterInsertRequest
    {
        public int StudentId { get; set; }
        public DateTime DatumUpisa { get; set; }
        public int GodinaStudija { get; set; }
        public int AkademskaGodinaId { get; set; }
        public int SnimioId { get; set; }
    }
}
