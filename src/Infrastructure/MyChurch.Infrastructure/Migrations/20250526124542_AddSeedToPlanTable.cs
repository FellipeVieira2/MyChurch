using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedToPlanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            migrationBuilder.Sql($@"
                INSERT INTO plan (""id"", ""name"", ""price"", ""max_members"", ""max_events"", ""max_storage_gb"", ""branches"", ""created"", ""updated"") VALUES
                (1, 'Descubra', 0, 50, 10, 1, 1, '{now}', NULL),
                (2, 'Crescer', 69, 500, 50, 5, 2, '{now}', NULL),
                (3, 'Multiplicar', 149, 3000, 200, 20, 5, '{now}', NULL),
                (4, 'Influenciar', 279, 10000, 1000, 100, 2147483647, '{now}', NULL);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
