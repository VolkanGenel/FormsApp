﻿using Microsoft.AspNetCore.Mvc;
using FormsApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FormsApp.Controllers;

public class HomeController : Controller
{
    public HomeController()
    {

    }

    // public IActionResult Index(string searchString, string category)
    // {
    //     var products = Repository.Products;
    //     if(!String.IsNullOrWhiteSpace(searchString))
    //     {
    //         ViewBag.SearchString = searchString;
    //         products = products.Where(x=>x.Name.ToLower().Contains(searchString)).ToList();
    //     }

    //     if(!String.IsNullOrWhiteSpace(category) && category != "0")
    //     {
    //         products = products.Where(x=>x.CategoryId == int.Parse(category)).ToList();
    //     }

    //     // Categori bilgilerini aspnet yardımı ile doğrudan select kutusunda açmak için SelectList kullanıyoruz.
    //     // Select kutusunda değeri Category Modelindeki CategoryId bilgisi gelsin, görünürü Name olsun.
    //     ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name", category);
    //     return View(products);
    // }

    public IActionResult Index(string searchString, string category)
    {
        var products = Repository.Products;
        if (!String.IsNullOrWhiteSpace(searchString))
        {
            ViewBag.SearchString = searchString;
            products = products.Where(x => x.Name!.ToLower().Contains(searchString)).ToList(); //x.Name! => ! Ben Name'i null göndermeyeceğim. Mutlaka veri göndereceğim anlamına gelir.
        }

        if (!String.IsNullOrWhiteSpace(category) && category != "0")
        {
            products = products.Where(x => x.CategoryId == int.Parse(category)).ToList();
        }

        // Categori bilgilerini aspnet yardımı ile doğrudan select kutusunda açmak için SelectList kullanıyoruz.
        // Select kutusunda değeri Category Modelindeki CategoryId bilgisi gelsin, görünürü Name olsun.
        //ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name", category);

        ProductViewModel myModel = new ProductViewModel()
        {
            Categories = Repository.Categories,
            Products = products,
            SelectedCategory = category
        };
        return View(myModel);
    }
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Product model, IFormFile imageFile)
    {
        var extension = "";
        if (imageFile != null)
        {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        // imageFile'ı kendi adında directory de wwwroot altında img altında kendi adında kaydetmek için yapıyorum. Eğer aynı isimde dosya varsa üzerine yazar. Bu yüzden benzersiz bir isim belirleyeceğiz.
        extension = Path.GetExtension(imageFile.FileName);
        // Eklediğimiz dosyanın uzantı bilgisini alır.
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("", "Geçerli bir resim seçiniz yalnızca jpg,fpeg ve png formatları geçerlidir.");
            }
        }

        if (ModelState.IsValid)
        {
            if (imageFile != null)
            {
            var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
            // bu isimde bir dosya oluştur.
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);
                using (var stream = new FileStream(path, FileMode.Create))
                // dosya yükleme işlemi
                {
                    await imageFile!.CopyToAsync(stream);
                }
            model.Image = randomFileName;
            model.ProductId = Repository.Products.Count + 1;
            Repository.CreateProduct(model);
            // Geriye View gönderseydik aynı forma geri dönerdik. Bu yüzden Index Metotuna gitmesini söyledik.
            return RedirectToAction("Index");
            }
        }
        // modeli geri gönderdik ki kullanıcı bilgileri sıfırdan yazmakla uğraşmasın.
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);
    }

    // [HttpPost]
    // public IActionResult Create(string Name, decimal Price, string Image, int CategoryId, bool IsActive)
    // {
    //     return View();
    // }

    // [HttpPost]
    // public IActionResult Create([Bind("Name","Price","Image","CategoryId","IsActive")]Product model)
    // {
    //     return View();
    // }

    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
        if (entity == null)
        {
            return NotFound();
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(entity);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product model, IFormFile? imageFile)
    {
        if (id != model.ProductId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (imageFile != null)
            {
        var extension = Path.GetExtension(imageFile!.FileName);
        var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile!.CopyToAsync(stream);
                }
                model.Image = randomFileName;
            }
            Repository.EditProduct(model);
            return RedirectToAction("Index");
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);
    }

    // public IActionResult Delete(int? id)
    // {
    //     if(id==null)
    //     {
    //         return NotFound();
    //     }
    //     var entity = Repository.Products.FirstOrDefault(p=>p.ProductId==id);
    //     if(entity==null)
    //     {
    //         return NotFound();
    //     }
    //     Repository.DeleteProduct(entity);
    //     return RedirectToAction("Index");
    // }

        public IActionResult Delete(int? id)
    {
        if(id==null)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p=>p.ProductId==id);
        if(entity==null)
        {
            return NotFound();
        }
        return View("DeleteConfirm",entity);
    }
    [HttpPost]
    public IActionResult Delete(int id, int ProductId)
    {
        if (id != ProductId)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p=>p.ProductId==ProductId);
        if(entity==null)
        {
            return NotFound();
        }
        Repository.DeleteProduct(entity);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult EditProducts(List<Product> Products)
    {
        foreach (var product in Products)
        {
            Repository.EditIsActive(product);
        }
        return RedirectToAction("Index");
    }
}
