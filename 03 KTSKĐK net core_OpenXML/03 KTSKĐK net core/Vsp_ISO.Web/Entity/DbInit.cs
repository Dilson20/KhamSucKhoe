using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VSP_HealthExam.Web.Models.KhamSucKhoe;
using VSP_HealthExam.Web.Models.Register;

namespace VSP_HealthExam.Web.Entity
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Tạo roles nếu chưa có
            var roles = new[] { "Admin", "Medic", "User"};
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            //// Tạo admin user nếu chưa có
            //await CreateUserIfNotExists(userManager, context, "admin", "admin@vietsov.com.vn", "admin", "Administrator", "Admin@123", "Admin");

            //// Tạo medic user nếu chưa có
            //await CreateUserIfNotExists(userManager, context, "medic", "medic@vietsov.com.vn", "medic", "Medic User", "123", "Medic");

            //// Tạo managermedic user nếu chưa có
            //await CreateUserIfNotExists(userManager, context, "managermedic", "managermedic@vietsov.com.vn", "managermedic", "Manager Medic User", "123", "ManagerMedic");
            //// Tạo managermedic user nếu chưa có
            await CreateUserIfNotExists(userManager, context, "admin2", "admin2@vietsov.com.vn", "44444", "Administrator", "123", "Admin");
            await CreateKSKLoaiNhomIfNotExists(context);
        }

        private static async Task CreateUserIfNotExists(UserManager<AppUser> userManager, ApplicationDbContext context,
            string userName, string email, string danhSo, string fullName, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = userName,
                    Email = email,
                    DanhSo = danhSo,
                    FullName = fullName,
                    //UserType = "Internal",
                    //CreatedDate = DateTime.Now,
                    //IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Gán role
                    var roleResult = await userManager.AddToRoleAsync(user, role);
                    //if (!roleResult.Succeeded)
                    //{
                    //    // Log lỗi gán role
                    //    Console.WriteLine($"Lỗi gán role {role} cho user {userName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    //}

                    //// Tạo UserRoleManagement record
                    //var userRole = new UserRoleManagement
                    //{
                    //    UserId = user.Id,
                    //    Role = role,
                    //    CreatedDate = DateTime.Now,
                    //    IsActive = true
                    //};

                    //context.UserRoleManagements.Add(userRole);
                    await context.SaveChangesAsync();

                    Console.WriteLine($"Đã tạo user {userName} với role {role}");
                }
                else
                {
                    // Log lỗi tạo user
                    Console.WriteLine($"Lỗi tạo user {userName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine($"User {userName} đã tồn tại");
            }
        }

        private static async Task CreateKSKLoaiNhomIfNotExists(ApplicationDbContext context)
        {
            if (!context.KSK_LoaiNhom.Any())
            {
                var loaiNhoms = new List<KSK_LoaiNhom>
                {
                    new KSK_LoaiNhom { TenNhom = "Lấy mẫu máu định kỳ", Color = "#6aa84f", SoPhutThucHien = 1},
                    new KSK_LoaiNhom { TenNhom = "Khám Lãnh đạo", Color = "#f1c232", SoPhutThucHien = 1 },
                    new KSK_LoaiNhom { TenNhom = "Khám chuyên đề nữ", Color = "#4a86e8", SoPhutThucHien = 1 },
                    new KSK_LoaiNhom { TenNhom = "Khám chuyên đề tim mạch", Color = "#e06666", SoPhutThucHien = 1 },
                    new KSK_LoaiNhom { TenNhom = "Tiêm vaccine", Color = "#FF6347", SoPhutThucHien = 1},
                    new KSK_LoaiNhom { TenNhom = "Khám sức khỏe định kỳ", Color = "#2E7D32", SoPhutThucHien = 1},
                    new KSK_LoaiNhom { TenNhom = "Khám bệnh nghề nghiệp", Color = "#1f1e33", SoPhutThucHien = 1}
                };
                context.KSK_LoaiNhom.AddRange(loaiNhoms);
                await context.SaveChangesAsync();
            }
        }
    }
}