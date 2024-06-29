using kzy_entities.Models.Response.Onboarding;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using kzy_entities.Constants;
using kzy_entities.Enums;

namespace UnitOfWorkDemo1.Services
{
    #region *** Interface ***

    public interface ITokenGenerator
    {
        public string GetBiometricToken(ProfileResponseModel model, string uniqueDeviceId, out DateTime expiryDate);
        public string GetAccessToken(ProfileResponseModel model, string uniqueDeviceId, out DateTime expiryDate);
        public string GetRefreshToken(ProfileResponseModel model, string uniqueDeviceId, out DateTime expiryDate);

    }
    #endregion
    #region *** Implementation ***

    public class TokenGenerator : ITokenGenerator
    {
        private readonly string Issuer;
        private readonly string Audience;
        private readonly string AccessTokenKey;
        private readonly string RefreshTokenKey;
        private readonly string OTPTokenKey;
        private readonly long accessTokenLifeSpam;
        private readonly long refreshTokenLifeSpam;
        private readonly long otpTokenLifeSpam;
        private readonly string BiometricTokenKey;
        public TokenGenerator(IConfiguration config)
        {
            Issuer = config["JWT:Issuer"];
            Audience = config["JWT:Audience"];
            AccessTokenKey = config["JWT:Keys:AccessToken"];
            RefreshTokenKey = config["JWT:Keys:RefreshToken"];
            OTPTokenKey = config["JWT:Keys:OTPToken"];
            BiometricTokenKey = config["JWT:Keys:BiometricToken"];
            //
           accessTokenLifeSpam = long.Parse(config["JWT:accessTokenLifeSpam"]);
           refreshTokenLifeSpam = long.Parse(config["JWT:refreshTokenLifeSpam"]);
           otpTokenLifeSpam = long.Parse(config["JWT:otpTokenLifeSpam"]);
        }
        public string GetBiometricToken(ProfileResponseModel model, string uniqueDeviceId, out DateTime expriyDate)
        {
            var claims = new[] {
                new Claim(ConstantStrings.EMAIL, model.Email),
                new Claim(ConstantStrings.USERID, model.Id),
                new Claim(ConstantStrings.FIRSTNAME, model.FirstName),
                new Claim(ConstantStrings.LASTNAME, model.LastName),
                new Claim(ConstantStrings.SESSIONID, uniqueDeviceId),
                new Claim(ConstantStrings.JTI, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(BiometricTokenKey));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            expriyDate = DateTime.UtcNow.AddMinutes(accessTokenLifeSpam);

            var token = new JwtSecurityToken(Issuer, Audience,
              claims,
              expires: expriyDate,
              signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GetAccessToken(ProfileResponseModel model, string uniqueDeviceId, out DateTime expriyDate)
        {
            var claims = new[] {
                new Claim(ConstantStrings.EMAIL, model.Email ?? ""),
                new Claim(ConstantStrings.USERID, model.Id),
                new Claim(ConstantStrings.FIRSTNAME, model.FirstName + string.Empty),
                new Claim(ConstantStrings.LASTNAME, model.LastName + string.Empty),
                new Claim(ConstantStrings.SESSIONID, uniqueDeviceId),
                new Claim(ConstantStrings.JTI, Guid.NewGuid().ToString()),
            };

            var aa = AccessTokenKey;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AccessTokenKey));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            expriyDate = DateTime.UtcNow.AddMinutes(accessTokenLifeSpam);

            var token = new JwtSecurityToken(Issuer, Audience,
              claims,
              expires: expriyDate,
              signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetRefreshToken(ProfileResponseModel model, string uniqueDeviceId, out DateTime expriyDate)
        {
            var claims = new[] {
                new Claim(ConstantStrings.SESSIONID, uniqueDeviceId),
                new Claim(ConstantStrings.USERID, model.Id),
                new Claim(ConstantStrings.JTI, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RefreshTokenKey));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
            expriyDate = DateTime.UtcNow.AddMinutes(refreshTokenLifeSpam);
            var token = new JwtSecurityToken(Issuer, Audience,
              claims,
              expires: expriyDate,
              signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    #endregion
}

