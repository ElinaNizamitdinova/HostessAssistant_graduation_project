using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static ElinaTestProject.Models.Table.TableRepository;

namespace ElinaTestProject.Interfaces.Table
{
    /// <summary>
    /// Table repository interface
    /// </summary>
    public interface ITableRepository
    {
        /// <summary>
        /// Get table by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IActionResult> GetTableAsync(int id);
        /// <summary>
        /// Add new table to database
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        Task<IActionResult> AddTableAsync(Table_dto table);
        /// <summary>
        /// Upadte existing table in database
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        Task<IActionResult> UpdateTableAsync(Table_dto table);
        /// <summary>
        /// Remove existing table from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IActionResult> DeleteTableAsync(int id);
        /// <summary>
        /// Find table by params
        /// </summary>
        /// <param name="finder"></param>
        /// <returns></returns>
        Task<IActionResult> FindTableAsync(TableFinder finder);
    }
}
