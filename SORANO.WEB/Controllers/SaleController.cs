using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.Sale;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    [CheckUser]
    public class SaleController : EntityBaseController<SaleCreateUpdateViewModel>
    {
        private readonly ISaleService _saleService;
        private readonly IMapper _mapper;

        public SaleController(ISaleService saleService,
            IMapper mapper,
            IUserService userService, 
            IHostingEnvironment environment, 
            IAttachmentTypeService attachmentTypeService, 
            IAttachmentService attachmentService, 
            IMemoryCache memorycache) : base(userService, 
                environment, 
                attachmentTypeService, 
                attachmentService, 
                memorycache)
        {
            _saleService = saleService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await TryGetActionResultAsync(async () =>
            {
                var showDeleted = Session.GetBool("ShowDeletedSales");

                var salesResult = await _saleService.GetAllAsync(showDeleted);

                if (salesResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список продаж.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ShowDeleted = showDeleted;

                await ClearAttachments();

                var viewModel = _mapper.Map<SaleIndexViewModel>(salesResult.Result);
                viewModel.Mode = SaleTableMode.SaleIndex;

                return View(viewModel);
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Home");
            });
        }

        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedSales", show);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return await TryGetActionResultAsync(async () =>
            {
                SaleCreateUpdateViewModel model;

                if (TryGetCached(out var cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else if (TryGetCached(out var cachedForCreateSaleItem, CacheKeys.CreateSaleItemCacheKey, CacheKeys.CreateSaleItemCacheValidKey))
                {
                    model = cachedForCreateSaleItem;
                }
                else
                {
                    model = new SaleCreateUpdateViewModel
                    {
                        MainPicture = new MainPictureViewModel()
                    };
                }

                return View(model);
            }, OnFault);
        }

        #endregion

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index");
        }
    }
}