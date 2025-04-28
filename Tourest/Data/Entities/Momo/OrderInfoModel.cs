namespace Tourest.Data.Entities.Momo
{
    public class OrderInfoModel
    {
        public string OrderId { get; set; } // Sẽ là uniqueAttemptId
        public string RequestId { get; set; } // Sẽ là uniqueAttemptId
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
        public string FullName { get; set; }
        public string ExtraData { get; set; } // Thêm trường này
    }
}
