using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2rismBeta.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRelacionReservasServicios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reservas_servicios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_reserva = table.Column<int>(type: "int", nullable: false),
                    id_servicio = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    fecha_servicio = table.Column<DateTime>(type: "DATE", nullable: true),
                    hora_servicio = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    precio_unitario = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_agregado = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservas_servicios", x => x.id);
                    table.ForeignKey(
                        name: "FK_reservas_servicios_reservas_id_reserva",
                        column: x => x.id_reserva,
                        principalTable: "reservas",
                        principalColumn: "id_reserva",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservas_servicios_servicios_adicionales_id_servicio",
                        column: x => x.id_servicio,
                        principalTable: "servicios_adicionales",
                        principalColumn: "id_servicio",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "idx_reservaservicio_reserva",
                table: "reservas_servicios",
                column: "id_reserva");

            migrationBuilder.CreateIndex(
                name: "idx_reservaservicio_servicio",
                table: "reservas_servicios",
                column: "id_servicio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservas_servicios");
        }
    }
}
