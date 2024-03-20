using Microsoft.AspNetCore.Identity;
using BaggageStorage.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace BaggageStorage.Models.UserViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(50)]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public bool IgnoreRole { get; set; }

        public ICollection<IdentityRole> Roles { get; set; }
        public ICollection<string> CurrentUserRoles { get; set; }
        public string[] UserRoles { get; set; }

        public ICollection<Customer> Customers { get; set; }
        public string CustomerId { get; set; }
    }
}