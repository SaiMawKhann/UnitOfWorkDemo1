using kzy_entities.Common;
using kzy_entities.Constants;
using kzy_entities.DBContext;
using kzy_entities.Entities;
using kzy_entities.Enums;
using kzy_entities.Models.Request.Onboarding;
using kzy_entities.Models.Request.Product;
using kzy_entities.Models.Response.Onboarding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System.Security.Claims;
using UnitOfWorkDemo.Repositories;
using UnitOfWorkDemo1.BL;

namespace UnitOfWorkDemo1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnboardingController : Controller
    {
        private readonly IOnboardingBL onboardingBL;

        public OnboardingController(IOnboardingBL onboardingBL)
        {
            this.onboardingBL = onboardingBL;

        }
        [HttpPost]
        [SwaggerOperation("Create Customer")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<string>))]
        public async Task<IActionResult> Register([FromBody] SignupRequestModel signUpRequestModel)
        {
            return Ok(await onboardingBL.Register(signUpRequestModel));
        }
        [HttpPost("login")]
        [SwaggerOperation("Login")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseModel<SuccessLoginResponseModel>))]
        public IActionResult Login(LogInRequestModel model)
        {
            var loginSuccess = onboardingBL.Login(model);
            //AddHeaderOnSuccessLogin(loginSuccess);
            return Ok(loginSuccess);
        }
        private void AddHeaderOnSuccessLogin(ResponseModel<SuccessLoginResponseModel> response)
        {
            string origin = string.IsNullOrEmpty(Request.Headers["origin"]) ? string.Empty : Request.Headers["origin"];
            origin = origin.Contains("localhost") || string.IsNullOrEmpty(origin) ? Request.Headers["domain-url"] : origin;

            if (response.Code == (long)ErrorCode.E0)
            {
                CookieSigningIn(response.Data);

                var urls = HttpContext.Request.Cookies["domains"];
                if (!string.IsNullOrEmpty(urls))
                {
                    HttpContext.Response.Cookies.Delete("domains");

                }
                var objArray = new string[] { origin };
                var json = JsonConvert.SerializeObject(objArray);

                HttpContext.Response.Cookies.Append("domains", json, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(365),
                    SameSite = SameSiteMode.None
                });
            }
        }

        private async void CookieSigningIn(SuccessLoginResponseModel model)
        {
            HttpContext.Response.Cookies.Delete(ConstantStrings.COOKIE_REFRESH_AUTHENTICATION);
            await HttpContext.SignOutAsync(scheme: ConstantStrings.COOKIE_AUTHENTICATION);
            //remove old one

            HttpContext.Response.Cookies.Append(ConstantStrings.COOKIE_REFRESH_AUTHENTICATION, model.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(365)

            });

            var claims = new[] {
                new Claim(ConstantStrings.USERID, model.Profile.Id),
                new Claim(ConstantStrings.FIRSTNAME, model.Profile.FirstName + string.Empty),
                new Claim(ConstantStrings.LASTNAME, model.Profile.LastName + string.Empty),
                new Claim(ConstantStrings.JTI, Guid.NewGuid().ToString())
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, ConstantStrings.COOKIE_AUTHENTICATION);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(ConstantStrings.COOKIE_AUTHENTICATION, claimsPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    IssuedUtc = DateTime.UtcNow,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(30)
                }
            );
        }
    }
 }
