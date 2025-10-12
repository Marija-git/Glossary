using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Glossary.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addFilteredUniqueIndexForTerm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GlossaryTerms_Term",
                table: "GlossaryTerms");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryTerms_Term",
                table: "GlossaryTerms",
                column: "Term",
                unique: true,
                filter: "\"Term\" <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GlossaryTerms_Term",
                table: "GlossaryTerms");

            migrationBuilder.CreateIndex(
                name: "IX_GlossaryTerms_Term",
                table: "GlossaryTerms",
                column: "Term",
                unique: true,
                filter: "Term <> ''");
        }
    }
}
