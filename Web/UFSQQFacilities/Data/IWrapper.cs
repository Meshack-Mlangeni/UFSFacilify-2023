namespace UFSQQFacilities.Data
{
    public interface IWrapper
    {
        IFacilityRepository FacilityRepository { get; }
        IBookingRepository BookingRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IRecoveryRepository RecoveryRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IImageRepository ImageRepository { get; }
        IUserRepository UserRepository { get; }
        IReviewRepository ReviewRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        IFavouriteRepository FavouriteRepository { get; }
        void Save();
    }
}
