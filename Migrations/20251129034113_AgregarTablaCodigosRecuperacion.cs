using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2rismBeta.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaCodigosRecuperacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "codigos_recuperacion",
                columns: table => new
                {
                    id_codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    codigo = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_codigo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_generacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    fecha_expiracion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    usado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    fecha_uso = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ip_solicitud = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    intentos_validacion = table.Column<int>(type: "int", nullable: false),
                    bloqueado = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_codigos_recuperacion", x => x.id_codigo);
                    table.ForeignKey(
                        name: "fk_codigo_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "idx_codigo_expiracion",
                table: "codigos_recuperacion",
                column: "fecha_expiracion");

            migrationBuilder.CreateIndex(
                name: "idx_codigo_unique",
                table: "codigos_recuperacion",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_codigo_usuario",
                table: "codigos_recuperacion",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "idx_codigo_usuario_activo",
                table: "codigos_recuperacion",
                columns: new[] { "id_usuario", "usado", "bloqueado", "fecha_expiracion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "codigos_recuperacion");
        }
    }
}
