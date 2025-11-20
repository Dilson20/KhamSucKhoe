
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using VSP_HealthExam.Web.ViewModels;
namespace VSP_HealthExam.Web.Services
{
	public interface IMailServices
	{
		Task SendMailAsync(string toEmail, string gioiTinh, string hoTen, string danhSo, string donVi, int lichKhamSucKhoeId, DateTime thoiGian, string tenLoaiNhom, string ghiChu = "");
	}

	public class MailServices : IMailServices
	{
		private readonly MailSettings _mailSettings;
		private readonly ILogger<MailServices> _logger;
		
		public MailServices(IOptions<MailSettings> mailSettings, ILogger<MailServices> logger)
		{
			_mailSettings = mailSettings.Value;
			_logger = logger;
		}
		
		public async Task SendMailAsync(string toEmail, string gioiTinh, string hoTen, string danhSo, string donVi, int lichKhamSucKhoeId, DateTime thoiGian, string tenLoaiNhom, string ghiChu = "")
		{
			if (_mailSettings.Active)
			{
				_logger.LogInformation("Bắt đầu gửi email đến: {toEmail}", toEmail);
				_logger.LogInformation("Cấu hình SMTP: Host={Host}, Port={Port}, SSL={SSL}, Email={Email}", 
					_mailSettings.Host, _mailSettings.Port, _mailSettings.SSL, _mailSettings.Email);
				
				var email = new MimeMessage();
				email.From.Add(MailboxAddress.Parse(_mailSettings.Email));
				email.To.Add(MailboxAddress.Parse(toEmail));

				email.Subject = "Thông báo đăng ký khám sức khỏe thành công";
				var builder = new BodyBuilder();
				string xungHo = gioiTinh == "Nữ" ? "Bà" : "Ông";
				string ghiChuHtml = !string.IsNullOrEmpty(ghiChu) ? $@"<p><b>Ghi chú của TTYT:</b> {ghiChu}</p>" : "";
				builder.HtmlBody = $@"<p>{xungHo} <b>{hoTen}</b> (Danh số: <b>{danhSo}</b>, Đơn vị: <b>{donVi}</b>)</p>
						<p>Đã đăng ký thành công <b>{tenLoaiNhom}</b>.</p>
						<p><b>Mã lịch khám:</b> {lichKhamSucKhoeId}</p>
						<p><b>Thời gian khám:</b> {thoiGian:dd/MM/yyyy HH:mm}</p>
						{ghiChuHtml}
						<p>Trân trọng!</p>";

				email.Body = builder.ToMessageBody();
				
				//send email
				using var smtp = new SmtpClient();
				try
				{
					_logger.LogInformation("Đang kết nối SMTP...");
					smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // Bypass certificate validation
					await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, _mailSettings.SSL ? SecureSocketOptions.StartTlsWhenAvailable : SecureSocketOptions.None);
					_logger.LogInformation("Kết nối SMTP thành công");
					
					if (!string.IsNullOrEmpty(_mailSettings.Password))
					{
						_logger.LogInformation("Đang xác thực SMTP...");
						await smtp.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
						_logger.LogInformation("Xác thực SMTP thành công");
					}
					else
					{
						_logger.LogInformation("Bỏ qua xác thực SMTP (không có password)");
					}
					
					_logger.LogInformation("Đang gửi email...");
					await smtp.SendAsync(email);
					_logger.LogInformation("Gửi email thành công đến: {toEmail}", toEmail);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Lỗi khi gửi email đến {toEmail}: {Message}", toEmail, ex.Message);
					Console.WriteLine($"Lỗi gửi email: {ex.Message}");
					throw; // Re-throw để controller có thể xử lý
				}
				finally
				{
					smtp.Disconnect(true);
					smtp.Dispose();
					_logger.LogInformation("Đã ngắt kết nối SMTP");
				}
			}
			else
			{
				_logger.LogWarning("Email service không được kích hoạt");
			}
		}
	}
}
