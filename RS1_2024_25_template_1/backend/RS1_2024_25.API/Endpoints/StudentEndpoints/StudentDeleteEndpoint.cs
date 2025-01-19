using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using System.Reflection.Metadata.Ecma335;

namespace RS1_2024_25.API.Endpoints.StudentEndpoints
{
    [Route("students")]
    public class StudentDeleteEndpoint(ApplicationDbContext context): MyEndpointBaseAsync.WithRequest<int>.WithActionResult
    {
        [HttpDelete("delete/{id}")]
        public override async Task<ActionResult> HandleAsync(int id, CancellationToken token = default)
        {
            var std= await context.StudentsAll.FindAsync(id, token);
            if (std == null) { 
            return NotFound();
            }
            std.IsDeleted = true;
            await context.SaveChangesAsync(token);
            return Ok(std);
        }
        
    }
}
