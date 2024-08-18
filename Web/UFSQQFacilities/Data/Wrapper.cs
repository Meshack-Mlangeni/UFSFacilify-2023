namespace UFSQQFacilities.Data
{
    public class Wrapper : IWrapper
    {
        private IBookingRepository bookingRepository;
        private IFacilityRepository facilityRepository;
        private INotificationRepository notificationsRepository;
        private IRecoveryRepository recoveryRepository;
        private ICategoryRepository categoryRepository;
        private IImageRepository imageRepository;
        private IUserRepository userRepository;
        private IReviewRepository reviewRepository;
        private ITransactionRepository transactionRepository;
        private IFavouriteRepository favouriteRepository;
        private AppDbContext context;

        public Wrapper(AppDbContext _context)
        {
            context = _context;
        }

        public IImageRepository ImageRepository
        {
            get
            {
                if (imageRepository == null)
                    imageRepository = new ImageRepository(context);
                return imageRepository;
            }
        }

        public IFavouriteRepository FavouriteRepository
        {
            get
            {
                if (favouriteRepository == null)
                    favouriteRepository = new FavouriteRepository(context);
                return favouriteRepository;
            }
        }

        public ITransactionRepository TransactionRepository
        {
            get
            {
                if (transactionRepository == null)
                    transactionRepository = new TransactionRepository(context);
                return transactionRepository;
            }
        }

        public IReviewRepository ReviewRepository
        {
            get
            {
                if (reviewRepository == null)
                    reviewRepository = new ReviewRepository(context);
                return reviewRepository;
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(context);
                return userRepository;
            }
        }

        public IFacilityRepository FacilityRepository
        {
            get
            {
                if (facilityRepository == null)
                    facilityRepository = new FacilityRepository(context);
                return facilityRepository;
            }
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (categoryRepository == null)
                    categoryRepository = new CategoryRepository(context);
                return categoryRepository;
            }
        }

        public IBookingRepository BookingRepository
        {
            get
            {
                if (bookingRepository == null)
                    bookingRepository = new BookingRepository(context);
                return bookingRepository;
            }
        }

        public INotificationRepository NotificationRepository
        {
            get
            {
                if (notificationsRepository == null)
                    notificationsRepository = new NotificationRepository(context);
                return notificationsRepository;
            }
        }

        public IRecoveryRepository RecoveryRepository
        {
            get
            {
                if (recoveryRepository == null)
                    recoveryRepository = new RecoveryRepository(context);
                return recoveryRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
