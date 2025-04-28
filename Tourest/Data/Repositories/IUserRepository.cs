using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface IUserRepository
    {
        Task<(IEnumerable<User> Users, int TotalCount)> GetUsersByRolePagedAsync(string roleName, int pageIndex = 1, int pageSize = 10, string searchTerm = "");
        Task<User?> GetUserWithAccountByIdAsync(int userId);
        Task<User?> FindByIdAsync(int userId);
        Task<bool> CheckEmailExistsAsync(string email, int? excludeUserId = null);
        Task<bool> CheckUsernameExistsAsync(string username, int? excludeUserId = null);
        Task<Account?> FindAccountByUsernameAsync(string username);
        Task<(bool Success, User? CreatedUser)> AddUserAndAccountAsync(User user, Account account); // Trả về User đã tạo
        Task<bool> UpdateUserAsync(User user);
        Task<bool> UpdateAccountAsync(Account account);
        Task<User?> GetGuideByIdAsync(int userId);
        Task<IEnumerable<TourGuideAssignment>> GetAssignmentsByManagerAsync(int managerUserId);
        Task<(bool Success, string? ErrorMessage)> UpdateFullNameAndAddressAsync(int userId, string fullName, string address);
        //Task<User?> GetGuideByIdAsync(int userId); // Đã có
        Task UpdateTourGuideRatingAsync(int tourGuideId, decimal newAverageRating); // Thêm phương thức cập nhật rating

        // --- Phương thức MỚI/SỬA ĐỔI cho TourGuide ---

        // Lấy User kèm Account VÀ TourGuide profile
        Task<User?> GetUserWithAccountAndGuideProfileByIdAsync(int userId);

        // Thêm User, Account VÀ TourGuide profile (trong transaction)
        Task<(bool Success, User? CreatedUser)> AddUserAccountAndGuideProfileAsync(User user, Account account, Tourest.Data.Entities.TourGuide guideProfile);

        // Cập nhật TourGuide profile
        Task<bool> UpdateTourGuideAsync(Tourest.Data.Entities.TourGuide guideProfile);

        // Lấy assignments được dẫn bởi Guide này
        Task<IEnumerable<TourGuideAssignment>> GetAssignmentsLedByGuideAsync(int guideUserId);

        // Lấy ratings mà Guide này nhận được
        Task<IEnumerable<Rating>> GetRatingsReceivedByGuideAsync(int guideUserId);
        Task<bool> ChangePassword(int uid, string newPassword, string currentPass);

    }

}

