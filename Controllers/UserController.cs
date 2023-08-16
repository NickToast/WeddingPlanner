//User controller
//User models & login models
//User views folders & views
//Routes home, register, login, logout
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WeddingPlanner.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;

    private MyContext db;

    public UserController(ILogger<UserController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        //This will prevent people from getting to the inner pages without logging in
        if (HttpContext.Session.GetInt32("UUID") != null)
        {
            return RedirectToAction("Index");
        }
        return View("Index");
    }

    //REGISTRATION FIRST TO TEST DATABASE
    [HttpPost("/register")]
    public IActionResult Register(User newUser)
    {
        if(!ModelState.IsValid)
        {
            return View("Index");
        }

        PasswordHasher<User> hashBrowns = new PasswordHasher<User>();

        newUser.Password = hashBrowns.HashPassword(newUser, newUser.Password);

        db.Users.Add(newUser);
        db.SaveChanges();
        //Set a new session to a variable with the newUser information
        //Key and Value pair, UUID key name and the new user's UserId as the value
        HttpContext.Session.SetInt32("UUID", newUser.UserId);

        return RedirectToAction("Index", "Wedding");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginUser userSubmission)
    {
        //EMAIL CHECK
        //if model state is not valid, return view to the same page where it will show validations
        if (!ModelState.IsValid)
        {
            return View("Index");
        }
        User? userInDb = db.Users.FirstOrDefault(e => e.Email == userSubmission.LoginEmail);

        if (userInDb == null) 
        {
            ModelState.AddModelError("LoginEmail", "Invalid Email Address");
            return View("Index");
        }
        
        //PASSWORD CHECK
        //Invoke hasher, and use LoginUser
        PasswordHasher<LoginUser> hashBrowns = new PasswordHasher<LoginUser>();
        //User submission, the password in the database, provided password in the form that was submitted
        var result = hashBrowns.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);

        //if its true
        if (result == 0)
        {
            ModelState.AddModelError("LoginEmail", "Invalid Email Address/Password");
            return View("Index");
        }

        //HANDLE SUCCESS
        HttpContext.Session.SetInt32("UUID", userInDb.UserId);
        HttpContext.Session.SetString("UserName", userInDb.FirstName);
        return RedirectToAction("Index", "Wedding");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
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
}
