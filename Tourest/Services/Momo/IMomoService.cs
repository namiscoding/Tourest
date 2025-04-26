using Tourest.Data.Entities.Momo;
using Tourest.Data.Entities;
namespace Tourest.Services.Momo
{
    public interface IMomoService
    {
        public Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfoModel model);
        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
