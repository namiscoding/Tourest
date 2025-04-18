/*
 ================================================================================
 Script: SeedInitialData_Complete_Fixed.sql
 Description: Populates initial data for ALL Tourest database tables.
              ** UPDATED ** to reflect price/amount columns changed to INT type.
              ** FIXED ** Removed GO statements inside transaction/try-catch block.
 Date: 2025-04-18
 Author: Your Name/Team
 ================================================================================
 ** WARNING - PLEASE READ CAREFULLY **
 1.  RUN THIS SCRIPT ONLY ONCE after the database schema (tables, constraints)
     has been successfully created or updated by EF Core Migrations.
     Running it multiple times WILL cause errors due to duplicate keys/data.
 2.  ASSUMED IDENTITY VALUES: This script ASSUMES that IDENTITY columns
     (UserID, CategoryID, TourID, BookingID, etc.) start at 1 and increment
     sequentially (1, 2, 3...). This assumption is FRAGILE and may break
     if run on a database with existing data or different IDENTITY seeds.
     Use with caution, primarily for initial development setup.
 3.  PASSWORD HASHING: This script inserts placeholder/empty values for PasswordHash.
     Real password hashes MUST be generated and updated securely via application logic.
     NEVER store plain text passwords. Seeded users need passwords set via the app.
 4.  DATA TYPE CHANGES: Values for AdultPrice, ChildPrice, TotalPrice, Amount,
     and RefundAmount have been changed from DECIMAL to INT as per your update.
 ================================================================================
*/

PRINT '================================================';
PRINT 'Starting Complete Seed Data Insertion...';
PRINT '================================================';
USE Tourest

BEGIN TRANSACTION; -- Start Transaction

BEGIN TRY

    -- 1. Seed Categories
    PRINT 'Seeding Categories...';
    INSERT INTO [dbo].[Categories] ([Name], [Description]) VALUES
    (N'Tour Biển Đảo', N'Khám phá vẻ đẹp biển Việt Nam'),       -- Assumed CategoryID = 1
    (N'Tour Leo Núi', N'Chinh phục các đỉnh cao'),           -- Assumed CategoryID = 2
    (N'Tour Văn Hóa Lịch Sử', N'Tìm hiểu về lịch sử, văn hóa'), -- Assumed CategoryID = 3
    (N'Tour Ngắn Ngày', N'Chuyến đi nhanh cuối tuần'),         -- Assumed CategoryID = 4
    (N'Tour Ẩm Thực', N'Khám phá đặc sản vùng miền');       -- Assumed CategoryID = 5
    PRINT '> Categories seeded.';

    -- 2. Seed Users (Admin, TourManager, TourGuide, Customer)
    PRINT 'Seeding Users...';
    INSERT INTO [dbo].[Users] ([FullName], [Email], [PhoneNumber], [Address], [IsActive]) VALUES
    (N'Administrator', N'admin@tourest.com', N'0123456789', N'1 Admin Way', 1),        -- Assumed UserID = 1
    (N'Quản Lý Viên', N'manager@tourest.com', N'0987654321', N'2 Management Plaza', 1), -- Assumed UserID = 2
    (N'Hướng Dẫn Viên A', N'guide.a@tourest.com', N'0911223344', N'3 Guide Street', 1), -- Assumed UserID = 3
    (N'Khách Hàng Vip', N'customer.vip@email.com', N'0905000111', N'4 Customer Avenue', 1); -- Assumed UserID = 4
    PRINT '> Users seeded.';

    -- 3. Seed Accounts (Corresponding to Users)
    PRINT 'Seeding Accounts...';
    INSERT INTO [dbo].[Accounts] ([UserID], [Username], [PasswordHash], [Role]) VALUES
    (1, N'admin@tourest.com', N'', N'Admin'),       -- UserID 1, WARNING: Empty PasswordHash!
    (2, N'manager@tourest.com', N'', N'TourManager'), -- UserID 2, WARNING: Empty PasswordHash!
    (3, N'guide.a@tourest.com', N'', N'TourGuide'),   -- UserID 3, WARNING: Empty PasswordHash!
    (4, N'customer.vip@email.com', N'', N'Customer'); -- UserID 4, WARNING: Empty PasswordHash!
    PRINT '> Accounts seeded. REMEMBER TO UPDATE PASSWORDS SECURELY!';

    -- 4. Seed TourGuides Profile (For the TourGuide User)
    PRINT 'Seeding TourGuides Profile...';
    INSERT INTO [dbo].[TourGuides] ([TourGuideUserID], [ExperienceLevel], [LanguagesSpoken], [Specializations], [MaxGroupSizeCapacity], [AverageRating]) VALUES
    (3, N'3 năm', N'Tiếng Việt;Tiếng Anh', N'Văn hóa;Ẩm thực', 30, NULL); -- Links to UserID 3
    PRINT '> TourGuides profile seeded.';

    -- 5. Seed Tours
    PRINT 'Seeding Tours...';
    INSERT INTO [dbo].[Tours]
        ([Name], [Destination], [Description], [DurationDays], [DurationNights], [AdultPrice], [ChildPrice], [MinGroupSize], [MaxGroupSize], [DeparturePoints], [IncludedServices], [ExcludedServices], [ImageUrls], [Status], [IsCancellable], [CancellationPolicyDescription])
    VALUES
        (N'Khám phá Đà Nẵng - Hội An 3N2Đ', N'Đà Nẵng - Hội An', N'Tham quan Cầu Vàng, Bà Nà Hills, tắm biển Mỹ Khê, dạo chơi phố cổ Hội An lung linh về đêm.', 3, 2, 3500000, 2800000, 10, 40, N'Sân bay Đà Nẵng;Trung tâm Đà Nẵng', N'Xe đưa đón, Khách sạn 3*, Ăn sáng, Vé Bà Nà, Vé Hội An', N'Vé máy bay, Ăn trưa/tối tự túc, Chi phí cá nhân', N'/images/tours/dnha1.jpg;/images/tours/dnha2.jpg', 'Active', 1, N'Hủy trước 7 ngày hoàn 100%, trước 3 ngày hoàn 50%'), -- Assumed TourID = 1
        (N'Chinh phục Fansipan 2N1Đ', N'Sapa - Lào Cai', N'Hành trình leo núi hoặc đi cáp treo lên đỉnh Fansipan, ngắm cảnh núi rừng Tây Bắc hùng vĩ.', 2, 1, 3200000, 2500000, 8, 25, N'Trung tâm Sapa', N'Xe di chuyển tại Sapa, Chỗ nghỉ, Porter (nếu leo bộ), Vé cáp treo (nếu đi cáp), Các bữa ăn trong lịch trình', N'Xe giường nằm HN-Sapa, Chi phí cá nhân, Đồ uống', N'/images/tours/fs1.jpg', 'Active', 1, N'Hủy trước 10 ngày hoàn 100%, trước 5 ngày hoàn 70%'), -- Assumed TourID = 2
        (N'Du Lịch Văn Hóa Hà Nội 1 Ngày', N'Hà Nội', N'Tham quan các di tích lịch sử nổi tiếng: Lăng Bác, Văn Miếu, Hồ Gươm, Phố Cổ. Thưởng thức ẩm thực đặc trưng.', 1, 0, 850000, 600000, 5, 35, N'Nhà hát Lớn Hà Nội', N'Xe du lịch, Hướng dẫn viên, Vé tham quan các điểm, Ăn trưa', N'Đồ uống, Chi phí cá nhân', N'/images/tours/hn1.jpg', 'Active', 0, NULL); -- Assumed TourID = 3
    PRINT '> Tours seeded.';

    -- 6. Seed TourCategories (Link Tours to Categories - Using assumed IDs)
    PRINT 'Seeding TourCategories...';
    INSERT INTO [dbo].[TourCategories] ([TourID], [CategoryID]) VALUES
    (1, 1), -- Tour 1 -> Category 1
    (1, 4), -- Tour 1 -> Category 4
    (2, 2), -- Tour 2 -> Category 2
    (3, 3), -- Tour 3 -> Category 3
    (3, 4), -- Tour 3 -> Category 4
    (3, 5); -- Tour 3 -> Category 5
    PRINT '> TourCategories seeded.';

    -- 7. Seed ItineraryDays (Using assumed TourIDs)
    PRINT 'Seeding ItineraryDays...';
    INSERT INTO [dbo].[ItineraryDays] ([TourID], [DayNumber], [Title], [Description], [Order]) VALUES
    (1, 1, N'Ngày 1: Đà Nẵng - Ngũ Hành Sơn - Hội An', N'Sáng: Đón khách, tham quan Ngũ Hành Sơn. Trưa: Ăn trưa. Chiều: Di chuyển vào Hội An, nhận phòng. Tối: Dạo chơi, ăn tối Phố cổ.', 1),
    (1, 2, N'Ngày 2: Hội An - Bà Nà Hills', N'Sáng: Trả phòng, đi Bà Nà Hills. Trưa: Ăn buffet Bà Nà. Chiều: Vui chơi, Cầu Vàng. Tối: Về Đà Nẵng, nhận phòng, ăn tối.', 2),
    (1, 3, N'Ngày 3: Đà Nẵng - Tiễn khách', N'Sáng: Tự do tắm biển, mua sắm. Trưa: Trả phòng. Chiều: Tiễn sân bay.', 3),
    (2, 1, N'Ngày 1: Sapa - Chinh phục Fansipan', N'Sáng: Gặp HDV/Porter, chuẩn bị. Bắt đầu leo núi/đi cáp treo. Trưa: Ăn trưa. Chiều: Tiếp tục hành trình/Lên đỉnh. Tối: Nghỉ đêm/về Sapa.', 1),
    (2, 2, N'Ngày 2: Đỉnh Fansipan - Sapa', N'Sáng: Ngắm bình minh, đỉnh Fansipan. Chụp ảnh. Xuống núi. Trưa: Ăn trưa Sapa. Chiều: Kết thúc.', 2),
    (3, 1, N'Ngày 1: Khám phá Hà Nội Ngàn Năm Văn Hiến', N'Sáng: Viếng Lăng Bác, Chùa Một Cột, Văn Miếu. Trưa: Ăn trưa bún chả/phở. Chiều: Hồ Gươm, Đền Ngọc Sơn, Phố Cổ. Kết thúc.', 1);
    PRINT '> ItineraryDays seeded.';

    -- 8. Seed Bookings (Using assumed CustomerID=4 and TourIDs=1, 2)
    PRINT 'Seeding Bookings...';
    INSERT INTO [dbo].[Bookings]
        ([BookingDate], [DepartureDate], [NumberOfAdults], [NumberOfChildren], [TotalPrice], [Status], [PickupPoint], [CustomerID], [TourID], [TourGroupID], [PaymentID], [CancellationDate], [RefundAmount])
    VALUES
        ('2025-04-10 10:00:00', '2025-06-20', 2, 1, 9800000, 'Confirmed', N'Khách sạn Novotel Đà Nẵng', 4, 1, NULL, NULL, NULL, NULL), -- Assumed BookingID = 1
        ('2025-04-15 11:30:00', '2025-07-05', 1, 0, 3200000, 'PendingPayment', N'Trung tâm thị trấn Sapa', 4, 2, NULL, NULL, NULL, NULL), -- Assumed BookingID = 2
        ('2025-03-01 15:00:00', '2025-03-25', 2, 0, 7000000, 'Completed', N'Sân bay Đà Nẵng', 4, 1, NULL, NULL, NULL, NULL), -- Assumed BookingID = 3
        ('2025-04-01 08:00:00', '2025-06-10', 4, 0, 12800000, 'Cancelled', N'Ga Lào Cai', 4, 2, NULL, NULL, '2025-05-20 10:00:00', 8960000), -- Assumed BookingID = 4
        ('2025-04-12 16:45:00', '2025-06-20', 1, 1, 6300000, 'Confirmed', N'Khách sạn Mường Thanh Đà Nẵng', 4, 1, NULL, NULL, NULL, NULL); -- Assumed BookingID = 5
    PRINT '> Bookings seeded.';

    -- 9. Seed Payments (Link to Bookings - Using assumed BookingIDs)
    PRINT 'Seeding Payments...';
    INSERT INTO [dbo].[Payments] ([BookingID], [Amount], [PaymentDate], [PaymentMethod], [TransactionID], [Status]) VALUES
    (1, 9800000, '2025-04-10 10:05:00', 'VNPay', 'VNP100425001', 'Success'), -- Assumed PaymentID = 1
    (3, 7000000, '2025-03-01 15:05:00', 'CreditCard', 'CC987654321', 'Success'), -- Assumed PaymentID = 2
    (5, 6300000, '2025-04-12 16:50:00', 'Momo', 'MOMO888777666', 'Success'); -- Assumed PaymentID = 3
    PRINT '> Payments seeded.';

    -- Update Bookings with PaymentID
    PRINT 'Updating Bookings with PaymentIDs...';
    UPDATE [dbo].[Bookings] SET [PaymentID] = 1 WHERE [BookingID] = 1;
    UPDATE [dbo].[Bookings] SET [PaymentID] = 2 WHERE [BookingID] = 3;
    UPDATE [dbo].[Bookings] SET [PaymentID] = 3 WHERE [BookingID] = 5;
    PRINT '> Bookings updated with PaymentIDs.';

    -- 10. Seed TourGroups (Using assumed TourID=1 and matching DepartureDate from Bookings 1 & 5)
    PRINT 'Seeding TourGroups...';
    INSERT INTO [dbo].[TourGroups] ([TourID], [DepartureDate], [TotalGuests], [Status], [AssignedTourGuideID]) VALUES
    (1, '2025-06-20', 5, 'PendingAssignment', NULL); -- Assumed TourGroupID = 1
    PRINT '> TourGroups seeded.';

    -- Update relevant Bookings with TourGroupID
    PRINT 'Updating Bookings with TourGroupIDs...';
    UPDATE [dbo].[Bookings] SET [TourGroupID] = 1 WHERE [BookingID] IN (1, 5);
    PRINT '> Bookings updated with TourGroupIDs.';

    -- Assign Guide to Group and Update Status
    PRINT 'Assigning Guide to TourGroup...';
    UPDATE [dbo].[TourGroups] SET [AssignedTourGuideID] = 3, [Status] = 'Assigned' WHERE [TourGroupID] = 1; -- Assign Guide UserID=3
    PRINT '> TourGroup assigned.';

    -- 11. Seed TourGuideAssignments (Record the assignment - Using assumed IDs)
    PRINT 'Seeding TourGuideAssignments...';
    INSERT INTO [dbo].[TourGuideAssignments] ([TourGroupID], [TourGuideID], [TourManagerID], [AssignmentDate], [Status], [ConfirmationDate]) VALUES
    (1, 3, 2, '2025-06-05 14:00:00', 'Accepted', '2025-06-05 17:00:00'); -- Assumed AssignmentID = 1
    PRINT '> TourGuideAssignments seeded.';

    -- 12. Seed Ratings (Using assumed CustomerID=4, TourID=1, TourGroupID=1, GuideID=3)
    PRINT 'Seeding Ratings...';
    INSERT INTO [dbo].[Ratings] ([CustomerID], [RatingValue], [Comment], [RatingDate], [RatingType]) VALUES
    (4, 4.50, N'Chuyến đi Đà Nẵng Hội An (tháng 3) rất vui, khách sạn ổn.', '2025-03-29 10:00:00', 'Tour'), -- Assumed RatingID = 1
    (4, 4.00, N'Hướng dẫn viên đoàn 25/03 khá am hiểu nhưng chưa sôi nổi.', '2025-03-29 10:05:00', 'TourGuide'), -- Assumed RatingID = 2
    (4, 5.00, N'Tour đi Đà Nẵng ngày 20/06 tuyệt vời!', '2025-06-23 11:00:00', 'Tour'), -- Assumed RatingID = 3
    (4, 5.00, N'Anh HDV đoàn 20/06 (ID=3) siêu nhiệt tình.', '2025-06-23 11:05:00', 'TourGuide'); -- Assumed RatingID = 4
    PRINT '> Ratings seeded.';

    -- 13. Seed TourRatings (Link Ratings to Tours)
    PRINT 'Seeding TourRatings...';
    INSERT INTO [dbo].[TourRatings] ([RatingID], [TourID]) VALUES
    (1, 1), -- Link Rating 1 to Tour 1
    (3, 1); -- Link Rating 3 to Tour 1
    PRINT '> TourRatings seeded.';

    -- 14. Seed TourGuideRatings (Link Ratings to Guides and Groups)
    PRINT 'Seeding TourGuideRatings...';
    INSERT INTO [dbo].[TourGuideRatings] ([RatingID], [TourGuideID], [TourGroupID]) VALUES
    (4, 3, 1); -- Link Rating 4 to Guide 3 for Group 1
    PRINT '> TourGuideRatings seeded.';

    -- 15. Seed TourAuditLogs (Example log - Using assumed IDs)
    PRINT 'Seeding TourAuditLogs...';
    INSERT INTO [dbo].[TourAuditLogs] ([TourID], [ActionType], [PerformedByUserID], [Timestamp], [OldValues], [NewValues]) VALUES
    (1, 'UPDATE', 2, '2025-04-17 09:30:00',
     N'{"Name":"Tour Đà Nẵng - Hội An Cũ","AdultPrice":3400000,"IsCancellable":false}',
     N'{"Name":"Khám phá Đà Nẵng - Hội An 3N2Đ","AdultPrice":3500000,"IsCancellable":true}'); -- Assumed LogID=1
    PRINT '> TourAuditLogs seeded.';

    -- 16. Seed Notifications (Using assumed IDs)
    PRINT 'Seeding Notifications...';
    INSERT INTO [dbo].[Notifications] ([RecipientUserID], [SenderUserID], [Type], [Title], [Content], [RelatedEntityID], [RelatedEntityType], [IsRead], [ActionUrl]) VALUES
    (4, NULL, 'BookingConfirmation', N'Xác nhận đặt tour #1 thành công', N'Đơn đặt tour Đà Nẵng - Hội An khởi hành 20/06/2025 của bạn đã được xác nhận và thanh toán.', '1', 'Booking', 0, '/Bookings/Details/1'),
    (3, 2, 'TourGuideAssignmentRequest', N'Yêu cầu dẫn đoàn #1', N'Bạn được yêu cầu dẫn đoàn #1 đi Đà Nẵng - Hội An ngày 20/06/2025. Vui lòng xác nhận.', '1', 'TourGuideAssignment', 0, '/TourGuide/Assignments/Details/1'),
    (4, NULL, 'TourGuideAssigned', N'HDV cho đoàn 20/06/2025', N'Hướng dẫn viên Hướng Dẫn Viên A (SĐT: 0911223344) đã được chỉ định cho đoàn của bạn.', '1', 'TourGroup', 0, NULL);
    PRINT '> Notifications seeded.';

    -- 17. Seed SupportRequests (Using assumed UserIDs)
    PRINT 'Seeding SupportRequests...';
    INSERT INTO [dbo].[SupportRequests] ([CustomerID], [Subject], [Message], [SubmissionDate], [Status], [HandlerUserID], [ResolutionNotes]) VALUES
    (4, N'Hỏi về điểm đón Tour Fansipan 05/07', N'Tôi đặt tour Fansipan ngày 05/07 (Booking ID 2), vui lòng cho biết chính xác giờ và địa điểm đón tại Sapa?', '2025-04-18 10:00:00', 'Open', NULL, NULL),
    (4, N'Vấn đề thanh toán Booking #3', N'Tôi thấy giao dịch thẻ của tôi báo thành công nhưng trạng thái booking chưa cập nhật?', '2025-03-02 09:00:00', 'Resolved', 1, N'Admin đã kiểm tra và xác nhận thanh toán thành công, cập nhật trạng thái booking thủ công.');
    PRINT '> SupportRequests seeded.';


    COMMIT TRANSACTION; -- Commit all changes if everything successful
    PRINT '================================================';
    PRINT 'Seed Data Insertion Completed Successfully.';
    PRINT '================================================';

END TRY
BEGIN CATCH
    -- If any error occurs, rollback the whole transaction
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT '================================================';
    PRINT 'Error occurred during Seed Data Insertion. Transaction rolled back.';
    -- Output detailed error information
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS VARCHAR(10));
    PRINT 'Error Severity: ' + CAST(ERROR_SEVERITY() AS VARCHAR(10));
    PRINT 'Error State: ' + CAST(ERROR_STATE() AS VARCHAR(10));
    PRINT 'Error Procedure: ' + ISNULL(ERROR_PROCEDURE(), 'N/A');
    PRINT 'Error Line: ' + CAST(ERROR_LINE() AS VARCHAR(10));
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT '================================================';
    -- Throw the error again to indicate failure to the calling process/tool
    THROW;
END CATCH;

