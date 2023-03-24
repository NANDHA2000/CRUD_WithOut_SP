using CRUD_with_Dapper.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace CRUD_with_Dapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KtEmployeeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public KtEmployeeController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public async Task<ActionResult<List<KtEmployees>>> GetAllKtEmployees()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<KtEmployees> employees = await SelectAllemployees(connection);
            return Ok(employees);
        }
        [HttpGet("{empId}")]
        public async Task<ActionResult<List<KtEmployees>>> GetKtEmployeesById(int empId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var employee = await connection.QueryFirstAsync<KtEmployees>("Select * from KT_Employees where EmpID = @Id", new { Id = empId });
            return Ok(employee);
        }

        [HttpPost]
        public async Task<ActionResult<List<KtEmployees>>> CreateEmployee([FromForm]KtEmployees  employees)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into KT_Employees(EmpID,EmpName,EmpPosition,EmpPlace,EmpSalary,depID) values(@EmpID,@EmpName,@EmpPosition,@EmpPlace,@EmpSalary,@depID)",employees);
            return Ok(await SelectAllemployees(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<KtEmployees>>> UpdateEmployee(KtEmployees employees)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update KT_Employees set EmpName=@EmpName,EmpPosition=@EmpPosition,EmpPlace=@EmpPlace,EmpSalary=@EmpSalary,depID=@depID where EmpID =@EmpID", employees);
            return Ok(await SelectAllemployees(connection));
        }

        [HttpDelete("{empId}")]
        public async Task<ActionResult<List<KtEmployees>>> DeleteKtEmployeeById(int empId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Delete from KT_Employees where EmpID = @Id", new { Id = empId });
            return Ok(await SelectAllemployees(connection));
        }

        private static async Task<IEnumerable<KtEmployees>> SelectAllemployees(SqlConnection connection)
        {
            return await connection.QueryAsync<KtEmployees>("Select * from KT_Employees");
        }
    }
}
