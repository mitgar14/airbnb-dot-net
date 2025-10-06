namespace Airbnb.Common.Constants;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Host = "Host";
    public const string Cliente = "Cliente";

    public static string[] GetAllRoles() => new[] { Admin, Host, Cliente };

    public static bool IsValidRole(string role)
    {
        return role == Admin || role == Host || role == Cliente;
    }
}
