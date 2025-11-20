using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using VSP_HealthExam.Web.Models;
using VSP_HealthExam.Web.Models.BaoHiem;
using VSP_HealthExam.Web.Models.KhamNgheNghiep;
using VSP_HealthExam.Web.Models.KhamSucKhoe;
using VSP_HealthExam.Web.Models.Register;

namespace VSP_HealthExam.Web.Entity
{
	public class ApplicationDbContext : IdentityDbContext<AppUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
		{

		}
        protected ApplicationDbContext(DbContextOptions option) : base(option)
        {

        }    		
        // Đã xóa các DbSet và phương thức liên quan đến các model/view/service không còn sử dụng
        public virtual DbSet<Document_BH> Document_BHs { get; set; }
        public virtual DbSet<FilesAttachment> FilesAttachments { get; set; }
        public virtual DbSet<KSK_LoaiNhom> KSK_LoaiNhom { get; set; }
        public virtual DbSet<LichKhamSucKhoe> LichKhamSucKhoe { get; set; }
        public virtual DbSet<KSK_NhanVien_DangKy> KSK_NhanVien_DangKy { get; set; }
        public virtual DbSet<ChuyenDeNuRegister> ChuyenDeNuRegister { get; set; }
        public virtual DbSet<View_NhanVien_All> View_NhanVien_All { get; set; }
        public virtual DbSet<KhamNgheNghiep> KhamNgheNghiep { get; set; }


        // Đã xóa các phương thức liên quan đến các model/view/service không còn sử dụng
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                        warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
            builder.Entity<View_NhanVien_All>().HasNoKey();

            builder.Entity<Document_BH>().HasKey(x => x.Id);
            builder.Entity<FilesAttachment>().HasKey(x => x.Id);
            builder.Entity<Document_BH>()
                .HasMany(x => x.FilesAttachments)
                .WithOne(a => a.Document_BH)
                .HasForeignKey(k => k.Document_BHId);
            builder.Entity<KSK_LoaiNhom>()
               .HasMany(x => x.LichKhamSucKhoes)
               .WithOne(x => x.LoaiNhomNavigation)
               .HasForeignKey(x => x.LoaiNhom);
            builder.Entity<LichKhamSucKhoe>()
                .HasMany(x => x.KSK_NhanVien_DangKys)
                .WithOne(x => x.LichKhamSucKhoe)
                .HasForeignKey(x => x.LichKhamSucKhoeId);
        }
      
    }
    public class SubDbContext : DbContext
    {
        public virtual DbSet<View_DanhBa_NhanVien> View_DanhBa_NhanVien { get; set; }
        public virtual DbSet<DepartmentTree> DepartmentTree { get; set; }
        public SubDbContext(DbContextOptions<SubDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DepartmentTree>().HasNoKey();
            modelBuilder.Entity<View_DanhBa_NhanVien>().HasNoKey();

        }

    }
}
