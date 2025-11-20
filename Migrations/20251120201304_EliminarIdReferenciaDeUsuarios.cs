using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2rismBeta.API.Migrations
{
    /// <inheritdoc />
    public partial class EliminarIdReferenciaDeUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_referencia",
                table: "usuarios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_referencia",
                table: "usuarios",
                type: "int",
                nullable: true);
        }
    }
}
