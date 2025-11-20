using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VSP_HealthExam.Web.Migrations
{
    /// <inheritdoc />
    public partial class Edit_LichKham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxSoLuong",
                table: "LichKhamSucKhoe");

            migrationBuilder.DropColumn(
                name: "UpdateDateTime",
                table: "LichKhamSucKhoe");

            migrationBuilder.RenameColumn(
                name: "ThoiGianKSK",
                table: "LichKhamSucKhoe",
                newName: "ThoiGianKetThuc");

            migrationBuilder.RenameColumn(
                name: "CreateDateTime",
                table: "LichKhamSucKhoe",
                newName: "ThoiGianBatDau");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThoiGianKetThuc",
                table: "LichKhamSucKhoe",
                newName: "ThoiGianKSK");

            migrationBuilder.RenameColumn(
                name: "ThoiGianBatDau",
                table: "LichKhamSucKhoe",
                newName: "CreateDateTime");

            migrationBuilder.AddColumn<int>(
                name: "MaxSoLuong",
                table: "LichKhamSucKhoe",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDateTime",
                table: "LichKhamSucKhoe",
                type: "datetime2",
                nullable: true);
        }
    }
}
