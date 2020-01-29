using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace weddingplannerBES.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
        public string Password { get; set; }
        // Will not be mapped to your users table!

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
        public string ConfirmPassword { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;



        // For Entity?????

        // Add a list of Weddings to this side since it does not have the foreign key.  Add a list of Reservations too.  

        public List<Wedding> PlannedWeddingsbyCreator { get; set; }
        // this links to the Foreign Key Creator....in the weddings table.

        public List<Reservation> Weddings { get; set; }

    }
}
