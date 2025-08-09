using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mayka.Models;
using mayka.ViewModels;
using mayka.Services;
using mayka.Helpers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using ImageMagick;
using System.Security.Cryptography;

namespace mayka.Controllers
{
    public class MainController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISession session;
        private IHostingEnvironment _env;

        public MainController(AppDbContext context, IHttpContextAccessor httpContextAccessor, IHostingEnvironment env)
        {
            _context = context;
            session = httpContextAccessor.HttpContext.Session;
            _env = env;
        }

        public async Task<IActionResult> Index(int? catId, string type, int page = 1)
        {
            var products = await _context.Products.Where(x => x.Public == true).OrderByDescending(x => x.Id).ToListAsync();
            ViewBag.Category = "Все";
            ViewBag.CategoryId = 0;
            ViewBag.Type = "Все";

            if (type != null && type.Length > 0 && type != "Все")
            {
                products = products.Where(x => x.Type == type).ToList();
                ViewBag.Type = type;
            }

            if (catId != null)
            {
                if (catId != 0)
                {
                    products = products.Where(x => x.CategoryId == catId).ToList();
                    var mCategory = await _context.Categories.FirstOrDefaultAsync(x => x.Id == catId);
                    ViewBag.Category = mCategory.Title;
                    ViewBag.CategoryId = mCategory.Id;
                }
            }

            //Постраничный вывод
            List<MProduct> productsFiltered = new List<MProduct>();

            ViewBag.PageSize = 10;
            ViewBag.Page = page;
            ViewBag.ProductsCount = products.Count;
            int begin = ViewBag.PageSize * (page - 1);
            int end = begin + ViewBag.PageSize;
            for (int i = begin; i < end; i++)
            {
                if (products.Count > i)
                    productsFiltered.Add(products[i]);
            }
            PagesPagination pPagination = new PagesPagination();
            var pagesTotal = Math.Ceiling(products.Count / (decimal)ViewBag.PageSize);
            int[] pageArr = pPagination.ShowPages(page, pagesTotal);
            ViewBag.PagesBegin = pageArr[0];
            ViewBag.PagesEnd = pageArr[1];

            var categories = await _context.Categories.ToListAsync();
            string pathServer = Path.Combine(_env.WebRootPath, "Images");

            var cpVM = new CategoriesProductsVM { Categories = categories, Products = productsFiltered };
            ViewData["TypeList"] = new SelectList(new string[] { "Все", "Мужская футболка", "Женская футболка", "Кружка", "Комплект" }, ViewBag.Type);
            return View(cpVM);
        }


        public async Task<IActionResult> Search(string searchString)
        {
            var products = await _context.Products.Where(x => x.Public == true).OrderByDescending(x => x.Id).ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(x => x.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else
            {
                products = null;
            }
            ViewBag.SearсhString = searchString;
            return View(products);
        }

        // Редактор мужских футболок
        public IActionResult Redactor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Redactor(string imageData, string imageDataBack, string[] imagesAll, string size2)
        {
            if (imageData != null)
            {


                //Сохранение основной фотографии

                imageData = imageData.Substring(imageData.IndexOf(',') + 1);
                byte[] image = Convert.FromBase64String(imageData);

                var mConsLocal = new MConstructor();
                mConsLocal.FullImg = image;

                imageDataBack = imageDataBack.Substring(imageDataBack.IndexOf(',') + 1);
                byte[] imageBack = Convert.FromBase64String(imageDataBack);
                mConsLocal.FullImgBack = imageBack;

                await _context.Constructors.AddAsync(mConsLocal);

                await _context.SaveChangesAsync();

                //Сохранение фотографии в файловой системе
                string pathServer = Path.Combine(_env.WebRootPath, "Images");
                if (!Directory.Exists(pathServer))
                {
                    Directory.CreateDirectory(pathServer);
                }
                string imageName = DateTime.Now.Day + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Hour + ".png";
                string imagePath = Path.Combine(pathServer, imageName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    fileStream.Write(image, 0, image.Length);
                }

                string imageName2 = DateTime.Now.Day + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Hour + "Back.png";
                string imagePath2 = Path.Combine(pathServer, imageName2);
                using (var fileStream = new FileStream(imagePath2, FileMode.Create))
                {
                    fileStream.Write(imageBack, 0, imageBack.Length);
                }

                //Сохранение остальных фотографий

                var consNow = await _context.Constructors.FirstOrDefaultAsync(x => x.FullImg == image);

                foreach (var item in imagesAll)
                {
                    string imgStr = item.Substring(item.IndexOf(',') + 1);
                    byte[] imageLocal = Convert.FromBase64String(imgStr);
                    var mPhoto = new MPhoto();
                    mPhoto.Img = imageLocal;
                    mPhoto.ConstructorId = consNow.Id;

                    await _context.Photos.AddAsync(mPhoto);

                    await _context.SaveChangesAsync();
                }

                //Сохранение в виде продукта
                MProduct mProduct = new MProduct
                {
                    Title = "Свой дизайн",
                    Type = "Мужская футболка",
                    Description = "Футболка со своим дизайном",
                    Price = 3000,
                    Currency = "тг",
                    Img = imageName,
                    ImgBack = imageName2,
                    CategoryId = 1,
                    Public = false,
                    Color = "Белый",
                    Composition = "Сэндвич"
                };

                await _context.Products.AddAsync(mProduct);
                await _context.SaveChangesAsync();

                var mProduct2 = await _context.Products.FirstOrDefaultAsync(x => x.Img == imageName);

                return RedirectToAction("Buy", "Cart", new { id = mProduct2.Id, size = size2 });
            }
            return View();
        }


        //Редактор женских футболок
        public IActionResult RedactorW()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RedactorW(string imageData, string imageDataBack, string[] imagesAll, string size2)
        {
            if (imageData != null)
            {
                //Сохранение основной фотографии

                imageData = imageData.Substring(imageData.IndexOf(',') + 1);
                byte[] image = Convert.FromBase64String(imageData);

                var mConsLocal = new MConstructor();
                mConsLocal.FullImg = image;

                imageDataBack = imageDataBack.Substring(imageDataBack.IndexOf(',') + 1);
                byte[] imageBack = Convert.FromBase64String(imageDataBack);
                mConsLocal.FullImgBack = imageBack;

                await _context.Constructors.AddAsync(mConsLocal);

                await _context.SaveChangesAsync();

                //Сохранение фотографии в файловой системе
                string pathServer = Path.Combine(_env.WebRootPath, "Images");
                if (!Directory.Exists(pathServer))
                {
                    Directory.CreateDirectory(pathServer);
                }
                string imageName = DateTime.Now.Day + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Hour + ".png";
                string imagePath = Path.Combine(pathServer, imageName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    fileStream.Write(image, 0, image.Length);
                }

                string imageName2 = DateTime.Now.Day + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Hour + "Back.png";
                string imagePath2 = Path.Combine(pathServer, imageName2);
                using (var fileStream = new FileStream(imagePath2, FileMode.Create))
                {
                    fileStream.Write(imageBack, 0, imageBack.Length);
                }

                //Сохранение остальных фотографий

                var consNow = await _context.Constructors.FirstOrDefaultAsync(x => x.FullImg == image);

                foreach (var item in imagesAll)
                {
                    string imgStr = item.Substring(item.IndexOf(',') + 1);
                    byte[] imageLocal = Convert.FromBase64String(imgStr);
                    var mPhoto = new MPhoto();
                    mPhoto.Img = imageLocal;
                    mPhoto.ConstructorId = consNow.Id;

                    await _context.Photos.AddAsync(mPhoto);

                    await _context.SaveChangesAsync();
                }

                //Сохранение в виде продукта
                MProduct mProduct = new MProduct
                {
                    Title = "Свой дизайн",
                    Type = "Женская футболка",
                    Description = "Футболка со своим дизайном",
                    Price = 3000,
                    Currency = "тг",
                    Img = imageName,
                    ImgBack = imageName2,
                    CategoryId = 1,
                    Public = false,
                    Color = "Белый",
                    Composition = "Сэндвич"
                };

                await _context.Products.AddAsync(mProduct);
                await _context.SaveChangesAsync();

                var mProduct2 = await _context.Products.FirstOrDefaultAsync(x => x.Img == imageName);

                return RedirectToAction("Buy", "Cart", new { id = mProduct2.Id, size = size2 });
            }
            return View();
        }

        // Редактор черных футболок
        public IActionResult RedactorFlex()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RedactorFlex(string imageData, string imageDataBack, string[] imagesAll, string size2, string color, string price)
        {
            if (imageData != null)
            {


                //Сохранение основной фотографии

                imageData = imageData.Substring(imageData.IndexOf(',') + 1);
                byte[] image = Convert.FromBase64String(imageData);

                var mConsLocal = new MConstructor();
                mConsLocal.FullImg = image;

                imageDataBack = imageDataBack.Substring(imageDataBack.IndexOf(',') + 1);
                byte[] imageBack = Convert.FromBase64String(imageDataBack);
                mConsLocal.FullImgBack = imageBack;

                await _context.Constructors.AddAsync(mConsLocal);

                await _context.SaveChangesAsync();

                //Сохранение фотографии в файловой системе
                string pathServer = Path.Combine(_env.WebRootPath, "Images");
                if (!Directory.Exists(pathServer))
                {
                    Directory.CreateDirectory(pathServer);
                }
                string imageName = DateTime.Now.Day + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Hour + ".png";
                string imagePath = Path.Combine(pathServer, imageName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    fileStream.Write(image, 0, image.Length);
                }

                string imageName2 = DateTime.Now.Day + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Hour + "Back.png";
                string imagePath2 = Path.Combine(pathServer, imageName2);
                using (var fileStream = new FileStream(imagePath2, FileMode.Create))
                {
                    fileStream.Write(imageBack, 0, imageBack.Length);
                }

                //Сохранение остальных фотографий

                var consNow = await _context.Constructors.FirstOrDefaultAsync(x => x.FullImg == image);

                foreach (var item in imagesAll)
                {
                    string imgStr = item.Substring(item.IndexOf(',') + 1);
                    byte[] imageLocal = Convert.FromBase64String(imgStr);
                    var mPhoto = new MPhoto();
                    mPhoto.Img = imageLocal;
                    mPhoto.ConstructorId = consNow.Id;

                    await _context.Photos.AddAsync(mPhoto);

                    await _context.SaveChangesAsync();
                }

                //Сохранение в виде продукта
                MProduct mProduct = new MProduct
                {
                    Title = "Свой дизайн",
                    Type = "Мужская футболка",
                    Description = "Футболка со своим дизайном",
                    Img = imageName,
                    ImgBack = imageName2,
                    CategoryId = 1,
                    Public = false,
                    Color = color,
                    Composition = "Хлопок"
                };

                await _context.Products.AddAsync(mProduct);
                await _context.SaveChangesAsync();

                var mProduct2 = await _context.Products.FirstOrDefaultAsync(x => x.Img == imageName);

                return RedirectToAction("Buy", "Cart", new { id = mProduct2.Id, size = size2 });
            }
            return View();
        }

        // Подробная страница
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mProduct = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
            var otherProducts = await _context.Products.Where(x => x.CategoryId == mProduct.CategoryId && x.Public == true).ToListAsync();

            ViewBag.OtherProducts = otherProducts;

            return View(mProduct);
        }

        // Подробная страница для кружек
        public async Task<IActionResult> DetailsCup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mProduct = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
            var otherProducts = await _context.Products.Where(x => x.CategoryId == mProduct.CategoryId && x.Public == true).ToListAsync();

            ViewBag.OtherProducts = otherProducts;

            return View(mProduct);
        }

        public IActionResult End()
        {
            return View();
        }

        public IActionResult Address()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> End(Item[] cartItem, MClient client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var mClient = await _context.Clients.FirstOrDefaultAsync(x => x.Phone == client.Phone);

            if (cartItem.Count() > 0)
            {
                // Создание заказа
                DateTime dateNow = DateTime.Now;

                MPurchase mPurchase = new MPurchase
                {
                    DatePurchase = dateNow,
                    DateDelivery = dateNow,
                    Completed = false,
                    ClientId = mClient.Id
                };
                _context.Purchases.Add(mPurchase);
                await _context.SaveChangesAsync();

                var purchase = await _context.Purchases.FirstOrDefaultAsync(x => x.DatePurchase == dateNow);

                foreach (var item in cartItem)
                {
                    MProduct product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.Product.Id);
                    MPurchaseProduct mPurchaseProduct = new MPurchaseProduct
                    {
                        Title = product.Title,
                        Type = product.Type,
                        Currency = product.Currency,
                        Price = product.Price,
                        Img = product.Img,
                        ImgBack = product.ImgBack,
                        Size = item.Size,
                        PurchaseId = purchase.Id
                    };

                    _context.PurchaseProducts.Add(mPurchaseProduct);
                    await _context.SaveChangesAsync();

                    // Удаление из корзины
                    List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

                    int index = isExist(Convert.ToString(item.Product.Id));
                    cart.RemoveAt(index);
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                    int count = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart").Count();
                    SessionHelper.SetCartCount(HttpContext.Session, count);
                }


                // Оправка заказа на почту
                #region Шифрование
                EmailService emailService = new EmailService();
                var purchaseId = Convert.ToString(purchase.Id);

                //Отправка клиенту на почту
                if (client.Email != null)
                    using (Aes myAes = Aes.Create())
                    {
                        EncryptionService encryption = new EncryptionService();
                        // Encrypt the string to an array of bytes.
                        byte[] encrypted = encryption.EncryptStringToBytes_Aes(purchaseId, myAes.Key, myAes.IV);

                        var callbackUrl = Url.Action(
                            "PurchaseDetails",
                            "Main",
                            new { encrypt = encrypted, key = myAes.Key, iv = myAes.IV },
                            protocol: HttpContext.Request.Scheme);

                        await emailService.SendEmailAsync(client.Email, client.FullName + ",спасибо за покупку в нашем интернет-магазине",
                            $"Ваш заказ <b>G2-O-{purchase.Id}</b> принят в обработку." +
                            $"<br>" +
                            $"Наш менеджер свяжется с Вами в течение 24 часов." +
                            $" <br>" +
                            $"Проверьте, правильно ли все указано в Вашем заказе" +
                            $" <br>" +
                            $"Детали вашего заказа <a href='{callbackUrl}'>Перейти</a>");
                    }
                #endregion

                //Отправка изготовителю на почту
                string message = "Поступил новый заказ! " + client.FullName + " Город: " + client.City
                    + " ,Адрес: " + client.Address + ",Номер:" + client.Phone + "";
                await emailService.SendEmailAsync(message);
            }
            return View("Success");
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Delivery()
        {
            return View();
        }

        public async Task<IActionResult> Rewiews()
        {
            var mComments = await _context.Comments.Where(x => x.Public == true).ToListAsync();
            return View(mComments);
        }

        [HttpPost]
        public async Task<IActionResult> RewiewCreate(MComment mComment, IFormFile image)
        {
            mComment.Date = DateTime.Now;
            mComment.Public = false;
            string pathServer = Path.Combine(_env.WebRootPath, "Images");
            if (!Directory.Exists(pathServer))
            {
                Directory.CreateDirectory(pathServer);
            }
            if (image != null)
            {
                string imageName = DateTime.Now.Day + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Hour + image.FileName;
                string imagePath = Path.Combine(pathServer, imageName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                mComment.Img = imageName;
            }

            await _context.Comments.AddAsync(mComment);
            await _context.SaveChangesAsync();
            return View("RewiewEnd");
        }

        public IActionResult Contacts()
        {
            return View();
        }

        private int isExist(string id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id == Convert.ToInt32(id))
                {
                    return i;
                }
            }
            return -1;
        }

        //Отслеживание заказа
        public async Task<IActionResult> PurchaseDetails(string[] encrypt, string[] key, string[] iv)
        {
            if (encrypt == null)
            {
                return View("Error");
            }

            byte[] encryptBytes = new byte[encrypt.Count()];
            byte[] keyBytes = new byte[key.Count()];
            byte[] ivBytes = new byte[iv.Count()];
            for (int i = 0; i < encrypt.Count(); i++)
            {
                encryptBytes[i] = byte.Parse(encrypt[i]);
            }
            for (int i = 0; i < key.Count(); i++)
            {
                keyBytes[i] = byte.Parse(key[i]);
            }
            for (int i = 0; i < iv.Count(); i++)
            {
                ivBytes[i] = byte.Parse(iv[i]);
            }
            EncryptionService encryption = new EncryptionService();
            string purchaseIdStr = encryption.DecryptStringFromBytes_Aes(encryptBytes, keyBytes, ivBytes);
            int purchaseId = Convert.ToInt32(purchaseIdStr);

            var purchase = await _context.Purchases
                .Include(x => x.Client)
                .Include(x => x.PurProducts)
                .FirstOrDefaultAsync(x => x.Id == purchaseId);
            return View(purchase);
        }
    }


}