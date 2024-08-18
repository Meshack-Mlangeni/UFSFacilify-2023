using Microsoft.EntityFrameworkCore;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public class TransactionRepository: RepoBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext context):base(context) { }

        public IQueryable<Transaction> GetTransactionsWithBookings()
        {
            return context.Transaction.Include(b => b.Booking);
        }
    }
}
