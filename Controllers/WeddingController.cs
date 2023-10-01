using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers;

[SessionCheck]
public class WeddingController : Controller
{
    private readonly ILogger<WeddingController> _logger;

    private MyContext db;

    public WeddingController(ILogger<WeddingController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    //Dashboard First Page
    [HttpGet("weddings")]
    public IActionResult Index()
    {
        //Many to many, add guests list and invited guests, to get 3 tables of information
        List<Wedding> weddings = db.Weddings.Include(g => g.Guests).ThenInclude(r => r.InvitedGuest).Include(c => c.Creator).ToList();
        return View(weddings);
    }

    //Add a Wedding Page
    [HttpGet("weddings/new")]
    public IActionResult New()
    {
        return View();
    }

    //Add Wedding to Database
    [HttpPost("weddings/create")]
    public IActionResult Create(Wedding newWedding)
    {
        if (!ModelState.IsValid)
        {
            return View("New");
        }
        newWedding.UserId = (int)HttpContext.Session.GetInt32("UUID");
        db.Weddings.Add(newWedding);
        db.SaveChanges();
        return RedirectToAction("Details", new {weddingId = newWedding.UserId});
    }

    //View One Wedding Page
    [HttpGet("weddings/{weddingId}")]
    public IActionResult Details(int weddingId)
    {
        //Include many to many guests list
        Wedding? wedding = db.Weddings.Include(g => g.Guests).ThenInclude(c => c.InvitedGuest).FirstOrDefault(w => w.WeddingId == weddingId);

        if (wedding == null)
        {
            return RedirectToAction("Index");
        }
        return View("Details", wedding);
        
    }

    //Delete Wedding
    [HttpPost("weddings/{weddingId}/delete")]
    public IActionResult Delete(int weddingId)
    {
        Wedding? wedding = db.Weddings.FirstOrDefault(w => w.WeddingId == weddingId);
        if (wedding == null || wedding.UserId != HttpContext.Session.GetInt32("UUID"))
        {
            return RedirectToAction("Index");
        }
        db.Weddings.Remove(wedding);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    //RSVP
    [HttpPost("weddings/{weddingId}/rsvp")]
    public IActionResult RSVP (int weddingId)
    {
        int? userId = HttpContext.Session.GetInt32("UUID");
        if (userId == null)
        {
            return RedirectToAction("Index");
        }

        //RSVP logic: If the UserId in the Wedding Guests is equal to the Session userId AND the wedding id of the wedding and parameter is equal
        WeddingGuest? rsvpGuest = db.WeddingGuests.FirstOrDefault(u => u.UserId == userId.Value && u.WeddingId == weddingId);

        if (rsvpGuest != null)
        {
            db.WeddingGuests.Remove(rsvpGuest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        else
        {
            WeddingGuest newWeddingGuest = new WeddingGuest()
            {
                WeddingId = weddingId,
                UserId = userId.Value
            };
            db.WeddingGuests.Add(newWeddingGuest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public class SessionCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Find the session, but remember it may be null so we need int?
            int? userId = context.HttpContext.Session.GetInt32("UUID");
            // Check to see if we got back null
            if(userId == null)
            {
                // Redirect to the Index page if there was nothing in session
                // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
                context.Result = new RedirectToActionResult("Index", "User", null);
            }
        }
    }
}
