using BankBranchAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankBranchAPI.Controllers
{
    /// <summary>
    /// Bank branches controller with branch related endpoints
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BankController : ControllerBase
    {
        private readonly ILogger<BankController> _logger;
        private readonly BankContext _context;

        public BankController(ILogger<BankController> logger, BankContext dbContext)
        {
            _logger = logger;
            _context = dbContext;
        }

        /// <summary>
        /// Returns a list of all branches
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BankBranchResponse>), 200)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public ActionResult<PageListResult<BankBranchResponse>> GetAll(int page = 1, string search = "", bool ascending = true)
        {
            try
            {

                if(search == "" && ascending)
                {
                    return _context.BankBranches.Select(branch => new BankBranchResponse
                    {
                        LocationName = branch.LocationName,
                        LocationURL = branch.LocationURL,
                        BranchManager = branch.BranchManager,
                        Employees = branch.Employees.Select(employee => new EmployeeResponse { Name = employee.Name, CivilId = employee.CivilId, Position = employee.Position })
                    })
                    .OrderBy(r => r.LocationName)
                    .ToPageList(page, 4);
                } 
                else if(search == "")
                {
                    return _context.BankBranches.Select(branch => new BankBranchResponse
                    {
                        LocationName = branch.LocationName,
                        LocationURL = branch.LocationURL,
                        BranchManager = branch.BranchManager,
                        Employees = branch.Employees.Select(employee => new EmployeeResponse { Name = employee.Name, CivilId = employee.CivilId, Position = employee.Position })
                    })
                    .OrderByDescending(r => r.LocationName)
                    .ToPageList(page, 4);
                }

                if (!ascending)
                {
                    return _context.BankBranches
                    .Where(s => s.LocationName.StartsWith(search))
                    .Select(branch => new BankBranchResponse
                    {
                        LocationName = branch.LocationName,
                        LocationURL = branch.LocationURL,
                        BranchManager = branch.BranchManager,
                        Employees = branch.Employees.Select(employee => new EmployeeResponse { Name = employee.Name, CivilId = employee.CivilId, Position = employee.Position })
                    })
                    .OrderByDescending (r => r.LocationName)
                    .ToPageList(page, 4);
                }

                return _context.BankBranches
                    .Where(s => s.LocationName.StartsWith(search))
                    .Select(branch => new BankBranchResponse
                    {
                        LocationName = branch.LocationName,
                        LocationURL = branch.LocationURL,
                        BranchManager = branch.BranchManager,
                        Employees = branch.Employees.Select(employee => new EmployeeResponse { Name = employee.Name, CivilId = employee.CivilId, Position = employee.Position })
                    })
                    .OrderBy(r => r.LocationName)
                    .ToPageList(page, 4);


            } catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            
        }

        /// <summary>
        /// Adds a new branch
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("add-branch")]
        [ProducesResponseType(typeof(IActionResult), 201)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddBranch(AddBranchRequest request)
        {
            try
            {
                var branch = new BankBranch()
                {
                    LocationName = request.LocationName,
                    LocationURL = request.LocationURL,
                    BranchManager = request.BranchManager
                };
                _context.BankBranches.Add(branch);
                _context.SaveChanges();
                return Created(nameof(Details), new { Id = branch.Id });
            } catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            
        }

        /// <summary>
        /// Returns the details of the branch with the corresponding id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BankBranchResponse), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<BankBranchResponse> Details(int id)
        {
            try
            {
                var bank = _context.BankBranches.Where(b => b.Id == id).Include(b => b.Employees).SingleOrDefault();
                if (bank == null)
                {
                    return NotFound();
                }
                return new BankBranchResponse
                {
                    LocationName = bank.LocationName,
                    LocationURL = bank.LocationURL,
                    BranchManager = bank.BranchManager,
                    Employees = bank.Employees.Select(employee => new EmployeeResponse { Name = employee.Name, CivilId = employee.CivilId, Position = employee.Position })
                };
            } catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError);
            }          
        }

        /// <summary>
        /// Updates the properties of the branch with the corresponding id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(IActionResult), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Edit(int id, AddBranchRequest request)
        {
            try
            {
                var branch = _context.BankBranches.Find(id);
                if (branch == null)
                {
                    return NotFound();
                }
                branch.LocationName = request.LocationName;
                branch.LocationURL = request.LocationURL;
                branch.BranchManager = request.BranchManager;
                _context.SaveChanges();

                return Created(nameof(Details), new { id = branch.Id });
            } catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }

        /// <summary>
        /// Deletes the branch with the correspondign id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof (IActionResult), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                var branch = _context.BankBranches.Find(id);
                if( branch == null)
                {
                    return NotFound();
                }
            
                _context.BankBranches.Remove(branch);
                _context.SaveChanges();

                return Ok();
            } catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }          
        }
    }
}
