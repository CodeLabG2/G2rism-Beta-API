using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2rismBeta.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarReservaPaquete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reservas_paquetes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_reserva = table.Column<int>(type: "int", nullable: false),
                    id_paquete = table.Column<int>(type: "int", nullable: false),
                    numero_personas = table.Column<int>(type: "int", nullable: false),
                    fecha_inicio_paquete = table.Column<DateTime>(type: "DATE", nullable: false),
                    fecha_fin_paquete = table.Column<DateTime>(type: "DATE", nullable: false),
                    precio_por_persona = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    personalizaciones = table.Column<string>(type: "json", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_agregado = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservas_paquetes", x => x.id);
                    table.ForeignKey(
                        name: "fk_reserva_paquete_paquete",
                        column: x => x.id_paquete,
                        principalTable: "paquetes_turisticos",
                        principalColumn: "id_paquete",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reserva_paquete_reserva",
                        column: x => x.id_reserva,
                        principalTable: "reservas",
                        principalColumn: "id_reserva",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "idx_reserva_paquete_fecha_inicio",
                table: "reservas_paquetes",
                column: "fecha_inicio_paquete");

            migrationBuilder.CreateIndex(
                name: "idx_reserva_paquete_paquete",
                table: "reservas_paquetes",
                column: "id_paquete");

            migrationBuilder.CreateIndex(
                name: "idx_reserva_paquete_paquete_reserva",
                table: "reservas_paquetes",
                columns: new[] { "id_paquete", "id_reserva" });

            migrationBuilder.CreateIndex(
                name: "idx_reserva_paquete_reserva",
                table: "reservas_paquetes",
                column: "id_reserva");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservas_paquetes");
        }
    }
}
