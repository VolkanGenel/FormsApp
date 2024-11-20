using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FormsApp.Models
{
    // [Bind("Name","Price")] Formlarda hangi alanların gelmesini istediğimizi burada da belirtebiliriz.
    public class Product
    {
        // [Display(Name="UrunId")] Bu anotasyon cshtml sayfasında ProductId yerine UrunId adını kullanmamızı sağlar.
        [Display(Name="UrunId")]
        // [BindNever] Örnek olarak formlarda bu alanı kullanmak istemiyorum anlamına gelir.
        public int ProductId { get; set; }

        public string? SecretKey { get; set; }

        [Display(Name="Urun Adı")] // Hiç koymasaydık Name şeklinde sayfada yazardı.
        [StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}!", MinimumLength =1)] //Sadece sınır da koyabilirdik
        [Required]
        public string? Name { get; set; } // = string.Empty yazsaydık Nullable anlamına gelir string? ile aynı. =null! yazsaydık,ben bunu null göndermeyeceğim garantisi verirdik. O durumda string? ne gerek kalmazdı. ? ni silebilirdik.

        [Display(Name="Fiyat")]
        [Required(ErrorMessage = "It is required field")]
        [Range(0,100_000, ErrorMessage = "Price requires to be between 0 to 100000!" )]
        public decimal? Price { get; set; }

        [Display(Name="Resim")]
        public string? Image { get; set; }  //= string.Empty; Otomatik olarak [Required] gibi oldu.

        public bool IsActive { get; set; }

        [Display(Name="Category")]
        [Required]
        public int? CategoryId { get; set; }

        }
}

// 1 iphone 14 1
// 2 iphone 15 1
// 3 macbook pro 2