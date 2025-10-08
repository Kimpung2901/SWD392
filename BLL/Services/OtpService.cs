using System;
using Microsoft.Extensions.Caching.Memory;
using BLL.IService; // ✅ FIX: Dùng đúng namespace của IEmailSender

namespace BLL.Services
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly IEmailSender _email;
        private const int ExpireMinutes = 5;
        private const int MaxAttempts = 5;

        public OtpService(IMemoryCache cache, IEmailSender email)
        {
            _cache = cache; 
            _email = email;
        }

        private static string Key(string email) => $"otp:{email.Trim().ToLowerInvariant()}";

        private record OtpState(string Code, int Attempts, DateTime ExpireAt);

        public async Task SendOtpAsync(string email)
        {
            var rnd = new Random();
            var code = rnd.Next(100000, 999999).ToString(); // OTP 6 số

            var state = new OtpState(code, 0, DateTime.UtcNow.AddMinutes(ExpireMinutes));
            _cache.Set(Key(email), state, TimeSpan.FromMinutes(ExpireMinutes));

            var html = $@"
<p>Xin chào,</p>
<p>Mã xác thực để đặt lại mật khẩu là: <b>{code}</b></p>
<p>Mã sẽ hết hạn sau {ExpireMinutes} phút.</p>";

            // ✅ FIX: Gọi đúng signature với displayName
            await _email.SendEmailAsync(email, "[DollStore] Mã OTP đặt lại mật khẩu", html, "Doll Store");
        }

        public Task<bool> VerifyOtpAsync(string email, string code)
        {
            if (!_cache.TryGetValue(Key(email), out OtpState? state))
                return Task.FromResult(false);

            if (state.ExpireAt <= DateTime.UtcNow)
            { 
                _cache.Remove(Key(email)); 
                return Task.FromResult(false); 
            }

            if (state.Attempts >= MaxAttempts)
            { 
                _cache.Remove(Key(email)); 
                return Task.FromResult(false); 
            }

            if (state.Code == code)
            { 
                _cache.Remove(Key(email)); 
                return Task.FromResult(true); 
            }

            // Sai mã → tăng lượt thử
            var remain = state.ExpireAt - DateTime.UtcNow;
            state = state with { Attempts = state.Attempts + 1 };
            _cache.Set(Key(email), state, remain > TimeSpan.Zero ? remain : TimeSpan.FromMinutes(1));
            return Task.FromResult(false);
        }
    }
}
