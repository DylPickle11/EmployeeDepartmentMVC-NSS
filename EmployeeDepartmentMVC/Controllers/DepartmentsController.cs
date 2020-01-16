using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EmployeeDepartmentMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EmployeeDepartmentMVC.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;
        public DepartmentsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Department
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT * FROM Department";

                    var reader = cmd.ExecuteReader();

                    var departments = new List<Department>();

                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("DeptName"))
                        });
                    }
                    reader.Close();
                    return View(departments);
                }
            }
        }

        // GET: Department/Details/5    
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id,
                                        e.FirstName,
                                        e.LastName,
                                        d.Id AS DepartmentId,
                                        d.DeptName
                                        FROM Department d
                                        LEFT JOIN Employee e ON d.Id = e.DepartmentId
                                        WHERE d.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();

                    Department department = null;
                    List<Employee> employees = new List<Employee>();
                    Employee employee = null;


                    while (reader.Read())
                    {
                        if (department != null)
                        {

                            department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("DeptName")),
                                Employees = employees
                            };

                            var hasEmployee = !reader.IsDBNull(reader.GetOrdinal("Id"));

                            if (hasEmployee)
                            {
                                department.Employees.Add(new Employee
                                {
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                });
                            }
                        }
                        reader.Close();
                        return View(department);
                    }

                    if (department == null)
                    {
                        return NotFound();
                    }

                    return View(department);

                }
                {
                    reader.Close();
                    return NotFound();
                }

            }
        }
    }

    // GET: Department/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: Department/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Department department)
    {
        try
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Department 
                                          (DeptName) 
                                          VALUES (@DeptName)";

                    cmd.Parameters.Add(new SqlParameter("@DeptName", department.Name));

                    cmd.ExecuteNonQuery();

                }
            }
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: Department/Edit/5
    public ActionResult Edit(int id)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT Id, DeptName
                                      FROM Department
                                      WHERE Id = @Id";
                cmd.Parameters.Add(new SqlParameter("@id", id));

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    var department = new Department
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("DeptName"))
                    };
                    reader.Close();
                    return View(department);
                }
                reader.Close();
                return NotFound();
            }
        }
    }

    // POST: Department/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, Department department)
    {
        try
        {
            // TODO: Add update logic here
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Department 
                                            SET DeptName = @DeptName
                                            WHERE Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@DeptName", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            return View();
        }
    }

    // GET: Department/Delete/5
    public ActionResult Delete(int id)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT DeptName, Id FROM Department WHERE Id = @id";

                cmd.Parameters.Add(new SqlParameter("@id", id));

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    var department = new Department
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("DeptName"))
                    };
                    reader.Close();
                    return View(department);
                }
                return NotFound();
            }
        }
    }


    // POST: Department/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteDelete(int id)
    {
        try
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Department WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();

                    return RedirectToAction(nameof(Index));
                }
            }
        }
        catch
        {
            return View();
        }
    }
}
}