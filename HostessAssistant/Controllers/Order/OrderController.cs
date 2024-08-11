
using ElinaTestProject.Interfaces.Order;
using ElinaTestProject.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ElinaTestProject.Models.Order.OrderRepository;

namespace ElinaTestProject.Controllers.Order
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(typeof(Order_dto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetOrder(int id)
        {
            return await _orderRepository.GetOrderAsync(id);
        }

        /// <summary>
        /// Add or update order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(Order_dto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Apply([FromBody] Order_dto order)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ControllerUtils.GetModelErrors(ModelState));

            if (order.Id.HasValue)
                return await _orderRepository.UpdateOrderAsync(order).ConfigureAwait(false);
            else
                return await _orderRepository.CreateOrderAsync(order).ConfigureAwait(false);
        }

        /// <summary>
        /// Find order by params
        /// </summary>
        /// <param name="finder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<Order_dto>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Find([FromBody] OrderFinder finder)
        {
            return await _orderRepository.FindOrderAsync(finder).ConfigureAwait(false);
        }
        /// <summary>
        /// Close order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Close(int id)
        {
            return await _orderRepository.CloseOrderAsync(id).ConfigureAwait(false);
        }
    }
}

