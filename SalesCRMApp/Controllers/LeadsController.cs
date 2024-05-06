
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using SalesCRMApp.Data;
using SalesCRMApp.Models;

namespace SalesCRMApp.Controllers
{
    [Authorize]
    
    //controllerin veri tabanına erişimini sağlar, itekleri alır işler ve sonucları olusturur
    public class LeadsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeadsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Leads
        //"Index" sayfasını göstermek için gerekli olan satış potansiyeli verilerini alır ve
        //bu verileri View'a gönderir. 
        public async Task<IActionResult> Index()
        {
            return View(await _context.SalesLead.ToListAsync());
        }




        // GET: Leads/Details/5
        //details adında bir metodu tanımlar
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //id değeri null değilse, SalesLead koleksiyonundan veritabanında belirtilen id ile eşleşen ilk satış
            //potansiyelini bulmak için LINQ sorgusu kullanılır. Bu işlem FirstOrDefaultAsync metodu ile asenkron
            //olarak gerçekleştirilir.
            var salesLeadEntity = await _context.SalesLead
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesLeadEntity == null)
            {
                return NotFound();
            }

            //id ile eşleşen satıs potansiyolı bulunursa ayrıntıları ıceren vıew'e yonlendırılır
            // Yönlendirme, View() metodu ile gerçekleştirilir ve salesLeadEntity değişkeni View'a model olarak iletilir.
            return View(salesLeadEntity);
        } //bu metot belirtilen id ile eşleşen bir satış potansiyelinin ayrıntılarını görüntülemek için kullanılır.





        // GET: Leads/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leads/Create
        [HttpPost] 
        [ValidateAntiForgeryToken] //CSRF (Cross-Site Request Forgery) saldırılarına karşı koruma sağlar. 
        //SalesLeadEntity türünden bir parametre alır, bu parametre yeni oluşturulacak satış potansiyeli verilerini içerir.
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Mobile,Email,Source")] SalesLeadEntity salesLeadEntity)
        {
            if (ModelState.IsValid) //ModelState.IsValid özelliği, gelen modelin geçerli olup olmadığını kontrol eder. Modelin doğrulanması gereken
                                    //belirli gereksinimlere uygun olup olmadığını kontrol eder.
            {
                _context.Add(salesLeadEntity); //model gecerli ise salesLeadEntity ogesini context icindeki salesLead koleksyonuna ekler
                await _context.SaveChangesAsync(); //değişiklikleri veri tabanına kaydeder
                return RedirectToAction(nameof(Index)); //index sayfasına yonlendırır
            }
            return View(salesLeadEntity); //model gecerli degilse yanı gereklı alanlar dogrulanmamıssa veya gereksınımler karsılanmamıssa
                                          //işlemi geri alarak kullanıcıya aynı sayfayı gosterır. bu kullanıcının hataları duzdltebılmesını saglar
        } //Bu şekilde, bu metot yeni bir satış potansiyeli oluşturur ve kullanıcının girdiği verileri veritabanına kaydeder.




        // GET: Leads/Edit/5
        //Edit adında bir metot tanımlanır. Bu metot, int? id adında bir parametre alır.
        //Bu parametre, düzenlemek istediğimiz satış potansiyelinin kimliğini temsil eder. 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Eğer id değeri null değilse, belirtilen id ile eşleşen satış potansiyelini veritabanından bulmak için _context.SalesLead.FindAsync(id) kullanılır.
            //Bu işlem asenkron olarak gerçekleştirilir.
            var salesLeadEntity = await _context.SalesLead.FindAsync(id);
            if (salesLeadEntity == null)
            {
                return NotFound();
            }
            return View(salesLeadEntity); //Eğer belirtilen id ile eşleşen bir satış potansiyeli bulunursa, bu satış potansiyelinin düzenleme sayfasına yönlendirilir.
                                          //yonlerndırme view() motodu ile gerceklestirilir
        } // bu metot belirtilen id ile eşleşen bir satış potansiyelinin düzenleme sayfasını görüntülemek için kullanılır.




        // POST: Leads/Edit/5
      
        [HttpPost] //satıs potansıyelının duzenleme işlemi
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Mobile,Email,Source")] SalesLeadEntity salesLeadEntity)
        {
            if (id != salesLeadEntity.Id)
            {
                return NotFound();
            }
            //Eğer model geçerli ise, try bloğu içinde _context.Update(salesLeadEntity) ile güncellenmiş satış potansiyelini
            //veritabanına kaydeder ve await _context.SaveChangesAsync() ile kaydedilen değişiklikleri veritabanına yansıtır.
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesLeadEntity);
                    await _context.SaveChangesAsync();
                }

                //Eğer aynı anda birden fazla kullanıcı aynı satış potansiyelini güncellemeye çalışırsa ve veri tabanında aynı anda
                //iki farklı değişiklik yapıldıysa, DbUpdateConcurrencyException istisnası oluşabilir. Bu durumda catch bloğu içinde kontrol edilir.
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesLeadEntityExists(salesLeadEntity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(salesLeadEntity);
        } //Bu şekilde, bu metot belirtilen id ile eşleşen bir satış potansiyelinin güncellenmesi işlemini gerçekleştirir.




        // GET: Leads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesLeadEntity = await _context.SalesLead
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesLeadEntity == null)
            {
                return NotFound();
            }

            return View(salesLeadEntity);
        }//Bu şekilde, bu metot belirtilen id ile eşleşen bir satış potansiyelinin silinmesi işlemi için onay sayfasını görüntülemek için kullanılır.



        // POST: Leads/Delete/5
        [HttpPost, ActionName("Delete")] //, bu metodu Delete adı yerine DeleteConfirmed olarak adlandırılmasını sağlar. Bu, bir ASP.NET Core MVC özelliğidir ve
                                         //birçok kullanıcı tarafından bir satış potansiyeli
                                         //silme işlemi onaylandığında çalıştırılacak kodu temsil eder.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salesLeadEntity = await _context.SalesLead.FindAsync(id);
            if (salesLeadEntity != null)
            {
                _context.SalesLead.Remove(salesLeadEntity); //Eğer belirtilen id ile eşleşen bir satış potansiyeli bulunursa,
                                                            //bu satış potansiyeli veritabanından kaldırılır 
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesLeadEntityExists(int id)
        {
            return _context.SalesLead.Any(e => e.Id == id);
        } //Bu kod, satış potansiyeli silme işleminin gerçekleşmesi için gereken aksiyonları içerir.
          //Ayrıca, bir satış potansiyelinin belirli bir kimliğe sahip olup olmadığını kontrol eden SalesLeadEntityExists metodu da tanımlanmıştır.
          //Bu metod, id değeriyle veritabanında satış potansiyelinin varlığını kontrol eder.

    }
}
