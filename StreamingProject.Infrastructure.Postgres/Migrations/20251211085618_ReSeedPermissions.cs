using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamingProject.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ReSeedPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RolePermissionEntity",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 2, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissionEntity",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 2, 2 });
        }
    }
}
