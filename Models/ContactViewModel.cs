using System.ComponentModel.DataAnnotations;

namespace DengeWeb.Models;

public class ContactViewModel
{
    [Required(ErrorMessage = "Lütfen adınızı ve soyadınızı giriniz.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Lütfen e-posta adresinizi giriniz.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Lütfen bir konu belirtiniz.")]
    public string Subject { get; set; }

    [Required(ErrorMessage = "Lütfen mesajınızı yazınız.")]
    public string Message { get; set; }
}