using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

// Controllers/DepartmentsController.cs

[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    private readonly DbConnector dbConnector;
    private readonly string connectionString = "Server=.;Database=EmployeeManagementDB;Integrated Security=true;TrustServerCertificate=true";

    //private readonly string connectionString = "Data Source=.;Initial Catalog=your_database_name;Integrated Security=True;"
    public DepartmentsController()
    {
        this.dbConnector = new DbConnector(connectionString);
    }

    [HttpGet]
    public IActionResult GetAllDepartments()
    {
        string query = "SELECT * FROM Departments";
        DataTable departments = dbConnector.ExecuteQuery(query);

        // Convert DataTable to List<Dictionary<string, object>>
        List<Dictionary<string, object>> departmentList = new List<Dictionary<string, object>>();
        foreach (DataRow row in departments.Rows)
        {
            Dictionary<string, object> department = new Dictionary<string, object>();
            foreach (DataColumn col in departments.Columns)
            {
                department.Add(col.ColumnName, row[col]);
            }
            departmentList.Add(department);
        }

        return Ok(departmentList);
    }


    [HttpGet("{id}")]
    public IActionResult GetDepartmentById(int id)
    {
        string query = "SELECT * FROM Departments WHERE DepartmentID = @DepartmentID";
        SqlParameter[] parameters = { new SqlParameter("@DepartmentID", id) };

        DataTable department = dbConnector.ExecuteQuery(query, parameters);

        if (department.Rows.Count == 0)
        {
            return NotFound("Department not found");
        }

        // Convert DataRow to Dictionary<string, object>
        Dictionary<string, object> departmentData = new Dictionary<string, object>();
        foreach (DataColumn col in department.Columns)
        {
            departmentData.Add(col.ColumnName, department.Rows[0][col]);
        }

        return Ok(departmentData);
    }


    [HttpPost]
    public IActionResult CreateDepartment([FromBody] Department department)
    {
        string query = "INSERT INTO Departments (DepartmentCode, DepartmentName) VALUES (@DepartmentCode, @DepartmentName)";
        SqlParameter[] parameters =
        {
            new SqlParameter("@DepartmentCode", department.DepartmentCode),
            new SqlParameter("@DepartmentName", department.DepartmentName)
        };

        int rowsAffected = dbConnector.ExecuteNonQuery(query, parameters);

        if (rowsAffected > 0)
        {
            return Ok("Department created successfully");
        }

        return BadRequest("Failed to create department");
    }

    [HttpPut("{id}")]
    public IActionResult UpdateDepartment(int id, [FromBody] Department department)
    {
        string query = "UPDATE Departments SET DepartmentCode = @DepartmentCode, DepartmentName = @DepartmentName WHERE DepartmentID = @DepartmentID";
        SqlParameter[] parameters =
        {
            new SqlParameter("@DepartmentID", id),
            new SqlParameter("@DepartmentCode", department.DepartmentCode),
            new SqlParameter("@DepartmentName", department.DepartmentName)
        };

        int rowsAffected = dbConnector.ExecuteNonQuery(query, parameters);

        if (rowsAffected > 0)
        {
            return Ok("Department updated successfully");
        }

        return NotFound("Department not found");
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteDepartment(int id)
    {
        string query = "DELETE FROM Departments WHERE DepartmentID = @DepartmentID";
        SqlParameter[] parameters = { new SqlParameter("@DepartmentID", id) };

        int rowsAffected = dbConnector.ExecuteNonQuery(query, parameters);

        if (rowsAffected > 0)
        {
            return Ok("Department deleted successfully");
        }

        return NotFound("Department not found");
    }
}
