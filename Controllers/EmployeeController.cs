using BankBranchAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankBranchAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly BankContext _context;
        public EmployeeController(BankContext context) 
        {
            _context = context;
        }

        [HttpPost("{id}")]
        public IActionResult Add(int id, AddEmployeeRequest request)
        {
            var branch = _context.BankBranches.FirstOrDefault(b => b.Id == id);
            if (branch == null)
            {
                return NotFound();
            }

            _context.Employees.Add(new Employee() { Name = request.Name, CivilId = request.CivilId, Position = request.Position, BankBranch = branch });
            _context.SaveChanges();

            return Created();
        }

        [HttpPatch("{id}")]
        public IActionResult Edit(int id, AddEmployeeRequest request)
        {
            var employee = _context.Employees.FirstOrDefault(b => b.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            employee.Position = request.Position;
            employee.Name = request.Name;
            employee.CivilId = request.CivilId;
            _context.SaveChanges();

            return Created();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }
            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet("{id}")]
        public ActionResult<EmployeeResponse> Details(int id) 
        {
            var employee = _context.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }
            return new EmployeeResponse { Name = employee.Name, CivilId = employee.CivilId, Position = employee.Position };
        }
    }
}
