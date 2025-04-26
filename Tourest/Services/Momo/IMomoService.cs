using Tourest.Data.Entities.Momo;
using Tourest.Data.Entities;
using Tourest.ViewModels.Booking;
namespace Tourest.Services.Momo
{
    public interface IMomoService
    {
        public Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfoModel model);
        //public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
        bool ValidateReturnUrlSignature(MomoCallbackViewModel callbackData); // Đổi tên để rõ ràng hơn
        bool ValidateIpnSignature(MomoCallbackViewModel ipnData); // Phương thức mới cho IPN

    }
}
