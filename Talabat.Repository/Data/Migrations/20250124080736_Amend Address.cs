using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AmendAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingAddress_LName",
                table: "Orders",
                newName: "ShippingAddress_LastName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingAddress_LastName",
                table: "Orders",
                newName: "ShippingAddress_LName");
        }
    }
}
