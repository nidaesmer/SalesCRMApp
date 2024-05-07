using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace SalesCRMApp.Controllers
{
    public class AppRolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppRolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        //list all the roles created by users/kullanıcılar taradından olusturulan tum kurallar listesi
        public IActionResult Index() //indexe view ekleyıp ındex.cshtml sayfasında roles sayfasını olusturduk
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create() // add view diyerek create sayfasının tetıklenmesını saglayacagız, sayfa olusturcaz
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult()) //mevcut bir rolün olup olmadığı kontrol ediliyor
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
                //Eğer model.Name ile belirtilen isimde bir rol yoksa yeni bir rol oluşturulur.
            }
            return RedirectToAction("Index"); //eğer rol başarıyla oluşturulursa kullanıcıyı rol yönetimi ana sayfasına yönlendirilir.
        }//Bu kod, bir HTTP POST isteği aldığında gelen verilerle bir rol oluşturmayı işler.
    }
}
