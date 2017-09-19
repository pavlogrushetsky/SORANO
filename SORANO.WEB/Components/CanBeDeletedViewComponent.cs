using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.Components
{
    public class CanBeDeletedViewComponent : ViewComponent
    {
#pragma warning disable 1998
        public async Task<IViewComponentResult> InvokeAsync(bool canBeDeleted, string warning, string error)
#pragma warning restore 1998
        {
            return View(new CanBeDeletedViewModel
            {
                CanBeDeleted = canBeDeleted,
                Warning = warning,
                Error = error
            });
        }
    }
}