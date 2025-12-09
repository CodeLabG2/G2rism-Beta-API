using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2rismBeta.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarReservasHoteles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reservas_hoteles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_reserva = table.Column<int>(type: "int", nullable: false),
                    id_hotel = table.Column<int>(type: "int", nullable: false),
                    fecha_checkin = table.Column<DateTime>(type: "DATE", nullable: false),
                    fecha_checkout = table.Column<DateTime>(type: "DATE", nullable: false),
                    numero_habitaciones = table.Column<int>(type: "int", nullable: false),
                    tipo_habitacion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero_huespedes = table.Column<int>(type: "int", nullable: false),
                    precio_por_noche = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservas_hoteles", x => x.id);
                    table.ForeignKey(
                        name: "fk_reserva_hotel_hotel",
                        column: x => x.id_hotel,
                        principalTable: "hoteles",
                        principalColumn: "id_hotel",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reserva_hotel_reserva",
                        column: x => x.id_reserva,
                        principalTable: "reservas",
                        principalColumn: "id_reserva",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "idx_reserva_hotel_checkin",
                table: "reservas_hoteles",
                column: "fecha_checkin");

            migrationBuilder.CreateIndex(
                name: "idx_reserva_hotel_fechas",
                table: "reservas_hoteles",
                columns: new[] { "id_hotel", "fecha_checkin", "fecha_checkout" });

            migrationBuilder.CreateIndex(
                name: "idx_reserva_hotel_hotel",
                table: "reservas_hoteles",
                column: "id_hotel");

            migrationBuilder.CreateIndex(
                name: "idx_reserva_hotel_reserva",
                table: "reservas_hoteles",
                column: "id_reserva");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservas_hoteles");
        }
    }
}
