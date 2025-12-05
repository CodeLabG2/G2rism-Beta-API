using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2rismBeta.API.Migrations
{
    /// <inheritdoc />
    public partial class CompletarModuloVuelos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_vuelos_id_aerolinea",
                table: "vuelos",
                newName: "idx_vuelo_aerolinea");

            migrationBuilder.AddColumn<int>(
                name: "cupos_disponibles",
                table: "vuelos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "cupos_totales",
                table: "vuelos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "destino",
                table: "vuelos",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "duracion_minutos",
                table: "vuelos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "escalas",
                table: "vuelos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "estado",
                table: "vuelos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_llegada",
                table: "vuelos",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_salida",
                table: "vuelos",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "hora_llegada",
                table: "vuelos",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "hora_salida",
                table: "vuelos",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "id_proveedor",
                table: "vuelos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "numero_vuelo",
                table: "vuelos",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "observaciones",
                table: "vuelos",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "origen",
                table: "vuelos",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "precio_economica",
                table: "vuelos",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "precio_ejecutiva",
                table: "vuelos",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_vuelo_estado",
                table: "vuelos",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "idx_vuelo_fecha_salida",
                table: "vuelos",
                column: "fecha_salida");

            migrationBuilder.CreateIndex(
                name: "idx_vuelo_numero_unique",
                table: "vuelos",
                column: "numero_vuelo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_vuelo_origen_destino",
                table: "vuelos",
                columns: new[] { "origen", "destino" });

            migrationBuilder.CreateIndex(
                name: "idx_vuelo_proveedor",
                table: "vuelos",
                column: "id_proveedor");

            migrationBuilder.AddForeignKey(
                name: "fk_vuelo_proveedor",
                table: "vuelos",
                column: "id_proveedor",
                principalTable: "proveedores",
                principalColumn: "id_proveedor",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vuelo_proveedor",
                table: "vuelos");

            migrationBuilder.DropIndex(
                name: "idx_vuelo_estado",
                table: "vuelos");

            migrationBuilder.DropIndex(
                name: "idx_vuelo_fecha_salida",
                table: "vuelos");

            migrationBuilder.DropIndex(
                name: "idx_vuelo_numero_unique",
                table: "vuelos");

            migrationBuilder.DropIndex(
                name: "idx_vuelo_origen_destino",
                table: "vuelos");

            migrationBuilder.DropIndex(
                name: "idx_vuelo_proveedor",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "cupos_disponibles",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "cupos_totales",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "destino",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "duracion_minutos",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "escalas",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "estado",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "fecha_llegada",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "fecha_salida",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "hora_llegada",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "hora_salida",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "id_proveedor",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "numero_vuelo",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "observaciones",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "origen",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "precio_economica",
                table: "vuelos");

            migrationBuilder.DropColumn(
                name: "precio_ejecutiva",
                table: "vuelos");

            migrationBuilder.RenameIndex(
                name: "idx_vuelo_aerolinea",
                table: "vuelos",
                newName: "IX_vuelos_id_aerolinea");
        }
    }
}
