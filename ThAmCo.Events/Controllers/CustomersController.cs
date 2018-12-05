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

        /// <summary>
        /// Constructs the controller by providing it with an entity framework context
        /// </summary>
        /// <param name="context">The EF context which will connect to the database</param>
        public CustomersController(EventsDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        /// <summary>
        /// Creates a list of all customers by going into the database context, getting the Customer objects,
        /// and then projecting those into a model appropriate for the Index view
        /// </summary>
        /// <returns>Task which returns if it was successful</returns>
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
        /// <summary>
        /// Creates list of attributes for a particular customer, by projecting from the context into the
        /// model designed to be appropriate for the view.
        /// </summary>
        /// <param name="id">Id of customer</param>
        /// <returns>Task which returns if success</returns>
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
        /// <summary>
        /// Calls the "Create" view, which will open a blank form.
        /// </summary>
        /// <returns>If method was success</returns>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Creates a new customer by binding values to a model object, projecting those into the database model,
        /// adding it to context and saving changes, and returning to view if invalid entry.
        /// </summary>
        /// <param name="customerVm">Object which will take new values</param>
        /// <returns>If task was success</returns>
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
        /// <summary>
        /// Edit data of a customer with a given id, which first projects into view model and sends to view.
        /// </summary>
        /// <param name="id">Id of customer</param>
        /// <returns>If action was success</returns>
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
        /// <summary>
        /// Edits a customer by binding given values to a new model, projecting into database model, updates
        /// context and saves changes to database.
        /// </summary>
        /// <param name="id">Id of customer</param>
        /// <param name="customerVm">Object wth binded values</param>
        /// <returns></returns>
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
        /// <summary>
        /// Deletes a customer by finding it from database, projecting into view model, and pushed to view.
        /// </summary>
        /// <param name="id">Id of customer</param>
        /// <returns>If operation was successful</returns>
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
        /// <summary>
        /// Gets id back from view, and removes relevant customer from database context, saves changes and redirects
        /// to customer index.
        /// </summary>
        /// <param name="id">Id of customer</param>
        /// <returns>If operation was successful</returns>
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
