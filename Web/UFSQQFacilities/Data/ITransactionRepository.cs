using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public interface ITransactionRepository: IRepoBase<Transaction>
    {
        IQueryable<Transaction> GetTransactionsWithBookings();
    }
}
