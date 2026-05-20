using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaBodega.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEntidadesSistemaCafe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admin_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    rol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admin_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    contacto = table.Column<string>(type: "text", nullable: true),
                    telefono = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clientes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "secciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_secciones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tipos_cafe",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tipos_cafe", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "auditorias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    admin_user_id = table.Column<int>(type: "integer", nullable: false),
                    accion = table.Column<string>(type: "text", nullable: false),
                    entidad_afectada = table.Column<string>(type: "text", nullable: false),
                    entidad_id = table.Column<int>(type: "integer", nullable: true),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    detalles = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auditorias", x => x.id);
                    table.ForeignKey(
                        name: "fk_auditorias_admin_users_admin_user_id",
                        column: x => x.admin_user_id,
                        principalTable: "admin_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "inventarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    seccion_id = table.Column<int>(type: "integer", nullable: false),
                    tipo_cafe_id = table.Column<int>(type: "integer", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventarios", x => x.id);
                    table.ForeignKey(
                        name: "fk_inventarios_secciones_seccion_id",
                        column: x => x.seccion_id,
                        principalTable: "secciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_inventarios_tipos_cafe_tipo_cafe_id",
                        column: x => x.tipo_cafe_id,
                        principalTable: "tipos_cafe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movimientos_entrada",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cliente_id = table.Column<int>(type: "integer", nullable: false),
                    seccion_id = table.Column<int>(type: "integer", nullable: false),
                    tipo_cafe_id = table.Column<int>(type: "integer", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    admin_user_id = table.Column<int>(type: "integer", nullable: false),
                    notas = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movimientos_entrada", x => x.id);
                    table.ForeignKey(
                        name: "fk_movimientos_entrada_admin_users_admin_user_id",
                        column: x => x.admin_user_id,
                        principalTable: "admin_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_movimientos_entrada_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_movimientos_entrada_secciones_seccion_id",
                        column: x => x.seccion_id,
                        principalTable: "secciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_movimientos_entrada_tipos_cafe_tipo_cafe_id",
                        column: x => x.tipo_cafe_id,
                        principalTable: "tipos_cafe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movimientos_salida",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cliente_id = table.Column<int>(type: "integer", nullable: false),
                    seccion_id = table.Column<int>(type: "integer", nullable: false),
                    tipo_cafe_id = table.Column<int>(type: "integer", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    admin_user_id = table.Column<int>(type: "integer", nullable: false),
                    notas = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movimientos_salida", x => x.id);
                    table.ForeignKey(
                        name: "fk_movimientos_salida_admin_users_admin_user_id",
                        column: x => x.admin_user_id,
                        principalTable: "admin_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_movimientos_salida_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_movimientos_salida_secciones_seccion_id",
                        column: x => x.seccion_id,
                        principalTable: "secciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_movimientos_salida_tipos_cafe_tipo_cafe_id",
                        column: x => x.tipo_cafe_id,
                        principalTable: "tipos_cafe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_username",
                table: "admin_users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_auditorias_admin_user_id",
                table: "auditorias",
                column: "admin_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_clientes_nombre",
                table: "clientes",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inventarios_seccion_id_tipo_cafe_id",
                table: "inventarios",
                columns: new[] { "seccion_id", "tipo_cafe_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inventarios_tipo_cafe_id",
                table: "inventarios",
                column: "tipo_cafe_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_entrada_admin_user_id",
                table: "movimientos_entrada",
                column: "admin_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_entrada_cliente_id",
                table: "movimientos_entrada",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_entrada_seccion_id",
                table: "movimientos_entrada",
                column: "seccion_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_entrada_tipo_cafe_id",
                table: "movimientos_entrada",
                column: "tipo_cafe_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_salida_admin_user_id",
                table: "movimientos_salida",
                column: "admin_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_salida_cliente_id",
                table: "movimientos_salida",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_salida_seccion_id",
                table: "movimientos_salida",
                column: "seccion_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_salida_tipo_cafe_id",
                table: "movimientos_salida",
                column: "tipo_cafe_id");

            migrationBuilder.CreateIndex(
                name: "ix_secciones_nombre",
                table: "secciones",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tipos_cafe_nombre",
                table: "tipos_cafe",
                column: "nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditorias");

            migrationBuilder.DropTable(
                name: "inventarios");

            migrationBuilder.DropTable(
                name: "movimientos_entrada");

            migrationBuilder.DropTable(
                name: "movimientos_salida");

            migrationBuilder.DropTable(
                name: "admin_users");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "secciones");

            migrationBuilder.DropTable(
                name: "tipos_cafe");
        }
    }
}
