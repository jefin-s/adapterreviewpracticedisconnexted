using adapterreviewpractice.Models;
using adapterreviewpractice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
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
                //dv.RowFilter = "description='string'";
                //dv.Sort = "price desc";
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
        [HttpGet("storedprocedure")]
        public IActionResult GetItemsUsingStoredProcedure()
        {
            List<Items> items = new List<Items>();
            string connectionString = _config.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("showprodcct", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                   SqlDataAdapter da= new SqlDataAdapter(cmd);

                    DataSet ds = new DataSet();
                    da.Fill(ds, "Products");
                    DataTable dt = ds.Tables["Products"];
                    foreach (DataRow row in dt.Rows)
                    {
                        items.Add(new Items
                        {
                            Id = (int)row["id"],
                            name = row["name"].ToString(),
                            description = row["description"].ToString()
                        });
                    }
                }
            }
            return Ok(items);
        }
        [HttpPost("sp add")]
        public IActionResult addsp(Items newitem)
        {
            string connection = _config.GetConnectionString("DefaultConnection");
            using (SqlConnection con=new SqlConnection(connection))
            {
                SqlCommand cmd = new SqlCommand( "addprodust",con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", newitem.name);
                cmd.Parameters.AddWithValue("@price", newitem.price);
                cmd.Parameters.AddWithValue("@description", newitem.description);
                cmd.Parameters.AddWithValue("@products_makkers", newitem.product_makers);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            return Ok("added successfully");
        }
        [HttpPut("id")]
        public IActionResult updaet(int id,Items updateitem)
        {
                string connection = _config.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(connection))
                {
                    SqlCommand cmd = new SqlCommand("up", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", updateitem.name);
                
                con.Open();
                int row=cmd.ExecuteNonQuery();
            }
            return Ok("added successfully");
        }
        [HttpGet("TotalProducts")]
        public IActionResult TotalaProducts()
        {
            int totalcount = 0;
            string cin = _config.GetConnectionString("Defaultconnection");
            using (SqlConnection con = new SqlConnection(cin))
            {
             
                SqlCommand cmd = new SqlCommand("tot", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter outputparams = new SqlParameter("@total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputparams);
                con.Open();
                cmd.ExecuteNonQuery();
              totalcount = (int)cmd.Parameters["@total"].Value;
                return Ok(totalcount);
            }
        }

    }
}