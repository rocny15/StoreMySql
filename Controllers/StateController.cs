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

        public async Task<IActionResult> Edit(int? id)
        {
            var state = new State();
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();  // Asegurarse de que la conexión sea asincrónica

                    string query = "SELECT * FROM states WHERE id_state = @Id";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    state.IdState = reader.GetInt32("id_state");
                                    state.Name = reader.GetString("name");
                                    state.Initials = reader.GetString("initials");
                                }
                            }
                        }
                    }
                }
                if (state == null)
                {
                    return NotFound();
                }

                return View(state);
            }
            catch (MySqlException ex)
            { 
                ModelState.AddModelError("", "Ocurrió un error durante la edición del estado. Por favor, inténtalo de nuevo más tarde.");
                return View(state);  
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error desconocido durante la edición del estado. Por favor, inténtalo de nuevo más tarde.");
                return View(state); 
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, State updatedState)
        {
            if (id != updatedState.IdState)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        await connection.OpenAsync();

                        string updateQuery = "UPDATE states SET name = @Name, initials = @Initials WHERE id_state = @Id";
                        using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@Id", updatedState.IdState);
                            updateCommand.Parameters.AddWithValue("@Name", updatedState.Name);
                            updateCommand.Parameters.AddWithValue("@Initials", updatedState.Initials);

                            int rowsAffected = updateCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                return RedirectToAction("Index"); 
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error durante la actualización del estado. Por favor, inténtalo de nuevo más tarde.");
                }
                catch (Exception ex)
                { 
                    ModelState.AddModelError("", "Ocurrió un error desconocido durante la actualización del estado. Por favor, inténtalo de nuevo más tarde.");
                }
            }
            return View(updatedState);
        }

    }
}
