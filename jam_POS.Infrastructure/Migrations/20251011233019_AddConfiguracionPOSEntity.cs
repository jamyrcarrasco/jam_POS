using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace jam_POS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracionPOSEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionesPOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrefijoRecibo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PrefijoFactura = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SiguienteNumeroRecibo = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    SiguienteNumeroFactura = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    MensajePieRecibo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IncluirLogoRecibo = table.Column<bool>(type: "boolean", nullable: false),
                    ImpuestoPorDefectoId = table.Column<int>(type: "integer", nullable: true),
                    PermitirDescuentos = table.Column<bool>(type: "boolean", nullable: false),
                    PermitirDevoluciones = table.Column<bool>(type: "boolean", nullable: false),
                    TiempoLimiteAnulacionMinutos = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
                    DescuentoMaximoPorcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 100m),
                    MonedaPorDefecto = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "DOP"),
                    SimboloMoneda = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "$"),
                    DecimalesMoneda = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    FormatoPapel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "TERMICO_58"),
                    ImprimirAutomaticamente = table.Column<bool>(type: "boolean", nullable: false),
                    ImprimirCopiaCliente = table.Column<bool>(type: "boolean", nullable: false),
                    EfectivoHabilitado = table.Column<bool>(type: "boolean", nullable: false),
                    TarjetaHabilitado = table.Column<bool>(type: "boolean", nullable: false),
                    TransferenciaHabilitado = table.Column<bool>(type: "boolean", nullable: false),
                    CreditoHabilitado = table.Column<bool>(type: "boolean", nullable: false),
                    ModoOfflineHabilitado = table.Column<bool>(type: "boolean", nullable: false),
                    IntervaloSincronizacionMinutos = table.Column<int>(type: "integer", nullable: false, defaultValue: 15),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesPOS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionesPOS_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfiguracionesPOS_Impuestos_ImpuestoPorDefectoId",
                        column: x => x.ImpuestoPorDefectoId,
                        principalTable: "Impuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$5fWtFVOh12gx86fQfcBOOet8VZYWvMYRv69Xd1L5tK.TZ1V66qer6");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$31wUTXxAMstpUc/XTY07oeBOgMiWITR692wn.vcJ4wCa2PLsHALvq");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesPOS_EmpresaId",
                table: "ConfiguracionesPOS",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesPOS_ImpuestoPorDefectoId",
                table: "ConfiguracionesPOS",
                column: "ImpuestoPorDefectoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesPOS");

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
        }
    }
}
