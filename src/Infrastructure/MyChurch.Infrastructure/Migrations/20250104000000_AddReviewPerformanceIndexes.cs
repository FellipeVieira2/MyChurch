using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Índice composto para otimizar contagem de votos por tipo
            migrationBuilder.CreateIndex(
                name: "IX_ReviewVotes_ReviewId_IsHelpful",
                table: "ReviewVotes",
                columns: new[] { "ReviewId", "IsHelpful" });

            // Índice composto para busca de reviews por entidade com ordenação
            migrationBuilder.CreateIndex(
                name: "IX_Reviews_EntityId_EntityType_CreatedAt",
                table: "Reviews",
                columns: new[] { "EntityId", "EntityType", "CreatedAt" });

            // Índice para buscar reviews verificadas
            migrationBuilder.CreateIndex(
                name: "IX_Reviews_IsVerified",
                table: "Reviews",
                column: "IsVerified");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReviewVotes_ReviewId_IsHelpful",
                table: "ReviewVotes");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_EntityId_EntityType_CreatedAt",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_IsVerified",
                table: "Reviews");
        }
    }
}
