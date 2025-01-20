using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RS1_2024_25.API.Migrations
{
    /// <inheritdoc />
    public partial class study : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudyYearses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumUpisa = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GodinaStudija = table.Column<int>(type: "int", nullable: false),
                    AkademskaGodinaId = table.Column<int>(type: "int", nullable: false),
                    CijenaSkolarine = table.Column<float>(type: "real", nullable: false),
                    Obnova = table.Column<bool>(type: "bit", nullable: false),
                    DatumOvjere = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NapomenaZaOvjeru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SnimioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyYearses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyYearses_AcademicYears_AkademskaGodinaId",
                        column: x => x.AkademskaGodinaId,
                        principalTable: "AcademicYears",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_StudyYearses_MyAppUsers_SnimioId",
                        column: x => x.SnimioId,
                        principalTable: "MyAppUsers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_StudyYearses_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyYearses_AkademskaGodinaId",
                table: "StudyYearses",
                column: "AkademskaGodinaId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyYearses_SnimioId",
                table: "StudyYearses",
                column: "SnimioId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyYearses_StudentId",
                table: "StudyYearses",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyYearses");
        }
    }
}
