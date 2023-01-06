using Orders.DAL.Entities;

namespace JobLessonASPNETMVCCore08v01.Services
{
    public interface IOrderService
    {
        Task<Order> CreateAsync(int buyerId, string address, string phone, IEnumerable<(int productId, int quantity)> products);
    }
}
