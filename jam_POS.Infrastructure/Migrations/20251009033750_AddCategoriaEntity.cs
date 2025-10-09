using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace jam_POS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Productos_Categoria",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Productos");

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Productos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Icono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Activo", "Color", "CreatedAt", "Descripcion", "Icono", "Nombre", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, true, "#3B82F6", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bebidas y refrescos", "local_drink", "Bebidas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, true, "#10B981", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Alimentos y comestibles", "restaurant", "Alimentos", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, true, "#8B5CF6", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Productos electrónicos y tecnología", "devices", "Electrónica", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, true, "#F59E0B", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Productos de limpieza e higiene", "cleaning_services", "Limpieza", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, true, "#EC4899", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ropa y accesorios", "checkroom", "Ropa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$5J6uCDIlHgZQ50Az/iXqGu/7TZHFgeFiuHCmOk7HBoc9Po4rPovTy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$6eSZe1lk59AyUK5WTT6VdOJShimnMvyduI8/S6o5tR77KPbmhXx5a");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Activo",
                table: "Categorias",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre",
                table: "Categorias",
                column: "Nombre");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Categorias_CategoriaId",
                table: "Productos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Categorias_CategoriaId",
                table: "Productos");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Productos");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Productos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$JatgcDzr/hOx6tDP4.E5CewhAj8g1k5Vfk0imF4twYMzkIR/iOhcO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$uDIpC0XOP9nQV1aT8.kJnOtizqJaN.3e7KYYfsja2guQEVspaRTU2");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Categoria",
                table: "Productos",
                column: "Categoria");
        }
    }
}
