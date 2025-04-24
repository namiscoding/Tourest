using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;
using Tourest.Data.Repositories;
using Tourest.Helpers;
using Tourest.ViewModels.Admin;

namespace Tourest.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        // Role constants
        private const string CUSTOMER_ROLE = "Customer";
        private const string TOURMANAGER_ROLE = "TourManager";
        private const string TOURGUIDE_ROLE = "TourGuide";
        private const string ADMIN_ROLE = "Admin";

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<PaginatedList<AdminCustomerViewModel>> GetCustomersForAdminAsync(int pageIndex = 1, int pageSize = 10, string searchTerm = "")
        {
            _logger.LogInformation("Fetching customers for admin. Page: {PageIndex}, Size: {PageSize}, Search: {SearchTerm}", pageIndex, pageSize, searchTerm);
            var (users, totalCount) = await _userRepository.GetUsersByRolePagedAsync("Customer", pageIndex, pageSize, searchTerm);

            // Map to ViewModel
            var viewModels = users.Select(u => new AdminCustomerViewModel
            {
                UserId = u.UserID,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                RegistrationDate = u.RegistrationDate
            }).ToList();

            // Tạo PaginatedList từ kết quả đã phân trang và tổng số bản ghi
            return new PaginatedList<AdminCustomerViewModel>(viewModels, totalCount, pageIndex, pageSize);
        }

        public async Task<AdminCustomerDetailsViewModel?> GetCustomerDetailsForAdminAsync(int userId)
        {
            _logger.LogInformation("Getting customer details for admin. UserID: {UserId}", userId);
            var user = await _userRepository.GetUserWithAccountByIdAsync(userId);

            if (user?.Account?.Role != "Customer")
            {
                _logger.LogWarning("User {UserId} not found or is not a customer.", userId);
                return null;
            }

            // Map to Details ViewModel
            return new AdminCustomerDetailsViewModel
            {
                UserId = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                RegistrationDate = user.RegistrationDate,
                Address = user.Address
                // Thêm các trường khác nếu cần
            };
        }

        public async Task<(bool Success, string ErrorMessage)> CreateCustomerByAdminAsync(AdminCreateCustomerViewModel model)
        {
            _logger.LogInformation("Attempting to create customer by admin. Email: {Email}", model.Email);
            // 1. Validate username/email uniqueness
            if (await _userRepository.CheckEmailExistsAsync(model.Email))
            {
                return (false, "Email đã tồn tại.");
            }
            // Username thường dùng email, kiểm tra lại nếu có username riêng
            // if (await _userRepository.CheckUsernameExistsAsync(model.Email)) // Dùng Email làm Username
            // {
            //     return (false, "Username (Email) đã tồn tại.");
            // }

            // 2. Hash password (Install BCrypt.Net-Next package)
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // 3. Create User entity
            var newUser = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                IsActive = model.IsActive,
                RegistrationDate = DateTime.UtcNow // Hoặc để DB tự tạo Default
            };

            // 4. Create Account entity
            var newAccount = new Account
            {
                // UserID sẽ được gán trong Repository
                Username = model.Email, // Dùng Email làm Username
                PasswordHash = hashedPassword,
                Role = "Customer" // Gán vai trò Customer
            };

            // 5. Call Repository to add both
            var (repoSuccess, createdUser) = await _userRepository.AddUserAndAccountAsync(newUser, newAccount);

            if (repoSuccess && createdUser != null)
            {
                _logger.LogInformation("Customer created successfully by admin. UserID: {UserId}", createdUser.UserID);
                return (true, string.Empty);
            }
            else
            {
                _logger.LogError("Failed to create customer by admin in repository. Email: {Email}", model.Email);
                return (false, "Đã có lỗi xảy ra trong quá trình tạo tài khoản.");
            }
        }

        public async Task<EditCustomerViewModel?> GetCustomerForEditAsync(int userId)
        {
            _logger.LogInformation("Getting customer for edit. UserID: {UserId}", userId);
            var user = await _userRepository.GetUserWithAccountByIdAsync(userId);

            if (user?.Account?.Role != "Customer")
            {
                _logger.LogWarning("User {UserId} not found or is not a customer for editing.", userId);
                return null;
            }

            // Map to Edit ViewModel
            return new EditCustomerViewModel
            {
                UserId = user.UserID,
                FullName = user.FullName,
                Email = user.Email, // Cho phép sửa Email cần kiểm tra unique lại trong Update
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                IsActive = user.IsActive
            };
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateCustomerByAdminAsync(EditCustomerViewModel model)
        {
            _logger.LogInformation("Attempting to update customer by admin. UserID: {UserId}", model.UserId);
            var user = await _userRepository.GetUserWithAccountByIdAsync(model.UserId);

            if (user?.Account?.Role != "Customer")
            {
                return (false, "Không tìm thấy khách hàng hoặc người dùng không phải khách hàng.");
            }

            // Kiểm tra unique Email nếu nó bị thay đổi
            if (user.Email.ToLower() != model.Email.ToLower())
            {
                if (await _userRepository.CheckEmailExistsAsync(model.Email, model.UserId))
                {
                    return (false, "Email mới đã tồn tại.");
                }
                user.Email = model.Email;
                // Nếu username = email, cũng cần cập nhật account.Username và kiểm tra unique username
                if (user.Account != null && user.Account.Username.ToLower() != model.Email.ToLower())
                {
                    // if (await _userRepository.CheckUsernameExistsAsync(model.Email, model.UserId))
                    // {
                    //     return (false, "Username (Email) mới đã tồn tại.");
                    // }
                    user.Account.Username = model.Email; // Cập nhật Username
                    await _userRepository.UpdateAccountAsync(user.Account); // Lưu thay đổi Account
                }
            }

            // Cập nhật các trường User
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.IsActive = model.IsActive;

            // Lưu thay đổi User
            bool updateSuccess = await _userRepository.UpdateUserAsync(user);

            if (updateSuccess)
            {
                _logger.LogInformation("Customer {UserId} updated successfully by admin.", model.UserId);
                return (true, string.Empty);
            }
            else
            {
                _logger.LogError("Failed to update customer {UserId} in repository.", model.UserId);
                return (false, "Đã có lỗi xảy ra trong quá trình cập nhật.");
            }
        }

        // --- Tour Manager Methods ---
        public async Task<PaginatedList<AdminTourManagerViewModel>> GetTourManagersForAdminAsync(int pageIndex = 1, int pageSize = 10, string searchTerm = "")
        {
            _logger.LogInformation("Fetching tour managers for admin. Page: {PageIndex}, Size: {PageSize}, Search: {SearchTerm}", pageIndex, pageSize, searchTerm);
            var (users, totalCount) = await _userRepository.GetUsersByRolePagedAsync(TOURMANAGER_ROLE, pageIndex, pageSize, searchTerm);

            // Map sang AdminTourManagerViewModel
            var viewModels = users.Select(u => new AdminTourManagerViewModel
            {
                UserId = u.UserID,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                RegistrationDate = u.RegistrationDate
                // Thêm các trường khác nếu ViewModel có
            }).ToList();

            return new PaginatedList<AdminTourManagerViewModel>(viewModels, totalCount, pageIndex, pageSize);
        }

        public async Task<AdminTourManagerDetailsViewModel?> GetTourManagerDetailsForAdminAsync(int userId)
        {
            _logger.LogInformation("Getting tour manager details for admin. UserID: {UserId}", userId);
            var user = await _userRepository.GetUserWithAccountByIdAsync(userId);

            if (user?.Account?.Role != TOURMANAGER_ROLE)
            {
                _logger.LogWarning("User {UserId} not found or is not a tour manager.", userId);
                return null;
            }

            // Lấy danh sách assignments do manager này thực hiện
            var assignments = await _userRepository.GetAssignmentsByManagerAsync(userId);

            // Map User sang ViewModel chính
            var viewModel = new AdminTourManagerDetailsViewModel
            {
                UserId = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                RegistrationDate = user.RegistrationDate,
                Address = user.Address,
                AssignmentsMade = new List<AssignmentInfoViewModel>() // Khởi tạo list
            };

            // Map danh sách assignments sang AssignmentInfoViewModel
            foreach (var assignment in assignments)
            {
                viewModel.AssignmentsMade.Add(new AssignmentInfoViewModel
                {
                    AssignmentId = assignment.AssignmentID,
                    // Lấy tên Tour từ TourGroup->Tour, xử lý null
                    TourName = assignment.TourGroup?.Tour?.Name ?? "N/A",
                    DepartureDate = assignment.TourGroup?.DepartureDate,
                    // Lấy tên Guide từ User liên quan, xử lý null
                    TourGuideName = assignment.TourGuide?.FullName ?? "N/A",
                    AssignmentStatus = assignment.Status,
                    AssignmentDate = assignment.AssignmentDate
                });
            }

            return viewModel;
        }

        public async Task<(bool Success, string ErrorMessage)> CreateTourManagerByAdminAsync(AdminCreateTourManagerViewModel model)
        {
            _logger.LogInformation("Attempting to create tour manager by admin. Email: {Email}", model.Email);
            if (await _userRepository.CheckEmailExistsAsync(model.Email))
            {
                return (false, "Email đã tồn tại.");
            }
            // Username dùng Email
            if (await _userRepository.CheckUsernameExistsAsync(model.Email))
            {
                return (false, "Username (Email) đã tồn tại.");
            }

            // Hash password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var newUser = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                IsActive = model.IsActive,
                RegistrationDate = DateTime.UtcNow
            };

            var newAccount = new Account
            {
                Username = model.Email, // Dùng Email làm Username
                PasswordHash = hashedPassword,
                Role = TOURMANAGER_ROLE // Gán vai trò TourManager
                // UserID sẽ được gán trong Repository
            };

            var (repoSuccess, createdUser) = await _userRepository.AddUserAndAccountAsync(newUser, newAccount);

            if (repoSuccess && createdUser != null)
            {
                _logger.LogInformation("Tour Manager created successfully by admin. UserID: {UserId}", createdUser.UserID);
                return (true, string.Empty);
            }
            else
            {
                _logger.LogError("Failed to create tour manager by admin in repository. Email: {Email}", model.Email);
                return (false, "Đã có lỗi xảy ra trong quá trình tạo tài khoản.");
            }
        }

        public async Task<EditTourManagerViewModel?> GetTourManagerForEditAsync(int userId)
        {
            _logger.LogInformation("Getting tour manager for edit. UserID: {UserId}", userId);
            var user = await _userRepository.GetUserWithAccountByIdAsync(userId);

            if (user?.Account?.Role != TOURMANAGER_ROLE)
            {
                _logger.LogWarning("User {UserId} not found or is not a tour manager for editing.", userId);
                return null;
            }

            // Map sang Edit ViewModel
            return new EditTourManagerViewModel
            {
                UserId = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                IsActive = user.IsActive
            };
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateTourManagerByAdminAsync(EditTourManagerViewModel model)
        {
            _logger.LogInformation("Attempting to update tour manager by admin. UserID: {UserId}", model.UserId);
            var user = await _userRepository.GetUserWithAccountByIdAsync(model.UserId);

            if (user?.Account?.Role != TOURMANAGER_ROLE)
            {
                return (false, "Không tìm thấy quản lý tour hoặc người dùng không phải quản lý tour.");
            }

            // Kiểm tra unique Email nếu nó bị thay đổi
            if (user.Email.ToLower() != model.Email.ToLower())
            {
                if (await _userRepository.CheckEmailExistsAsync(model.Email, model.UserId))
                {
                    return (false, "Email mới đã tồn tại.");
                }
                user.Email = model.Email;
                // Cập nhật cả Username nếu nó bằng Email
                if (user.Account != null && user.Account.Username.ToLower() != model.Email.ToLower())
                {
                    // Check unique username if necessary
                    user.Account.Username = model.Email;
                    await _userRepository.UpdateAccountAsync(user.Account);
                }
            }

            // Cập nhật các trường User
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.IsActive = model.IsActive;

            bool updateSuccess = await _userRepository.UpdateUserAsync(user);

            if (updateSuccess)
            {
                _logger.LogInformation("Tour Manager {UserId} updated successfully by admin.", model.UserId);
                return (true, string.Empty);
            }
            else
            {
                _logger.LogError("Failed to update tour manager {UserId} in repository.", model.UserId);
                return (false, "Đã có lỗi xảy ra trong quá trình cập nhật.");
            }
        }

        // --- Tour Guide Methods Implementation ---
        public async Task<PaginatedList<AdminTourGuideViewModel>> GetTourGuidesForAdminAsync(int pageIndex = 1, int pageSize = 10, string searchTerm = "")
        {
            _logger.LogInformation("Fetching tour guides for admin. Page: {PageIndex}, Size: {PageSize}, Search: {SearchTerm}", pageIndex, pageSize, searchTerm);
            // Repo trả về User đã include TourGuide profile
            var (users, totalCount) = await _userRepository.GetUsersByRolePagedAsync(TOURGUIDE_ROLE, pageIndex, pageSize, searchTerm);

            var viewModels = users.Select(u => new AdminTourGuideViewModel
            {
                UserId = u.UserID,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                ExperienceLevel = u.TourGuide?.ExperienceLevel, // Lấy từ profile
                AverageRating = u.TourGuide?.AverageRating // Lấy từ profile
            }).ToList();

            return new PaginatedList<AdminTourGuideViewModel>(viewModels, totalCount, pageIndex, pageSize);
        }

        public async Task<AdminTourGuideDetailsViewModel?> GetTourGuideDetailsForAdminAsync(int userId)
        {
            _logger.LogInformation("Getting tour guide details for admin. UserID: {UserId}", userId);
            // Repo trả về User đã include Account và TourGuide profile
            var user = await _userRepository.GetUserWithAccountAndGuideProfileByIdAsync(userId);

            if (user?.Account?.Role != TOURGUIDE_ROLE || user.TourGuide == null)
            {
                _logger.LogWarning("User {UserId} not found, is not a tour guide, or missing profile.", userId);
                return null;
            }

            // Lấy thêm assignments và ratings
            var assignments = await _userRepository.GetAssignmentsLedByGuideAsync(userId);
            var ratings = await _userRepository.GetRatingsReceivedByGuideAsync(userId);

            // Map sang Details ViewModel
            var viewModel = new AdminTourGuideDetailsViewModel
            {
                UserId = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                RegistrationDate = user.RegistrationDate,
                Address = user.Address,
                // Profile info
                ExperienceLevel = user.TourGuide.ExperienceLevel,
                LanguagesSpoken = user.TourGuide.LanguagesSpoken,
                Specializations = user.TourGuide.Specializations,
                MaxCapacity = user.TourGuide.MaxGroupSizeCapacity,
                AverageRating = user.TourGuide.AverageRating,
                // Assignments
                AssignmentsLed = assignments.Select(a => new AssignmentLedViewModel
                {
                    AssignmentId = a.AssignmentID,
                    TourName = a.TourGroup?.Tour?.Name ?? "N/A",
                    DepartureDate = a.TourGroup?.DepartureDate,
                    AssignmentStatus = a.Status
                }).ToList(),
                // Ratings
                RatingsReceived = ratings.Select(r => new RatingReceivedViewModel
                {
                    RatingId = r.RatingID,
                    RatingValue = r.RatingValue,
                    Comment = r.Comment,
                    RatingDate = r.RatingDate,
                    CustomerName = r.Customer?.FullName ?? "N/A" // Lấy tên Customer
                }).ToList()
            };

            return viewModel;
        }

        public async Task<(bool Success, string ErrorMessage)> CreateTourGuideByAdminAsync(AdminCreateTourGuideViewModel model)
        {
            _logger.LogInformation("Attempting to create tour guide by admin. Email: {Email}", model.Email);
            // Validate uniqueness
            if (await _userRepository.CheckEmailExistsAsync(model.Email)) return (false, "Email đã tồn tại.");
            if (await _userRepository.CheckUsernameExistsAsync(model.Email)) return (false, "Username (Email) đã tồn tại.");

            // Hash password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Create User
            var newUser = new User { /* ... gán từ model ... */ FullName = model.FullName, Email = model.Email, PhoneNumber = model.PhoneNumber, Address = model.Address, IsActive = model.IsActive };
            // Create Account
            var newAccount = new Account { /* ... gán từ model ... */ Username = model.Email, PasswordHash = hashedPassword, Role = TOURGUIDE_ROLE };
            // Create TourGuide Profile
            var newGuideProfile = new Tourest.Data.Entities.TourGuide
            {
                ExperienceLevel = model.ExperienceLevel,
                LanguagesSpoken = model.LanguagesSpoken,
                Specializations = model.Specializations,
                MaxGroupSizeCapacity = model.MaxGroupSizeCapacity
                // AverageRating sẽ tự tính hoặc null ban đầu
            };

            // Call Repo to add all three in transaction
            var (repoSuccess, createdUser) = await _userRepository.AddUserAccountAndGuideProfileAsync(newUser, newAccount, newGuideProfile);

            if (repoSuccess && createdUser != null)
            {
                _logger.LogInformation("Tour Guide created successfully by admin. UserID: {UserId}", createdUser.UserID);
                return (true, string.Empty);
            }
            else
            {
                _logger.LogError("Failed to create tour guide by admin in repository. Email: {Email}", model.Email);
                return (false, "Đã có lỗi xảy ra trong quá trình tạo tài khoản.");
            }
        }

        public async Task<EditTourGuideViewModel?> GetTourGuideForEditAsync(int userId)
        {
            _logger.LogInformation("Getting tour guide for edit. UserID: {UserId}", userId);
            var user = await _userRepository.GetUserWithAccountAndGuideProfileByIdAsync(userId);

            if (user?.Account?.Role != TOURGUIDE_ROLE || user.TourGuide == null)
            {
                _logger.LogWarning("User {UserId} not found, is not a tour guide, or missing profile for editing.", userId);
                return null;
            }

            // Map User và TourGuide sang Edit ViewModel
            return new EditTourGuideViewModel
            {
                UserId = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                IsActive = user.IsActive,
                // Profile fields
                ExperienceLevel = user.TourGuide.ExperienceLevel,
                LanguagesSpoken = user.TourGuide.LanguagesSpoken,
                Specializations = user.TourGuide.Specializations,
                MaxGroupSizeCapacity = user.TourGuide.MaxGroupSizeCapacity
            };
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateTourGuideByAdminAsync(EditTourGuideViewModel model)
        {
            _logger.LogInformation("Attempting to update tour guide by admin. UserID: {UserId}", model.UserId);
            var user = await _userRepository.GetUserWithAccountAndGuideProfileByIdAsync(model.UserId);

            if (user?.Account?.Role != TOURGUIDE_ROLE || user.TourGuide == null)
            {
                return (false, "Không tìm thấy hướng dẫn viên hoặc hồ sơ.");
            }

            // Validate Email uniqueness if changed
            if (user.Email.ToLower() != model.Email.ToLower())
            {
                if (await _userRepository.CheckEmailExistsAsync(model.Email, model.UserId))
                {
                    return (false, "Email mới đã tồn tại.");
                }
                user.Email = model.Email;
                // Update username if it's the same as email
                if (user.Account != null && user.Account.Username.ToLower() != model.Email.ToLower())
                {
                    // if (await _userRepository.CheckUsernameExistsAsync(model.Email, model.UserId)) return (false, "Username (Email) mới đã tồn tại.");
                    user.Account.Username = model.Email;
                    await _userRepository.UpdateAccountAsync(user.Account);
                }
            }

            // Update User fields
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.IsActive = model.IsActive;

            // Update TourGuide profile fields
            user.TourGuide.ExperienceLevel = model.ExperienceLevel;
            user.TourGuide.LanguagesSpoken = model.LanguagesSpoken;
            user.TourGuide.Specializations = model.Specializations;
            user.TourGuide.MaxGroupSizeCapacity = model.MaxGroupSizeCapacity;

            // Save changes - EF Core tracks changes on related entities loaded in the same context
            var success = await _userRepository.UpdateUserAsync(user); // This might trigger saving related TourGuide changes too if tracked
                                                                       // Optionally call UpdateTourGuideAsync explicitly if needed or if repo separation is strict
                                                                       // var successGuide = await _userRepository.UpdateTourGuideAsync(user.TourGuide);

            if (success) // Check primary update success
            {
                _logger.LogInformation("Tour Guide {UserId} updated successfully by admin.", model.UserId);
                return (true, string.Empty);
            }
            else
            {
                _logger.LogError("Failed to update tour guide {UserId} in repository.", model.UserId);
                return (false, "Đã có lỗi xảy ra trong quá trình cập nhật.");
            }
        }


        // --- General User Methods ---
        
        public async Task<bool> ToggleUserActiveStatusAsync(int userId)
        {
            _logger.LogInformation("Toggling active status for UserID: {UserId}", userId);
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for toggling status.", userId);
                return false;
            }

            user.IsActive = !user.IsActive;
            bool success = await _userRepository.UpdateUserAsync(user);
            if (success)
            {
                _logger.LogInformation("Successfully toggled active status for UserID {UserId} to {IsActive}", userId, user.IsActive);
            }
            else
            {
                _logger.LogError("Failed to toggle active status for UserID {UserId}", userId);
            }
            return success;
        }
    }
}
