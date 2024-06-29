using AutoMapper;
using Azure.Core;
using kzy_entities.Common;
using kzy_entities.Constants;
using kzy_entities.DBContext;
using kzy_entities.Entities;
using kzy_entities.Models.Request.Onboarding;
using kzy_entities.Models.Request.Product;
using kzy_entities.Models.Response.Onboarding;
using kzy_entities.Models.Response.Product;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Text;
using UnitOfWorkDemo.Repositories;
using UnitOfWorkDemo1.Common;
using UnitOfWorkDemo1.Services;

namespace UnitOfWorkDemo1.BL
{
    public interface IOnboardingBL 
    {
        //Task<ResponseModel<string>> LogIn(LogInRequestModel createRequestModel);
        Task<ResponseModel<SuccessLoginResponseModel>> Register(SignupRequestModel model);
        ResponseModel<SuccessLoginResponseModel> Login(LogInRequestModel model);

    }
    public class OnboardingBL : BaseBL, IOnboardingBL
    {
        private readonly IUnitOfWork<ApplicationDbContext, ReaderDbContext> _unitOfWork;
        private readonly IErrorCodeProvider _errorCodeProvider;
        protected readonly IMapper _mapper;
        private readonly ITokenGenerator _tokenGenerator;


        public OnboardingBL(IUnitOfWork<ApplicationDbContext, ReaderDbContext> unitOfWork,
                                         IErrorCodeProvider errorCodeProvider, IMapper mapper, ITokenGenerator tokenGenerator)
                                        : base(unitOfWork, errorCodeProvider)
        {
            _unitOfWork = unitOfWork;
            _errorCodeProvider = errorCodeProvider;
            _mapper = mapper;
            _tokenGenerator = tokenGenerator;
        }
        public ResponseModel<SuccessLoginResponseModel> Login(LogInRequestModel model)
        {
            var cUser = _unitOfWork.GetRepository<Customer>().Query(x => x.Email == "kzy@gmail.com", true).FirstOrDefault();

            if (cUser is null)
                return errorCodeProvider.GetResponseModel<SuccessLoginResponseModel>(ErrorCode.E119);

            // Check user have password
            if (string.IsNullOrEmpty(cUser.Hash) || string.IsNullOrEmpty(cUser.Salt))
            {
                return errorCodeProvider.GetResponseModel<SuccessLoginResponseModel>(ErrorCode.E1402);
            }

            // Validate password
            var isValid = PasswordHelper.ValidatePassword(model.Password, new PasswordHelper.PBKDF2Result
            {
                Hash = cUser.Hash,
                Salt = cUser.Salt
            });

            var registerUser = _mapper.Map<ProfileResponseModel>(cUser);
            var data = GetSuccessLoginInfo(registerUser);

            return errorCodeProvider.GetResponseModel<SuccessLoginResponseModel>(ErrorCode.E0, data);
        }

        public async Task<ResponseModel<SuccessLoginResponseModel>> Register(SignupRequestModel model)
        {
            int mobileCode = 0;
            string mobile = string.Empty;
            string email = string.Empty;

            if (mobileCode == 0 && model.MobileCode != 0)
            {
                mobileCode = model.MobileCode;
            }

            if (string.IsNullOrEmpty(mobile))
            {
                mobile = model.Mobile;
            }

            if (string.IsNullOrEmpty(email))
            {
                email = model.Email;
            }

            #region Check Email Address Valid
            var isEmailValid = IsValidEmail(email, "Development");
            if (!isEmailValid)
            {
                return errorCodeProvider.GetResponseModel<SuccessLoginResponseModel>(ErrorCode.E100);
            }
            #endregion

            #region Check If Email already Exists
            if (!string.IsNullOrEmpty(email))
            {
                var isEmailExisted = await unitOfWork.GetRepository<Customer>().Query(x => x.Email == email).AnyAsync();
                if (isEmailExisted)
                {
                    return errorCodeProvider.GetResponseModel<SuccessLoginResponseModel>(ErrorCode.E101);
                }
            }
            #endregion

            #region Check If Mobile already Exists
            if (!string.IsNullOrEmpty(mobile))
            {
                var isMobileExisted = await unitOfWork.GetRepository<Customer>().Query(x => x.MobileCode == mobileCode && x.Mobile == mobile).AnyAsync();
                if (isMobileExisted)
                {
                    return errorCodeProvider.GetResponseModel<SuccessLoginResponseModel>(ErrorCode.E124);
                }
            }
            #endregion

            // Validate password
            if (!IsValidPassword(model.Password))
            {
                return errorCodeProvider.GetResponseModel<SuccessLoginResponseModel>(ErrorCode.E113);
            }

            PasswordHelper.PBKDF2Result hashResult = PasswordHelper.HashPassword(model.Password);

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                DOB = model.Dob,
                Email = email,
                Status = Status.Active,
                Type = model.Type,
                Hash = hashResult.Hash,
                Salt = hashResult.Salt,
                MobileCode = mobileCode,
                Mobile = mobile,
                ProfileImageUrl = model.ProfileImageUrl,
                CreatedOn = DateTime.UtcNow
            };

            unitOfWork.GetRepository<Customer>().Add(customer);
            await unitOfWork.SaveChangesAsync();

            return errorCodeProvider.GetResponseModel<SuccessLoginResponseModel>(ErrorCode.E0);
        }
        public SuccessLoginResponseModel GetSuccessLoginInfo(ProfileResponseModel model)
        {
            Guid sessionId = Guid.NewGuid(); // To control single device and CMS logout

            var aa = _tokenGenerator.GetAccessToken(model, sessionId.ToString(), out DateTime expiryDategg);

            var bb = _tokenGenerator.GetRefreshToken(model, sessionId.ToString(), out DateTime refreshExpiryDategg);

            return new SuccessLoginResponseModel
            {
                AccessToken = _tokenGenerator.GetAccessToken(model, sessionId.ToString(), out DateTime expiryDate),
                RefreshToken = _tokenGenerator.GetRefreshToken(model, sessionId.ToString(), out DateTime refreshExpiryDate),
                Profile = model,
                ExpiryDatetime = expiryDate,
                RefreshExpiryDatetime = refreshExpiryDate
            };
        }
    }
}