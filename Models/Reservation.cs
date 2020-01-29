using System;
using System.ComponentModel.DataAnnotations;

// This is the many to many relationship table

namespace weddingplannerBES.Models
{
    public class Reservation
    {
        [Key]

        // Primary key 
        public int RSVPId { get; set; }

        // Foreign Keys below:
        public int GuestId { get; set; }

        public int OneWeddingId { get; set; }


        // **********************************
        // For Entity below. These need to match exactly the two fields above whatever they are.  These are an instance of a single user and a single wedding. 

        public User Guest { get; set; }
        // create a list in User to match this reference.

        public Wedding OneWedding { get; set; }
        // create a list in Weddings to match this reference.




    }
}