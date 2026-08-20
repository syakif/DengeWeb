using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DengeWeb.Controllers
{
    public class LanguageController : Controller
    {
        public IActionResult Change(string culture, string returnUrl)
        {
            // Seçilen dili tarayıcıya çerez (cookie) olarak 1 yıllığına kaydediyoruz
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            // Kullanıcı dili değiştirdiği sayfaya geri dönsün
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}