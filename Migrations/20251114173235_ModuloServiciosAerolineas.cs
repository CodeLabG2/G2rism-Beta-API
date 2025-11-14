using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2rismBeta.API.Migrations
{
    /// <inheritdoc />
    public partial class ModuloServiciosAerolineas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aerolineas",
                columns: table => new
                {
                    id_aerolinea = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    codigo_iata = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    codigo_icao = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    pais = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sitio_web = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefono_contacto = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email_contacto = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    politicas_equipaje = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_creacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aerolineas", x => x.id_aerolinea);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vuelos",
                columns: table => new
                {
                    id_vuelo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_aerolinea = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vuelos", x => x.id_vuelo);
                    table.ForeignKey(
                        name: "FK_vuelos_aerolineas_id_aerolinea",
                        column: x => x.id_aerolinea,
                        principalTable: "aerolineas",
                        principalColumn: "id_aerolinea",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Aerolineas_CodigoIata",
                table: "aerolineas",
                column: "codigo_iata",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aerolineas_CodigoIcao",
                table: "aerolineas",
                column: "codigo_icao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aerolineas_Estado",
                table: "aerolineas",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_Aerolineas_Nombre",
                table: "aerolineas",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "IX_Aerolineas_Pais",
                table: "aerolineas",
                column: "pais");

            migrationBuilder.CreateIndex(
                name: "IX_vuelos_id_aerolinea",
                table: "vuelos",
                column: "id_aerolinea");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vuelos");

            migrationBuilder.DropTable(
                name: "aerolineas");
        }
    }
}
