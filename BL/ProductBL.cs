using kzy_entities.Common;
using kzy_entities.Converter;
using kzy_entities.DBContext;
using kzy_entities.Entities;
using kzy_entities.Models.Request.Product;
using UnitOfWorkDemo.Repositories;
using System.Threading.Tasks;
using System;
using System.Threading;
using kzy_entities.Models.Response.Product;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace UnitOfWorkDemo1.BL
{
    #region interfaces
    public interface IProductBL
    {
        Task<ResponseModel<string>> CreateProduct(CreateProductRequestModel createRequestModel);
        Task<ResponseModel<List<ProductListResponseModel>> >GetProductList();

    }
    #endregion

    public class ProductBL :  IProductBL
    {
        private readonly IUnitOfWork<ApplicationDbContext, ReaderDbContext> _unitOfWork;
        private readonly IErrorCodeProvider _errorCodeProvider;
        protected readonly IMapper mapper;


        public ProductBL
            (IUnitOfWork<ApplicationDbContext, ReaderDbContext> unitOfWork,
            IErrorCodeProvider errorCodeProvider)
        {
            _unitOfWork = unitOfWork;
            _errorCodeProvider = errorCodeProvider;
        }
        public async Task<ResponseModel<List<ProductListResponseModel>>> GetProductList()
        {
            try
            {
                var products =  _unitOfWork.GetRepository<Product>().Query(x=> x.Price > 0,true)
                    .OrderByDescending(x => x.CreatedOn)
                    .FirstOrDefault();

                var data = mapper.Map<ProductListResponseModel>(products);


                return _errorCodeProvider.GetResponseModel<List<ProductListResponseModel>>(ErrorCode.E0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return _errorCodeProvider.GetResponseModel<List<ProductListResponseModel>>(ErrorCode.E500);
            }
        }

        public async Task<ResponseModel<string>> CreateProduct(CreateProductRequestModel model)
        {
            try
            {
                Product product = new Product
                {
                    Id = new Guid(),
                    Name = model.Name,
                    Price = model.Price,
                    CreatedBy = null,
                    CreatedOn = DateTime.UtcNow,
                };

                _unitOfWork.GetRepository<Product>().Add(product);
                await _unitOfWork.SaveChangesAsync();
                return _errorCodeProvider.GetResponseModel<string>(ErrorCode.E0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return _errorCodeProvider.GetResponseModel<string>(ErrorCode.E500);
            }
        }

    }
}
