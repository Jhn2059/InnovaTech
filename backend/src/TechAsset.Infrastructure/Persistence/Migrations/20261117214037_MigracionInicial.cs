using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechAsset.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoPatrimonial = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Marca = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NumeroSerie = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Ubicacion = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    FechaAdquisicion = table.Column<DateTime>(type: "date", nullable: true),
                    Especificaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    MotivoEstado = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movimientos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Entidad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EntidadId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    UsuarioNombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimientos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombres = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    HashContrasena = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FechaDeshabilitacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MotivoDeshabilitacion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    UltimoAcceso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prestamos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaPrestamo = table.Column<DateTime>(type: "date", nullable: false),
                    FechaDevolucion = table.Column<DateTime>(type: "date", nullable: true),
                    Observacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ObservacionDevolucion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponsableId = table.Column<Guid>(type: "uuid", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestamos", x => x.Id);
                    table.CheckConstraint("CK_Prestamos_FechaDevolucion", "\"FechaDevolucion\" IS NULL OR \"FechaDevolucion\" > \"FechaPrestamo\"");
                    table.ForeignKey(
                        name: "FK_Prestamos_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_Usuarios_ResponsableId",
                        column: x => x.ResponsableId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_Categoria",
                table: "Equipos",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_CodigoPatrimonial",
                table: "Equipos",
                column: "CodigoPatrimonial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_Estado",
                table: "Equipos",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_Fecha",
                table: "Movimientos",
                column: "Fecha",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_Tipo",
                table: "Movimientos",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_Equipo_Activo",
                table: "Prestamos",
                column: "EquipoId",
                unique: true,
                filter: "\"FechaDevolucion\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_Estado",
                table: "Prestamos",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_FechaPrestamo",
                table: "Prestamos",
                column: "FechaPrestamo");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_ResponsableId",
                table: "Prestamos",
                column: "ResponsableId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_Usuario",
                table: "Prestamos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Area",
                table: "Usuarios",
                column: "Area");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Rol",
                table: "Usuarios",
                column: "Rol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movimientos");

            migrationBuilder.DropTable(
                name: "Prestamos");

            migrationBuilder.DropTable(
                name: "Equipos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
