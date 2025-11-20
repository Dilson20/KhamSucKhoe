using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VSP_HealthExam.Web.Migrations
{
    /// <inheritdoc />
    public partial class Add_KhamSucKhoe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KhamNgheNghiep",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DanhSo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhamNgheNghiep", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KhamNgheNghiep_CCCD",
                table: "KhamNgheNghiep",
                column: "CCCD",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhamNgheNghiep_DanhSo",
                table: "KhamNgheNghiep",
                column: "DanhSo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KhamNgheNghiep");
        }
    }
}
