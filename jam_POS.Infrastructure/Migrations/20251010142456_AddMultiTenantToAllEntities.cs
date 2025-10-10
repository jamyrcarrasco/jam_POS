using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jam_POS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenantToAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                table: "Roles");

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Roles",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "EmpresaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "EmpresaId",
                value: null);

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
                name: "IX_Roles_EmpresaId",
                table: "Roles",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Empresas_EmpresaId",
                table: "Roles",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Empresas_EmpresaId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_EmpresaId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Roles");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$IKwRT4JxKf0lhKC7bTzq6eb7bRjMAOTdg0SP3RlwpSDXAW7uTeQO2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$XHFe.kT1wi1GQEmcI.Z1.eqLmZ4Q9c6R6sthRoQjtoQCiiZUqrFI6");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);
        }
    }
}
