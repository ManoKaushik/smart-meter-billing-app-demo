using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartMeterWeb.Configs;
using SmartMeterWeb.Data.Context;
using SmartMeterWeb.Data.Entities;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models.AuthDto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using SmartMeterWeb.Exceptions;


namespace SmartMeterWeb.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailService _mailService;

        

        public AuthService(AppDbContext context, IConfiguration config, ILogService logger, IHttpContextAccessor httpContextAccessor, IMailService mailService)
        {
            _context = context;
            _config = config;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _mailService = mailService;
        }

        private bool IsUserAuthenticated()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Identity != null && user.Identity.IsAuthenticated;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            if(request.Role == "User")
            {
                if (IsUserAuthenticated())
                {
                    var checkRole = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
                    if (checkRole != "User")
                    {
                        throw new ApiException("Only Users can add new Users");
                    }
                    if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
                    {
                        throw new ApiException("UserName already taken");
                    }

                    var user = new User
                    {
                        UserName = request.UserName,
                        DisplayName = request.DisplayName,
                        Email = request.Email,
                        Phone = request.Phone,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123"),
                        IsActive = true
                    };

                    try
                    {
                        await _mailService.SendEmailAsync(
                            user.Email ?? "user.test@local.com", // fallback for testing
                            "User Registration Successful",
                            $@"
                        <h3>Hello {user.DisplayName},</h3>
                        <p>You are successfully registered as a Smart meter service provider.</p>
                        <p>
                            Your services are licensed and activated henceforth. You are expected to follow the Guidelines.
                        </p>
                        <p> Your default password is ""User123"". Please change your password after first login.
                        <p>Thank you,<br/>Smart Meter System</p>"
                        );
                    }
                    catch (Exception ex)
                    {
                        // _logger.LogError(ex, "Error sending email for bill {BillId}", bill.BillId);
                        throw new ApiException("User Registered but failed to send email notification.", 500);
                    }
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new ApiException("You need to be a User(logged in) to add new Users");
                }
                return new AuthResponseDto { Name = request.DisplayName, Role = request.Role };

            }
            else if(request.Role == "Consumer")
            {
                if(await _context.Consumers.AnyAsync(c => c.Email == request.Email))
                {
                    throw new Exception("Email already registered.");
                }

                var application = new Application
                {
                    Name = request.UserName,
                    Email = request.Email,
                    Phone = request.Phone
                };

                _context.Applications.Add(application);
                await _context.SaveChangesAsync();
                return new AuthResponseDto { Name = request.UserName, Role = request.Role, Message = "Please wait for approval" };
                //_context.Consumers.Add(consumer);
            }
            else
            {
                throw new Exception("invalid role");
            }
            
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (request.Role == "User")
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UsernameOrEmail);
                if (user == null)
                {
                    await _logger.LogLoginAttemptAsync(request.UsernameOrEmail, "User", false, "User not found", ipAddress);
                    throw new Exception("User not found");
                }
                else if (user.LoginLockEnd.HasValue && user.LoginLockEnd.Value > DateTime.UtcNow)
                {
                    var remaining = user.LoginLockEnd.Value - DateTime.UtcNow;
                    throw new Exception($"Account locked. Try again in {remaining.Minutes} minutes.");
                }
                else if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts >= AuthSettings.MaxAttempts)
                    {
                        user.LoginLockEnd = DateTime.UtcNow.Add(AuthSettings.Duration);
                        await _logger.LogLoginAttemptAsync(request.UsernameOrEmail, "User", false, "Wrong Password", ipAddress);
                        await _context.SaveChangesAsync();
                        throw new Exception("Invalid Password. Account locked due to too many failed attempts");
                    }
                    else
                    {
                        await _logger.LogLoginAttemptAsync(request.UsernameOrEmail, "User", false, "Wrong Password", ipAddress);
                        throw new Exception("Wrong Password");
                    }
                }

                await _logger.LogLoginAttemptAsync(request.UsernameOrEmail, "User", true, "Login successful", ipAddress);
                var token = GenerateJwtToken(user.UserName, "User");
                user.LastLoginUtc = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return new AuthResponseDto { Name = user.UserName, Role = "User", Token = token };
            }
            else if (request.Role == "Consumer")
            {
                var user = await _context.Consumers.FirstOrDefaultAsync(u => u.Email == request.UsernameOrEmail);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    await _logger.LogLoginAttemptAsync(request.UsernameOrEmail, "Consumer", false, "Invalid Credentials", ipAddress);
                    throw new Exception("Invalid credentials");
                }
                else if (user.LoginLockEnd.HasValue && user.LoginLockEnd.Value > DateTime.UtcNow)
                {
                    var remaining = user.LoginLockEnd.Value - DateTime.UtcNow;
                    throw new Exception($"Account locked. Try again in {remaining.Minutes} minutes.");
                }

                else if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts >= AuthSettings.MaxAttempts)
                    {
                        user.LoginLockEnd = DateTime.UtcNow.Add(AuthSettings.Duration);
                        await _logger.LogLoginAttemptAsync(request.UsernameOrEmail, "Consumer", false, "Wrong Password", ipAddress);
                        await _context.SaveChangesAsync();
                        throw new Exception("Invalid Password. Account locked due to too many failed attempts");
                    }
                    else
                    {
                        await _logger.LogLoginAttemptAsync(request.UsernameOrEmail, "Consumer", false, "Wrong Password", ipAddress);
                        throw new Exception("Wrong Password");
                    }
                }

                await _logger.LogLoginAttemptAsync(request.UsernameOrEmail, "Consumer", true, "Login successful", ipAddress);
                var token = GenerateJwtToken(user.Name, "User");
                await _context.SaveChangesAsync();
                return new AuthResponseDto { Name = user.Name, Role = "Consumer", Token = token };
            }
            else
            {
                throw new Exception("Invalid Role");
            }
        }

            private string GenerateJwtToken(string username, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ActionResult> UpdatePassWord(PasswordUpdateDto request)
        {
            if(request.role == "User")
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.nameOrEmail);
                if(user == null)
                {
                    throw new Exception("User not found");
                }

                else if (!BCrypt.Net.BCrypt.Verify(request.oldpassword, user.PasswordHash))
                {
                    throw new Exception("Enter the correct old password");
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.newpassword);
                await _context.SaveChangesAsync();
                return new OkObjectResult("password changed successfully");
            }
            else if(request.role =="Consumer")
            {
                var con = await _context.Consumers.FirstOrDefaultAsync(u => u.Email == request.nameOrEmail);
                if (con == null)
                {
                    throw new Exception("User not found");
                }

                else if (!BCrypt.Net.BCrypt.Verify(request.oldpassword, con.PasswordHash))
                {
                    throw new Exception("Enter the correct old password");
                }

                con.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.newpassword);
                await _context.SaveChangesAsync();
                return new OkObjectResult("password changed successfully");
            }
            else
            {
                throw new Exception("wrong role specified");
            }
        }
        
    }
}
