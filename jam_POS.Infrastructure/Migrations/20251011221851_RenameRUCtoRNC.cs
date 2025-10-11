using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jam_POS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameRUCtoRNC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Empresas_RUC",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "RUC",
                table: "Empresas");

            migrationBuilder.AddColumn<string>(
                name: "RNC",
                table: "Empresas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_RNC",
                table: "Empresas",
                column: "RNC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Empresas_RNC",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "RNC",
                table: "Empresas");

            migrationBuilder.AddColumn<string>(
                name: "RUC",
                table: "Empresas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$q2iRAqq.iN8f/kx2VzmPnO9k6Yl3uqVGje4PfnktK/SCzTfzjUduG");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$oeOflqse0U8ZTFYD2tNvWuhpcdcE5wR6sWkX.ZCMiqoRUmuNTvM/y");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_RUC",
                table: "Empresas",
                column: "RUC",
                unique: true);
        }
    }
}
