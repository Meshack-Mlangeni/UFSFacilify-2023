using UFSQQFacilities.Models;

namespace UFSQQFacilities.Data
{
    public interface IRecoveryRepository : IRepoBase<Recovery>
    {
        string FindUserQuestion(string email); 
        bool VerifyAnswer(string answer, string email);
    }
}
