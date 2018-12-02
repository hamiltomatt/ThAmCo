using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThAmCo.Events.Data;
using ThAmCo.Events.Models;

namespace ThAmCo.Events
{
    public class CustomersController : Controller
    {
        private readonly EventsDbContext _context;

        public CustomersController(EventsDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers.Select(c => new CustomerViewModel
            {
                Id = c.Id,
                Surname = c.Surname,
                FirstName = c.FirstName,
                Email = c.Email
            }).ToListAsync();

            return View(customers);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.Select(c => new CustomerDetailsViewModel
            {
                Id = c.Id,
                Surname = c.Surname,
                FirstName = c.FirstName,
                Email = c.Email,
                Events = c.Bookings.Select(b => new GuestEventViewModel
                {
                    CustomerId = b.CustomerId,
                    EventId = b.EventId,
                    EventName = b.Event.Title,
                    EventDate = b.Event.Date,
                    Attended = b.Attended
                })
            }).FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Surname,FirstName,Email")] CustomerViewModel customerVm)
        {
            if (ModelState.IsValid)
            {
                var customer = new Customer()
                {
                    Id = customerVm.Id,
                    Surname = customerVm.Surname,
                    FirstName = customerVm.FirstName,
                    Email = customerVm.Email
                };

                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customerVm);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.Select(c => new CustomerViewModel
            {
                Id = c.Id,
                Surname = c.Surname,
                FirstName = c.FirstName,
                Email = c.Email
            }).FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Surname,FirstName,Email")] CustomerViewModel customerVm)
        {
            if (id != customerVm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var customer = new Customer()
                    {
                        Id = customerVm.Id,
                        Surname = customerVm.Surname,
                        FirstName = customerVm.FirstName,
                        Email = customerVm.Email
                    };

                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customerVm.Id))
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
            return View(customerVm);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.Select(c => new CustomerViewModel
            {
                Id = c.Id,
                Surname = c.Surname,
                FirstName = c.FirstName,
                Email = c.Email
            }).FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
