using CRUD_Operation.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;

namespace CRUD_Operation.Controllers
{
    public class UserController : Controller
    {
        private myContext context;

        public UserController(myContext mycontext)
        {
            context = mycontext;
        }
        

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Your@gmail.com", "Your App Password"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("Your@gmail.com", "Your App Password");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SignUp() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(string txtmail, string txtname, string txtpwd)
        {
            var existingUser = context.tbl_user.FirstOrDefault(u => u.u_email == txtmail);
            if (existingUser != null)
            {
                ViewBag.Error = "Email already registered.";
                return View();
            }

            // Generate 6-digit OTP
            string otp = new Random().Next(100000, 999999).ToString();

            var user = new User
            {
                u_email = txtmail,
                u_name = txtname, 
                u_password = txtpwd,
                EmailOTP = otp,
                IsEmailVerified = false
            };

            context.tbl_user.Add(user);
            context.SaveChangesAsync();

            string subject = "🔐 Verify Your Email - OTP Inside (Action Required)";
            string body = $@"
            <div style='font-family:Segoe UI, sans-serif; color:#001f1f; background-color:#f9f9f9; padding:30px; border-radius:10px; border:1px solid #00b3b3; max-width:500px; margin:auto;'>
            <h2 style='color:#00b3b3;'>Email Verification Required</h2>
            <p>Dear User,</p>
            <p>Thank you for signing up. To complete your registration, please enter the following One-Time Password (OTP) in the verification page:</p>
    
            <div style='text-align:center; margin:20px 0;'>
                <span style='display:inline-block; background-color:#00b3b3; color:#ffffff; font-size:24px; padding:10px 30px; border-radius:8px; letter-spacing:5px;'>
                    {otp}
                </span>
            </div>

            <p>This OTP is valid for <strong>5 minutes</strong>. If you didn’t request this, please ignore this email.</p>

                <p style='margin-top:30px;'>Regards,<br><strong>Your App Team</strong></p>
            </div>";


            SendEmailAsync(txtmail, subject, body);

            TempData["Email"] = txtmail;
            return RedirectToAction("VerifyOtp");
        }
        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOtp(string otp)
        {
            var email = TempData["Email"] as string;
            var user = context.tbl_user.FirstOrDefault(u => u.u_email == email);

            if (user != null && user.EmailOTP == otp)
            {
                user.IsEmailVerified = true;
                user.EmailOTP = "Verify";
                context.SaveChanges();
                return Content("✅ Email Verified Successfully!");
            }

            ViewBag.Error = "❌ Invalid OTP!";
            return View();
        }
    }
}
