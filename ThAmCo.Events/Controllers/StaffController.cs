using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThAmCo.Events.Data;
using ThAmCo.Events.Models;

namespace ThAmCo.Events.Controllers
{
    public class StaffController : Controller
    {
        private readonly EventsDbContext _context;

        public StaffController(EventsDbContext context)
        {
            _context = context;
        }

        // GET: Staff
        /// <summary>
        /// Gets all staff objects in list, displays as staff view model, sends to view
        /// </summary>
        /// <returns>If success</returns>
        public async Task<IActionResult> Index()
        {
            var staff = await _context.Staff.Select(s => new StaffViewModel
            {
                Id = s.Id,
                Surname = s.Surname,
                FirstName = s.FirstName,
                Email = s.Email,
                IsFirstAider = s.IsFirstAider
            }).ToListAsync();

            return View(staff);
        }

        // GET: Staff/Details/5
        /// <summary>
        /// Gets details for a staff member, by using their id to find their object in database, projects into
        /// view model and sends to view.
        /// </summary>
        /// <param name="id">Id of staff</param>
        /// <returns>If success</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff.Select(s => new StaffViewModel
            {
                Id = s.Id,
                Surname = s.Surname,
                FirstName = s.FirstName,
                Email = s.Email,
                IsFirstAider = s.IsFirstAider
            }).FirstOrDefaultAsync(m => m.Id == id);

            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staff/Create
        /// <summary>
        /// Gets create form for staff for the view.
        /// </summary>
        /// <returns>If success</returns>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Staff/Create
        /// <summary>
        /// Gets data from create view, binds to staff object which it posts to the database and returns object
        /// to view.
        /// </summary>
        /// <param name="staff">Object with binded values</param>
        /// <returns>If success</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Surname,FirstName,Email,IsFirstAider")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        // GET: Staff/Edit/5
        /// <summary>
        /// Edit an event by providing an id, where it finds the staff object, which it projects into new
        /// view model, and returns to view.
        /// </summary>
        /// <param name="id">Id of staff</param>
        /// <returns>If success</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            var staffVm = new StaffViewModel()
            {
                Id = staff.Id,
                Surname = staff.Surname,
                FirstName = staff.FirstName,
                Email = staff.Email,
                IsFirstAider = staff.IsFirstAider
            };

            return View(staffVm);
        }

        // POST: Staff/Edit/5
        /// <summary>
        /// Gets data from edit, binds it to staff object, and posts to database.
        /// </summary>
        /// <param name="id">Id of staff</param>
        /// <param name="staff">Object with new binded values</param>
        /// <returns>If success</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Surname,FirstName,Email,IsFirstAider")] Staff staff)
        {
            if (id != staff.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        // GET: Staff/Delete/5
        /// <summary>
        /// Deletes an event by getting the parameter of staff id, getting it from db and projecting into viewmodel,
        /// before sending to view.
        /// </summary>
        /// <param name="id">Id of staff</param>
        /// <returns>If success</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff.Select(s => new StaffViewModel
            {
                Id = s.Id,
                Surname = s.Surname,
                FirstName = s.FirstName,
                Email = s.Email,
                IsFirstAider = s.IsFirstAider
            }).FirstOrDefaultAsync(m => m.Id == id);

            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // POST: Staff/Delete/5
        /// <summary>
        /// Gets staff id from view, uses it to find staff object from database, removes it and saves changes.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checking if a staff for an id exists in the current database context
        /// </summary>
        /// <param name="id">Id of staff</param>
        /// <returns>If staff does exist</returns>
        private bool StaffExists(int id)
        {
            return _context.Staff.Any(e => e.Id == id);
        }
    }
}
