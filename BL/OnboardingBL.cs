using AutoMapper;
using kzy_entities.Common;
using kzy_entities.DBContext;
using kzy_entities.Entities;
using kzy_entities.Models.Request.Onboarding;
using kzy_entities.Models.Request.Product;
using kzy_entities.Models.Response.Product;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UnitOfWorkDemo.Repositories;

namespace UnitOfWorkDemo1.BL
{
    public interface IOnboardingBL
    {
        Task<ResponseModel<string>> LogIn(CreateProductRequestModel createRequestModel);

    }
    //public class OnboardingBL : IOnboardingBL
    //{
    //    private readonly IUnitOfWork<ApplicationDbContext, ReaderDbContext> _unitOfWork;
    //    private readonly IErrorCodeProvider _errorCodeProvider;

    //    public OnboardingBL()
    //    {
    //    }
    //    public async Task<ResponseModel<string>> LogIn(LogInRequestModel logInRequestModel);
    //    {

    //    }
    //}
}