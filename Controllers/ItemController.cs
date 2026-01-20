using adapterreviewpractice.Models;
using adapterreviewpractice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace adapterreviewpractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        List<Items> item = new List<Items>();
        private readonly IConfiguration _config;
        public ItemController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public IActionResult getitems()
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT Id, name, description FROM Products";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "Products");
                DataTable dt = ds.Tables["Products"];
                DataView dv = new DataView(dt);
                dv.RowFilter = "description='string'";
                dv.Sort = "price desc";
                //foreach (DataRowView row in dv)
                    foreach (DataRow row in dt.Rows)
                {
                    item.Add(new Items
                    {
                        Id = (int)row["id"],
                        name = row["name"].ToString(),
                        description = row["description"].ToString()
                    });
                }

                return Ok(item);
            }


        }
        [HttpPost]
        public IActionResult addnew(Items newitem)
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Products";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                SqlCommandBuilder cmd = new SqlCommandBuilder(adapter);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "Products");
                DataTable dt = ds.Tables["Products"];
                DataRow newRow = dt.NewRow();
                newRow["name"] = newitem.name;
                newRow["description"] = newitem.description;
                newRow["price"] = newitem.price;
                newRow["Products_makers"] = newitem.product_makers;
                dt.Rows.Add(newRow);
                adapter.Update(dt);

            }
            return Ok("Item added successsfully");
        }
        [HttpPut("{id}")]
        public IActionResult UpdateItem(int id, Items item)
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Products", con);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);

                DataSet ds = new DataSet();
                da.Fill(ds, "Products");

                DataTable dt = ds.Tables["Products"];

                // Update the row in memory
                foreach (DataRow row in dt.Rows)
                {
                    if ((int)row["Id"] == id)
                    {
                        row["Name"] = item.name;
                        row["Description"] = item.description;
                        break;
                    }

                }

                // Push changes to DB
                da.Update(dt);
            }

            return Ok("Item updated successfully!");
        }
        [HttpDelete("{id}")]
        public IActionResult DeletItem(int id, Items item)
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Products", con);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);

                DataSet ds = new DataSet();
                da.Fill(ds, "Products");

                DataTable dt = ds.Tables["Products"];

                // Update the row in memory
                foreach (DataRow row in dt.Rows)
                {
                    if ((int)row["Id"] == id)
                    {
                        row.Delete();
                    }

                }

                // Push changes to DB
                da.Update(dt);
            }

            return Ok("Item deleted successfully!");
        }


    }
}