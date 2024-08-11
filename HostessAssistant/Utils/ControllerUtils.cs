using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace ElinaTestProject.Utils
{
    public static class ControllerUtils
    {
        public static string GetModelErrors(this ModelStateDictionary modelState)
        {
            string allErrors = string.Join("; ", modelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)));
            return allErrors;
        }
    }
}
