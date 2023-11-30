using Ethereal_Cloud.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ethereal_Cloud.Pages
{
    public class AccountModel : PageModel
    {

        private readonly TestingDbContext _context;

        
        public List<Account> Accounts { get; set; } = new List<Account>();


        [BindProperty]
        public Account NewAccount { get; set; }

        public AccountModel(TestingDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            Accounts = _context.Account.ToList();



        }

        public IActionResult OnPost()
        {
            _context.Account.Add(NewAccount);

            _context.SaveChanges();

            return RedirectToPage();
        }


    }

}
