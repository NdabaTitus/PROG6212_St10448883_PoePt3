using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Poe.Data;
using Poe.Models;
using Microsoft.AspNetCore.Authorization;

using Microsoft.EntityFrameworkCore;

namespace Poe.Controllers
{
    [Authorize]

    public class WorkClaimsController : Controller
    {

        private readonly ApplicationDbContext _context;


        public WorkClaimsController(ApplicationDbContext context)
        {

            _context = context;
        }


        [Authorize(Roles = "Worker")]

        public async Task<IActionResult> MyClaims()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var claims = await _context.WorkClaims
                .Where(c => c.WorkerUserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();


            return View(claims);
        }


        [Authorize(Roles = "Worker")]

        public async Task<IActionResult> Create()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var profile = await _context.EmployeeProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);


            var model = new WorkClaim();


            if (profile != null)
            {

                model.Name = profile.Name;

                model.Surname = profile.Surname;

                model.Department = profile.Department;

                model.RatePerJob = profile.DefaultRatePerJob;
            }


            return View(model);
        }


        [HttpPost]

        [ValidateAntiForgeryToken]

        [Authorize(Roles = "Worker")]

        public async Task<IActionResult> Create(WorkClaim claim)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            claim.WorkerUserId = userId;


            claim.TotalAmount = claim.RatePerJob * claim.NumberOfJobs;


            claim.Status = "Submitted";


            if (ModelState.IsValid)
            {

                _context.WorkClaims.Add(claim);


                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(MyClaims));
            }


            return View(claim);
        }


        [Authorize(Roles = "ProjectManager")]

        public async Task<IActionResult> PmPending()
        {

            var pendingClaims = await _context.WorkClaims
                .Where(c => c.Status == "Submitted")
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();


            return View(pendingClaims);
        }


        [Authorize(Roles = "ProjectManager")]

        public async Task<IActionResult> PmApprove(int id)
        {

            var claim = await _context.WorkClaims.FindAsync(id);


            if (claim == null)
            {

                return NotFound();
            }


            claim.Status = "PM Approved";


            claim.RejectReason = null;


            _context.WorkClaims.Update(claim);


            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(PmPending));
        }


        [Authorize(Roles = "ProjectManager")]

        public async Task<IActionResult> PmReject(int id)
        {

            var claim = await _context.WorkClaims.FindAsync(id);


            if (claim == null)
            {
                return NotFound();
            }


            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> PmRejectConfirmed(int id, string? rejectReason)
        {
            var claim = await _context.WorkClaims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }


            claim.Status = "PM Rejected";
            claim.RejectReason = rejectReason;


            if (string.IsNullOrWhiteSpace(rejectReason))
            {

                claim.ReasonRequired = true;


                var notification = new Notification
                {
                    Message = $"Construction Manager requests a reason for PM rejection on claim #{claim.Id}. Please re-open and provide a reason for the worker.",
                    TargetRole = "ProjectManager",
                    WorkClaimId = claim.Id
                };

                _context.Notifications.Add(notification);
            }
            else
            {

                claim.ReasonRequired = false;
            }

            _context.WorkClaims.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PmPending));
        }
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> PmNotifications()
        {
            var notes = await _context.Notifications
                .Where(n => n.TargetRole == "ProjectManager" && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notes);
        }

        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> PmMarkNotificationRead(int id)
        {
            var note = await _context.Notifications.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            note.IsRead = true;
            _context.Notifications.Update(note);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PmNotifications));
        }

        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> PmAddReason(int id)
        {
            var claim = await _context.WorkClaims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }


            if (!claim.ReasonRequired)
            {
                return RedirectToAction(nameof(PmPending));
            }

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> PmAddReasonConfirmed(int id, string rejectReason)
        {
            var claim = await _context.WorkClaims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(rejectReason))
            {
                ModelState.AddModelError("", "A reason is now required for this rejection.");
                return View("PmAddReason", claim);
            }

            claim.RejectReason = rejectReason;
            claim.ReasonRequired = false;
            claim.Status = "PM Rejected";

            _context.WorkClaims.Update(claim);


            var relatedNotes = _context.Notifications
                .Where(n => n.TargetRole == "ProjectManager" && n.WorkClaimId == claim.Id && !n.IsRead);

            foreach (var n in relatedNotes)
            {
                n.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PmPending));
        }



        [Authorize(Roles = "ConstructionManager")]

        public async Task<IActionResult> CmReviewList()
        {

            var claims = await _context.WorkClaims
                .Where(c => c.Status.StartsWith("PM "))
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();


            return View(claims);
        }


        [Authorize(Roles = "ConstructionManager")]

        public async Task<IActionResult> CmNotifications()
        {

            var notifications = await _context.Notifications
                .Where(n => n.TargetRole == "ConstructionManager" && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();


            return View(notifications);
        }

        // 
        [Authorize(Roles = "ConstructionManager")]
        // 
        public async Task<IActionResult> CmMarkNotificationRead(int id)
        {

            var notification = await _context.Notifications.FindAsync(id);


            if (notification == null)
            {
                return NotFound();
            }


            notification.IsRead = true;


            _context.Notifications.Update(notification);


            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(CmNotifications));
        }

        // 
        [Authorize(Roles = "ConstructionManager")]
        // 
        public async Task<IActionResult> CmApprove(int id)
        {

            var claim = await _context.WorkClaims.FindAsync(id);


            if (claim == null)
            {
                return NotFound();
            }


            claim.Status = "CM Approved";




            _context.WorkClaims.Update(claim);


            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(CmReviewList));
        }

        // 
        [Authorize(Roles = "ConstructionManager")]
        // 
        public async Task<IActionResult> CmReject(int id)
        {

            var claim = await _context.WorkClaims.FindAsync(id);


            if (claim == null)
            {
                return NotFound();
            }


            return View(claim);
        }


        [HttpPost]

        [ValidateAntiForgeryToken]

        [Authorize(Roles = "ConstructionManager")]

        public async Task<IActionResult> CmRejectConfirmed(int id, string? rejectReason)
        {

            var claim = await _context.WorkClaims.FindAsync(id);


            if (claim == null)
            {
                return NotFound();
            }


            claim.Status = "CM Rejected";


            claim.RejectReason = rejectReason;


            _context.WorkClaims.Update(claim);


            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(CmReviewList));
        }


        [Authorize(Roles = "HR")]

        public async Task<IActionResult> HrAllClaims()
        {

            var claims = await _context.WorkClaims
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();


            return View(claims);
        }


        [Authorize(Roles = "HR")]

        public async Task<IActionResult> HrMarkPaid(int id)
        {

            var claim = await _context.WorkClaims.FindAsync(id);


            if (claim == null)
            {
                return NotFound();
            }


            claim.Status = "Paid";


            _context.WorkClaims.Update(claim);


            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(HrAllClaims));
        }

        [Authorize(Roles = "Worker")]
        public IActionResult PaymentSimulation()
        {
            return View();
        }

    }
}