using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using StoreMySql.Models;

namespace StoreMySql.Controllers
{
    public class StateController : Controller
    {
        private readonly IConfiguration _configuration;
        public StateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
       
        public IActionResult Index()
        {
            List<State> states = new List<State>();
            using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                string query = "SELECT * FROM states ORDER BY name";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                State state = new State
                                {
                                    IdState = reader.GetInt32("id_state"),
                                    Name = reader.GetString("name"),
                                    Initials = reader.GetString("initials")
                                };

                                states.Add(state);
                            }
                        }
                    }
                }
            }
            return View(states);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Initials")] CreateState state)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        connection.Open();

                        string query = "INSERT INTO `states` (`name`, `initials`) VALUES (@Name, @Initials)";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Name", state.Name);
                            command.Parameters.AddWithValue("@Initials", state.Initials);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                return RedirectToAction(nameof(Index));
                            }
                            else
                            {
                                ModelState.AddModelError("", "No se pudo insertar el estado.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
                ModelState.AddModelError("", "Ocurrió un error durante la creación del estado.");
            }

            return View(state);
        }
    }
}
