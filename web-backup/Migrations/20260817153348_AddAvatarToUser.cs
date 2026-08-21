using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_backup.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Amenities",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "IconClass",
                table: "Amenities",
                newName: "PhoneNumber");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Amenities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Amenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Amenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Amenities");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Amenities");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Amenities");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Amenities",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Amenities",
                newName: "IconClass");
        }
    }
}
