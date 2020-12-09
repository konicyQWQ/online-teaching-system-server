using Microsoft.Extensions.Logging;
using OTS_API.DatabaseContext;
using OTS_API.Models;
using OTS_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OTS_API.Services
{
    public class PasswordRetrieveService
    {
        private readonly ILogger<PasswordRetrieveService> logger;

        private Dictionary<string, SToken> tokenMap;

        public PasswordRetrieveService(ILogger<PasswordRetrieveService> logger)
        {
            this.logger = logger;
            this.tokenMap = new Dictionary<string, SToken>();
        }

        public async Task<string> AddSTokenAsync(User userInfo, string email)
        {
            if (userInfo == null)
            {
                throw new Exception("用户不存在");
            }
            if (userInfo.Email != email)
            {
                throw new Exception("邮箱错误");
            }
            var t = new SToken(userInfo.Id, email);
            var tID = CodeGenerator.GetCode(20);
            t.ID = tID;
            await SendValidatingEmailAsync(t);

            tokenMap.Add(tID, t);
            return tID;
        }

        public Task VerifyAsync(string tokenID, string validationCode)
        {
            return Task.Run(() =>
            {
                if (tokenID == null || !tokenMap.ContainsKey(tokenID))
                {
                    throw new Exception("操作失败，请重试");
                }
                var token = tokenMap[tokenID];
                if (!token.IsValid())
                {
                    tokenMap.Remove(tokenID);
                    throw new Exception("操作失败，请重试");
                }
                if (token.ValidationCode != validationCode)
                {
                    throw new Exception("验证码不正确");
                }
                token.IsVerified = true;
            });
        }

        public async Task ResendEmailAsync(string tokenID)
        {
            if (tokenID == null || !tokenMap.ContainsKey(tokenID))
            {
                throw new Exception("操作失败，请重试");
            }
            var token = tokenMap[tokenID];
            if (!token.IsValid())
            {
                tokenMap.Remove(tokenID);
                throw new Exception("操作失败，请重试");
            }
            await SendValidatingEmailAsync(token);
        }

        public Task<string> ResetVerifyAsync(string tokenID)
        {
            return Task.Run(() =>
            {
                if (tokenID == null || !tokenMap.ContainsKey(tokenID))
                {
                    throw new Exception("操作失败，请重试");
                }
                var token = tokenMap[tokenID];
                if (!token.IsValid())
                {
                    tokenMap.Remove(tokenID);
                    throw new Exception("操作失败，请重试");
                }
                if (!token.IsVerified)
                {
                    throw new Exception("操作失败，请重试");
                }
                var userID = token.UserID;
                tokenMap.Remove(tokenID);
                return userID;
            });
        }

        private Task SendValidatingEmailAsync(SToken token)
        {
            return Task.Run(() =>
            {
                token.ValidationCode = CodeGenerator.GetCode(8);
                logger.LogInformation("Validation Code for User(id: " + token.UserID + ") is: " + token.ValidationCode);
                var mailMsg = new MailMessage();
                mailMsg.From = new MailAddress("446416074@qq.com", "Do-Not-Reply");
                mailMsg.To.Add(new MailAddress("3180102973@zju.edu.cn"));
                mailMsg.Subject = "线上教学系统：密码找回验证码";
                mailMsg.Body = "请勿将验证码泄露给他人！\n验证码：K3NLAONL3";
                var client = new SmtpClient();
                client.Host = "smtp.qq.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("446416074@qq.com", "qkjwrwnsydtsbiee");
                try
                {
                    client.SendAsync(mailMsg, null);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    throw new Exception("邮件发送失败");
                }
            });
        }
    }
}
