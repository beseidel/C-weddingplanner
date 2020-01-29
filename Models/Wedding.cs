using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace weddingplannerBES.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Bride's Name must be 3 characters or longer!")]
        public string WedderOneBride { get; set; }


        [Required]
        [MinLength(3, ErrorMessage = "Groom's Name must be 3 characters or longer!")]
        public string WedderTwoGroom { get; set; }


        [Required]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage = "Wedding Dates Must be in the future only")]
        public DateTime Date { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;



        // Foreign Keys below 

        public int CreatorId { get; set; }

        // for a foreign key , we need a single item that will match the other table that has a list.  Single item...
        public User Creator { get; set; }

        // ENTITY BELOW

        // Need to create a list of reservations from the reservation table. 
        public List<Reservation> Guests { get; set; }



    }

    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return (DateTime)value < DateTime.Now ? new ValidationResult("WEDDINGS MUST BE IN FUTURE") : ValidationResult.Success;
        }
    }

}

