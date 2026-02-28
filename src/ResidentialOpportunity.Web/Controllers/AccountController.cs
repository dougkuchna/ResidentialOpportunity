using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Application.Services;
using ResidentialOpportunity.Domain.Entities;
using ResidentialOpportunity.Domain.ValueObjects;
using ResidentialOpportunity.Infrastructure.Data;

namespace ResidentialOpportunity.Web.Controllers;

[Route("api/account")]
[ApiController]
[IgnoreAntiforgeryToken]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly AppDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ServiceRequestService _requestService;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        AppDbContext dbContext,
        IUnitOfWork unitOfWork,
        ServiceRequestService requestService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _requestService = requestService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromForm] string email,
        [FromForm] string password,
        [FromForm] string? rememberMe)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Redirect("/account/login?error=Email+and+password+are+required");

        var result = await _signInManager.PasswordSignInAsync(
            email, password, isPersistent: rememberMe == "true", lockoutOnFailure: false);

        if (!result.Succeeded)
            return Redirect("/account/login?error=Invalid+email+or+password");

        // Claim any unclaimed requests for this email
        var user = await _userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            var customer = await _dbContext.Customers
                .FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);

            if (customer is not null)
                await _requestService.ClaimRequestsForCustomerAsync(customer.Id, email);
        }

        return Redirect("/");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromForm] string email,
        [FromForm] string password,
        [FromForm] string confirmPassword,
        [FromForm] string name,
        [FromForm] string phone)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
            return Redirect("/account/register?error=All+fields+are+required");

        if (password != confirmPassword)
            return Redirect("/account/register?error=Passwords+do+not+match");

        var user = new IdentityUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Redirect($"/account/register?error={Uri.EscapeDataString(errors)}");
        }

        // Create Customer entity linked to Identity user
        var contactInfo = new ContactInfo(name, email, phone);
        var customer = Customer.Create(user.Id, contactInfo);
        _dbContext.Customers.Add(customer);
        await _unitOfWork.SaveChangesAsync();

        // Claim any anonymous requests matching this email
        await _requestService.ClaimRequestsForCustomerAsync(customer.Id, email);

        // Sign in
        await _signInManager.SignInAsync(user, isPersistent: true);
        return Redirect("/");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/");
    }
}
