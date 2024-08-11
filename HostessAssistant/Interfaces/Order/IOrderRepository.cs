using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static ElinaTestProject.Models.Order.OrderRepository;

namespace ElinaTestProject.Interfaces.Order
{
    public interface IOrderRepository
    {
        Task<IActionResult> GetOrderAsync(int id);
        Task<IActionResult> UpdateOrderAsync(Order_dto order);
        Task<IActionResult> CreateOrderAsync(Order_dto order);
        Task<IActionResult> FindOrderAsync(OrderFinder finder);
        Task<IActionResult> CloseOrderAsync(int id);
    }
}
