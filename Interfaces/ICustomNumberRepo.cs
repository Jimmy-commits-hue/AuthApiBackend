namespace Web.Interfaces
{

    public interface ICustomNumberRepo
    {

        Task<string?> GetLastCustomNumber();

    }
    
}
