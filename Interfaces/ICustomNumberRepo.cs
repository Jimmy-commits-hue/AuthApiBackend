namespace AuthApi.Interfaces
{

    public interface ICustomNumberRepo
    {

        Task<string?> GetLastCustomNumber();

    }
    
}
