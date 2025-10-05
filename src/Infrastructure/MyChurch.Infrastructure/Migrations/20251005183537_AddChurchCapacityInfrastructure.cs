using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyChurch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChurchCapacityInfrastructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalFacilities",
                schema: "postgres",
                table: "church",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentNotes",
                schema: "postgres",
                table: "church",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasAirConditioning",
                schema: "postgres",
                table: "church",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasBaptistery",
                schema: "postgres",
                table: "church",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasBookstore",
                schema: "postgres",
                table: "church",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasCafeteria",
                schema: "postgres",
                table: "church",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasNursery",
                schema: "postgres",
                table: "church",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasProjector",
                schema: "postgres",
                table: "church",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSoundSystem",
                schema: "postgres",
                table: "church",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasWifi",
                schema: "postgres",
                table: "church",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParkingSpaces",
                schema: "postgres",
                table: "church",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeatingCapacity",
                schema: "postgres",
                table: "church",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StandingCapacity",
                schema: "postgres",
                table: "church",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WifiPassword",
                schema: "postgres",
                table: "church",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalFacilities",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "EquipmentNotes",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "HasAirConditioning",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "HasBaptistery",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "HasBookstore",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "HasCafeteria",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "HasNursery",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "HasProjector",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "HasSoundSystem",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "HasWifi",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "ParkingSpaces",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "SeatingCapacity",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "StandingCapacity",
                schema: "postgres",
                table: "church");

            migrationBuilder.DropColumn(
                name: "WifiPassword",
                schema: "postgres",
                table: "church");
        }
    }
}
