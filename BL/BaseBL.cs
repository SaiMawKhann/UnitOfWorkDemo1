using kzy_entities.Common;
using kzy_entities.DBContext;
using UnitOfWorkDemo.Repositories;

public abstract class BaseBL
{
    protected readonly IUnitOfWork<ApplicationDbContext, ReaderDbContext> unitOfWork;
    protected readonly IErrorCodeProvider errorCodeProvider;

    protected BaseBL(IUnitOfWork<ApplicationDbContext, ReaderDbContext> unitOfWork, IErrorCodeProvider errorCodeProvider)
    {
        this.unitOfWork = unitOfWork;
        this.errorCodeProvider = errorCodeProvider;
    }
}
