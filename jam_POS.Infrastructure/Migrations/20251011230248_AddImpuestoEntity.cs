using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace jam_POS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImpuestoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Impuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Porcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "PORCENTUAL"),
                    MontoFijo = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    AplicaPorDefecto = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Impuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Impuestos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$RO.XgaeqNhtc0acHac6EoOII4r/tBSwEl/5MNmPHeXd.2gKefywNa");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$8FTf2TaMXEzWHG.eTVJCMeQ58nRg28Wt9HEZG41Yrc2osmcoMnJdC");

            migrationBuilder.CreateIndex(
                name: "IX_Impuestos_Activo",
                table: "Impuestos",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Impuestos_AplicaPorDefecto",
                table: "Impuestos",
                column: "AplicaPorDefecto");

            migrationBuilder.CreateIndex(
                name: "IX_Impuestos_EmpresaId",
                table: "Impuestos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Impuestos_Nombre",
                table: "Impuestos",
                column: "Nombre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Impuestos");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$7cfSAsKKN57V3TT1np3Sze4kdy.vFzIvdloLIYcNbRREX/FeczMgO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$Rt4faDjitVumppza5BR1ZujXxG0NK4oHyecEWTfQqrnWM1U6JeZ4C");
        }
    }
}
