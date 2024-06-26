using kzy_entities.Common;
using kzy_entities.Converter;
using kzy_entities.DBContext;
using kzy_entities.Entities;
using kzy_entities.Models.Request.Product;
using UnitOfWorkDemo.Repositories;
using System.Threading.Tasks;

namespace UnitOfWorkDemo1.BL
{
    #region interfaces
    public interface IProductBL
    {
        Task<ResponseModel<string>> CreateProduct(CreateProductRequestModel createRequestModel);
    }
    #endregion

    public class ProductBL : BaseBL, IProductBL
    {
        public ProductBL
            (IUnitOfWork<ApplicationDbContext, ReaderDbContext> unitOfWork,
            IErrorCodeProvider errorCodeProvider)
            : base(unitOfWork, errorCodeProvider)
        {
        }

        //public ResponseModel<string> CreateProduct(CreateProductRequestModel model)
        //{
        //    try
        //    {
        //        Product product = new Product
        //        {
        //            Id = model.Id,
        //            Name = model.Name,
        //            Price = model.Price
        //        };

        //         unitOfWork.GetRepository<Product>().AddAsync(product);
        //         unitOfWork.GGWPChangesAsync();
        //    }
        //    catch (Exception)
        //    {
        //        return errorCodeProvider.GetResponseModel<string>(ErrorCode.E404);
        //    }
        //    return errorCodeProvider.GetResponseModel<string>(ErrorCode.E0);

        //}

        public async Task<ResponseModel<string>> CreateProduct(CreateProductRequestModel model)
        {
            try
            {
                Product product = new Product
                {
                    Id = model.Id,
                    Name = model.Name,
                    Price = model.Price
                };

                // Add product to repository asynchronously
                await unitOfWork.GetRepository<Product>().AddAsync(product);

                // Save changes to the database asynchronously
                int result = await unitOfWork.GGWPChangesAsync();

                if (result > 0)
                {
                    // Product successfully created
                    return errorCodeProvider.GetResponseModel<string>(ErrorCode.E0);
                }
                else
                {
                    // Handle scenario where no changes were made (optional)
                    return errorCodeProvider.GetResponseModel<string>(ErrorCode.E404);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (recommended)
                // logger.LogError(ex, "Error creating product");

                // Return an error response
                return errorCodeProvider.GetResponseModel<string>(ErrorCode.E404);
            }
        }

    }
}
