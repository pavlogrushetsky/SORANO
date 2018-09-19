using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SORANO.WEB.ViewModels.Visit;

namespace SORANO.WEB.Components
{
    public class VisitFormViewComponent : ViewComponent
    {
#pragma warning disable 1998
        public async Task<IViewComponentResult> InvokeAsync()
#pragma warning restore 1998
        {
            var model = new VisitCreateViewModel
            {
                Code = "мж2",
                Date = $"{DateTime.Now}",
                LocationID = 0,
                LocationName = string.Empty
            };

            return View(model);
        }
    }
}