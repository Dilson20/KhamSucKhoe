using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VSP_HealthExam.Web.Migrations
{
    /// <inheritdoc />
    public partial class Add_New : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DanhSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DM_ToChucDonVi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenToChuc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenToChuc_ru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Id_Cha = table.Column<int>(type: "int", nullable: true),
                    ID_DonVi = table.Column<int>(type: "int", nullable: true),
                    Cap = table.Column<int>(type: "int", nullable: true),
                    TenTat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenTat_ru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ma = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ma_DV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STT = table.Column<int>(type: "int", nullable: true),
                    TamNgung = table.Column<bool>(type: "bit", nullable: true),
                    Khoi_id = table.Column<int>(type: "int", nullable: true),
                    Phong_id = table.Column<int>(type: "int", nullable: true),
                    Gian_id = table.Column<int>(type: "int", nullable: true),
                    To_id = table.Column<int>(type: "int", nullable: true),
                    Nhom_id = table.Column<int>(type: "int", nullable: true),
                    DepartmentTreeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DM_ToChucDonVi", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DM_ToChucDonVi_DM_ToChucDonVi_DepartmentTreeID",
                        column: x => x.DepartmentTreeID,
                        principalTable: "DM_ToChucDonVi",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Document_BHs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DanhSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateUpload = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenBenhVien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThongTin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HinhThucKCB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayBatDauKham = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThucKham = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoaiHinhDieuTri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DongYSuDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoteByDoctor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document_BHs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KSK_LoaiNhom",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNhom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoPhutThucHien = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KSK_LoaiNhom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "View_NhanVien_All",
                columns: table => new
                {
                    BoPhan_id = table.Column<int>(type: "int", nullable: false),
                    NhanVien_id = table.Column<int>(type: "int", nullable: true),
                    DanhSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoTen_ru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone_CQ = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChucDanh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChucDanh_ru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Loai_CBCNV = table.Column<int>(type: "int", nullable: true),
                    ID_DonVi = table.Column<int>(type: "int", nullable: true),
                    DonVi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonVi_ru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NhomNhanVien_id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChuyenDeNuRegister",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DanhSoCu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonViId = table.Column<int>(type: "int", nullable: false),
                    TrangThaiPheDuyet = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LyDoTuChoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuyenDeNuRegister", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChuyenDeNuRegister_DM_ToChucDonVi_DonViId",
                        column: x => x.DonViId,
                        principalTable: "DM_ToChucDonVi",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilesAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DanhSo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Document_BHId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilesAttachments_Document_BHs_Document_BHId",
                        column: x => x.Document_BHId,
                        principalTable: "Document_BHs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichKhamSucKhoe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonViId = table.Column<int>(type: "int", nullable: false),
                    ThoiGianKSK = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxSoLuong = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatePerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiNhom = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichKhamSucKhoe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichKhamSucKhoe_KSK_LoaiNhom_LoaiNhom",
                        column: x => x.LoaiNhom,
                        principalTable: "KSK_LoaiNhom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KSK_NhanVien_DangKy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonViId = table.Column<int>(type: "int", nullable: false),
                    DanhSo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LichKhamSucKhoeId = table.Column<int>(type: "int", nullable: false),
                    YeuCauDacBiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatePerson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KSK_NhanVien_DangKy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KSK_NhanVien_DangKy_LichKhamSucKhoe_LichKhamSucKhoeId",
                        column: x => x.LichKhamSucKhoeId,
                        principalTable: "LichKhamSucKhoe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenDeNuRegister_DonViId",
                table: "ChuyenDeNuRegister",
                column: "DonViId");

            migrationBuilder.CreateIndex(
                name: "IX_DM_ToChucDonVi_DepartmentTreeID",
                table: "DM_ToChucDonVi",
                column: "DepartmentTreeID");

            migrationBuilder.CreateIndex(
                name: "IX_FilesAttachments_Document_BHId",
                table: "FilesAttachments",
                column: "Document_BHId");

            migrationBuilder.CreateIndex(
                name: "IX_KSK_NhanVien_DangKy_LichKhamSucKhoeId",
                table: "KSK_NhanVien_DangKy",
                column: "LichKhamSucKhoeId");

            migrationBuilder.CreateIndex(
                name: "IX_LichKhamSucKhoe_LoaiNhom",
                table: "LichKhamSucKhoe",
                column: "LoaiNhom");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChuyenDeNuRegister");

            migrationBuilder.DropTable(
                name: "FilesAttachments");

            migrationBuilder.DropTable(
                name: "KSK_NhanVien_DangKy");

            migrationBuilder.DropTable(
                name: "View_NhanVien_All");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "DM_ToChucDonVi");

            migrationBuilder.DropTable(
                name: "Document_BHs");

            migrationBuilder.DropTable(
                name: "LichKhamSucKhoe");

            migrationBuilder.DropTable(
                name: "KSK_LoaiNhom");
        }
    }
}
