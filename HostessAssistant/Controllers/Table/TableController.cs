using ElinaTestProject.Interfaces.Table;
using ElinaTestProject.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ElinaTestProject.Models.Table.TableRepository;

namespace ElinaTestProject.Controllers.Table
{
    [Route("api/[controller]")]
    public class TableController : Controller
    {
        private readonly ITableRepository _tableRepository;

        public TableController(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        /// <summary>
        /// Get table from data base by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(typeof(Table_dto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetTable(int id)
        {
            return await _tableRepository.GetTableAsync(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Add or update table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(Table_dto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Apply([FromBody] Table_dto table)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ControllerUtils.GetModelErrors(ModelState));

            if(table.Id.HasValue)
                return await _tableRepository.UpdateTableAsync(table).ConfigureAwait(false);
            else
                return await _tableRepository.AddTableAsync(table).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove table from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Delete(int id)
        {
            return await _tableRepository.DeleteTableAsync(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Find table by params
        /// </summary>
        /// <param name="finder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<Table_dto>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Find([FromBody] TableFinder finder)
        {
            return await _tableRepository.FindTableAsync(finder).ConfigureAwait(false);
        }
    }
}
