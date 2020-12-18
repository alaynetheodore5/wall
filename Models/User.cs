using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheWall.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage="First Name is required")]
        [MinLength(2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage="Last Name is required")]
        [MinLength(2)]
        public string LastName { get; set; }

        [Required(ErrorMessage="Email is required")]
        [EmailAddress(ErrorMessage="Please enter a valid Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage="Password is required")]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer")]
        [DataType("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage="Confirming a Password is required")]
        [Compare("Password")]
        [DataType("Password")]
        [NotMapped]
        public string Confirm { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}