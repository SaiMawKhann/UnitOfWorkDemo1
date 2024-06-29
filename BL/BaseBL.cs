using kzy_entities.Common;
using kzy_entities.DBContext;
using System.Text.RegularExpressions;
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

    protected bool IsValidEmail(string email, string Environment)
    {
        if (String.IsNullOrEmpty(email))
        {
            return true;
        }

        email = email.ToLower();

        if (email.Trim().EndsWith("."))
            return false;

        if (IsProduction(Environment))
        {
            if (email.Contains("@mailinator") || email.Contains("@yopmail"))
                return false;
        }

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    protected bool IsProduction(string Environment)
    {
            if (Environment?.ToLower() == "production")
            {
                return true;
            }
            return false;
    }
    protected bool IsValidPassword(string password)
    {
        /*
        ^: first line
        (?=.*[a-z]) : Should have at least one lower case
        (?=.*[A-Z]) : Should have at least one upper case
        (?=.*\d) : Should have at least one number
        (?=.*[#$^+=!*()@%&]) : Should have at least one special character // it's no needed as product requirement
        .{8,} : Minimum 8 characters
        $ : end line
        */

        var pattern = "^(?=.*[A-Za-z])(?=.*\\d)(?=.*[?#$^+=!*()@%&.|~_-])[A-Za-z\\d?#$^+=!*()@%&.|~_-]{8,}$";
        return Regex.IsMatch(password, pattern);
    }

}
