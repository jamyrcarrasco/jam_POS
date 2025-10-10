using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace jam_POS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpresaMultiTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Productos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Categorias",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NombreComercial = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RUC = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Pais = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Ciudad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CodigoPostal = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Plan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "BASICO"),
                    FechaVencimientoPlan = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "EmpresaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "EmpresaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "EmpresaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "EmpresaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "EmpresaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EmpresaId", "PasswordHash" },
                values: new object[] { null, "$2a$11$IKwRT4JxKf0lhKC7bTzq6eb7bRjMAOTdg0SP3RlwpSDXAW7uTeQO2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EmpresaId", "PasswordHash" },
                values: new object[] { null, "$2a$11$XHFe.kT1wi1GQEmcI.Z1.eqLmZ4Q9c6R6sthRoQjtoQCiiZUqrFI6" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmpresaId",
                table: "Users",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_EmpresaId",
                table: "Productos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_EmpresaId",
                table: "Categorias",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Activo",
                table: "Empresas",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_RUC",
                table: "Empresas",
                column: "RUC",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Empresas_EmpresaId",
                table: "Categorias",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Empresas_EmpresaId",
                table: "Productos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Empresas_EmpresaId",
                table: "Users",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Empresas_EmpresaId",
                table: "Categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Empresas_EmpresaId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Empresas_EmpresaId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropIndex(
                name: "IX_Users_EmpresaId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Productos_EmpresaId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_EmpresaId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Categorias");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$uAFVsuH6DAGm1pPN0OrKTe5AXO1jfhRq9Qo8KvhMOwc/WoioESGMu");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$7ufCCV.s2T3H3lQPP5Pbze0zroGfRa3YoxAzu83YQuacIrLLIZap6");
        }
    }
}
