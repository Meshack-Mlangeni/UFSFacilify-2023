using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public class RecoveryRepository: RepoBase<Recovery>, IRecoveryRepository
    {
        public RecoveryRepository(AppDbContext context) : base(context) { }

        public string FindUserQuestion(string email)
        {
            return context.Recoveries.FirstOrDefault(u => u.UserEmail == email).SecurityQuestion;
        }

        public bool VerifyAnswer(string answer ,string email)
        {
            return context.Recoveries.FirstOrDefault(u => u.UserEmail == email)
                .SecurityAnswer.ToLower().Equals(answer.ToLower()) ;
        }
    }
}
