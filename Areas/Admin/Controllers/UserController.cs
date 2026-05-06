using BooksSpr2026.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BooksSpr2026.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {

        private readonly BooksDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(BooksDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            List<ApplicationUser> userList = _dbContext.ApplicationUsers.ToList(); //fetches list of all users

            var allRoles = _dbContext.Roles.ToList();//fethces all roles in the AspNetRoles table

            var userRoles = _dbContext.UserRoles.ToList();//fetches info about userId's associated with RoleIds

            foreach(var user in userList)
            {
                var roleId = userRoles.Find(ur => ur.UserId == user.Id).RoleId; //fetches the RoleId for the current user

                var roleName = allRoles.Find(r => r.Id == roleId).Name;//fetches the name of the role that the current user falls under

                user.RoleName = roleName;

            }

            return View(userList);

        }

        public IActionResult LockUnlock(string id)
        {
            var userFromDB = _dbContext.ApplicationUsers.Find(id);

            if(userFromDB.LockoutEnd != null && userFromDB.LockoutEnd > DateTime.Now)
            {
                userFromDB.LockoutEnd = DateTime.Now;

            }
            else //account is currently unlocked, we will lock it
            {
                userFromDB.LockoutEnd = DateTime.Now.AddYears(10);

            }

            _dbContext.SaveChanges();

            return RedirectToAction("Index");

        }

        public IActionResult EditUserRole(string id)
        {
            var currentUserRole = _dbContext.UserRoles.FirstOrDefault(ur => ur.UserId == id);//fetches userId and the roleId from the UserRoles asp net table

            IEnumerable<SelectListItem> listOfRoles = _dbContext.Roles.ToList().Select(r =>
                new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id.ToString()

                }
                );

            ViewBag.ListOfRoles = listOfRoles;

            ViewBag.UserInfo = _dbContext.ApplicationUsers.Find(id);

            return View(currentUserRole);

        }

        [HttpPost]
        public IActionResult EditUserRole(IdentityUserRole<string> updatedRole)
        {
            ApplicationUser applicationUser = _dbContext.ApplicationUsers.Find(updatedRole.UserId);

            string newRoleName = _dbContext.Roles.Find(updatedRole.RoleId).Name;

            string oldRoleId = _dbContext.UserRoles.FirstOrDefault(u => u.UserId == applicationUser.Id).RoleId;

            string oldRoleName = _dbContext.Roles.Find(oldRoleId).Name;

            _userManager.RemoveFromRoleAsync(applicationUser, oldRoleName).GetAwaiter().GetResult();

            _userManager.AddToRoleAsync(applicationUser, newRoleName).GetAwaiter().GetResult();

            return RedirectToAction("Index");

        }

    }
}
