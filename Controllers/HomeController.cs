using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using weddingplannerBES.Models;



namespace weddingplannerBES.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        //  INDEX GET REGISTER/LOGIN
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // REGISTER USER 
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {

                if (dbContext.UsersTable.Any(u => u.Email == user.Email))
                {
                    ModelState.TryAddModelError("Email", "Email already in use!");
                    ViewBag.DuplicateMessage = "Email already in use";
                    return View("index");
                }
                else
                {

                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    dbContext.UsersTable.Add(user);
                    dbContext.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View("index");
        }


        // **************************
        // LOGIN INTO THE DASHBOARD

        [HttpPost("Login")]

        public IActionResult Login(string Email, string Password)
        {
            if (ModelState.IsValid)
            {
                // If initial ModelState is valid, query for a user with provided email
                var userInDb = dbContext.UsersTable.FirstOrDefault(u => u.Email == Email);
                // If no user exists with provided email
                if (userInDb == null || Password == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email");
                    return View("Index");
                }

                {
                    var hasher = new PasswordHasher<User>();

                    // verify provided password against hash stored in db
                    var result = hasher.VerifyHashedPassword(userInDb, userInDb.Password, Password);

                    // result can be compared to 0 for failure
                    if (result == 0)
                    {
                        ModelState.AddModelError("Password", "Invalid Password");
                        return View("Index");
                    }



                }
                HttpContext.Session.SetInt32("Id", userInDb.UserId);
                ViewBag.show = "Successfully created your Email";

                return RedirectToAction("dashboard");

            }
            return View("Index");
        }

        // LOGOUT

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {

            HttpContext.Session.Clear();

            return RedirectToAction("");
        }

        // **********************************

        // SHOW Dashboard

        [Route("dashboard")]
        [HttpGet]
        public IActionResult dashboard()
        {
            var Key = HttpContext.Session.GetInt32("Id");

            System.Console.WriteLine(Key);
            if (Key == null)
            {
                return RedirectToAction("Index");
            }

            // SHOWS the wedding table starting here

            List<Wedding> AllWeddings = dbContext.WeddingsTable.OrderBy(weds => weds.CreatedAt)
            .Include(wedder => wedder.Guests)
            .ThenInclude(reservation => reservation.OneWedding)
            .ToList();

            // ****************************

            // Welcome to the dashboard User Barbara

            User NewUser = dbContext.UsersTable.SingleOrDefault(userIterate => userIterate.UserId == HttpContext.Session.GetInt32("Id"));

            ViewBag.Id = Key;
            ViewBag.NewUser = NewUser;

            ViewBag.UserId = HttpContext.Session.GetInt32("Id");

            // *************************

            return View("dashboard", AllWeddings);


        }


        //SHOW FORM TO CREATE NEW WEDDING

        [HttpGet("NewWedding")]
        public IActionResult NewWedding()
        {
            return View();


        }

        // Processing a new Wedding
        // NewWedding route can be called anything. 

        [Route("NewWedding")]
        [HttpPost]
        // submitted_wedding can be named anything..
        public IActionResult processwedding(Wedding submitted_wedding)
        {

            if (ModelState.IsValid)
            {
                User LoggedinUser = dbContext.UsersTable.SingleOrDefault(userIterate => userIterate.UserId ==
                HttpContext.Session.GetInt32("Id"));

                submitted_wedding.CreatorId = (int)HttpContext.Session.GetInt32("Id");

                dbContext.Add(submitted_wedding);
                dbContext.SaveChanges();
                return RedirectToAction("dashboard");
            }
            else
            {
                return View("newwedding");
            }
        }

        // [HttpPost("createwed")]
        // public IActionResult processwedding(Wedding newOne)
        // {

        //     dbContext.WeddingsTable.Add(newOne);
        //     dbContext.SaveChanges();
        //     return RedirectToAction("dashboard", new { WeddingId = newOne.WeddingId });

        // }


        // Create New Wedding Processing
        [HttpPost("rsvp/{WeddingId}")]
        public IActionResult rsvp(int WeddingId)
        {
            Reservation Objrsvp = new Reservation
            {
                GuestId = (int)HttpContext.Session.GetInt32("Id"),
                OneWeddingId = WeddingId

            };

            dbContext.ReservationsTable.Add(Objrsvp);
            dbContext.SaveChanges();
            return RedirectToAction("dashboard");
        }


        // // creating the unrsvp route????
        [HttpPost("unrsvp/{WeddingId}")]
        public IActionResult unrsvp(int WeddingId)
        {
            Reservation ObjRSVP = dbContext.ReservationsTable.Where(guest => guest.GuestId == (int)HttpContext.Session.GetInt32("Id")).Where(wedding => wedding.OneWeddingId == WeddingId)
            .SingleOrDefault();

            // Delete unrsvp by signed in User
            dbContext.ReservationsTable.Remove(ObjRSVP);
            dbContext.SaveChanges();
            return RedirectToAction("dashboard");
        }

        // Delete Wedding by Loggedin User/Creator in Session

        [HttpGet("delete/{WeddingId}")]
        public IActionResult Delete(int WeddingId)
        {
            Wedding RetrievedWeddingbyCreatorUserinSession = dbContext.WeddingsTable.SingleOrDefault(wedder => wedder.WeddingId == WeddingId);
            dbContext.WeddingsTable.Remove(RetrievedWeddingbyCreatorUserinSession);
            dbContext.SaveChanges();

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");

            return RedirectToAction("dashboard");
        }

        // //show one wedding 
        [HttpGet("ShowOne/{WeddingId}")]
        public IActionResult ShowOne(int WeddingId)
        {
            Wedding selectedWedding = dbContext.WeddingsTable

            // Guest newguest = new Guest

            .Include(wedder => wedder.Guests)
            // .Include(wedder => wedder.Guests)
            .ThenInclude(Reservate => Reservate.Guest)

            .FirstOrDefault(wedd => wedd.WeddingId == WeddingId);


            ViewBag.AllGuests = selectedWedding;


            return View("ShowOne", selectedWedding);
        }

        // [HttpGet("edit/{id}")]
        // public IActionResult Edit(int id)
        // {
        //     Wedding selectedWedding = dbContext.WeddingsTable.FirstOrDefault(Wedding => Wedding.WeddingId == id);
        //     return View("Edit", selectedWedding);
        // }



        // [HttpPost("{id}")]
        // public IActionResult Update(int id, Wedding selectedWedding)
        // {
        //     Wedding RetrievedWedding = dbContext.WeddingsTable.FirstOrDefault(d => d.Id == id);


        //     if (ModelState.IsValid)
        //     {
        //         RetrievedWedding.WedderOneBride = Wedding.WedderOneBride;

        //         RetrievedWedding.UpdatedAt = DateTime.Now;

        //         dbContext.SaveChanges();

        //         return RedirectToAction("ViewOne", id);
        //     }
        //     else
        //     {
        //         return View("Edit", RetrievedWedding);
        //     }
        // }





    }

}
//   {
//             Wedding RetrievedOne = dbContext.wedding
//             .Include(a => a.Users)
//             .ThenInclude(b => b.Auser)
//             .FirstOrDefault(us => us.WeddingId == WeddingId);
//             ViewBag.AllGuest = RetrievedOne;


//             return View("Show", RetrievedOne);
//         }

//   [HttpGet("view/{id}")]
//         public IActionResult ViewOne(int id)
//         {
//             Dish selectedDish = dbContext.DishTable.FirstOrDefault(dish => dish.Id == id);
//             return View("ViewOne", selectedDish);
//         }






