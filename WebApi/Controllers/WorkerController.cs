// WorkerController

using Example.WebApi.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Example.WebApi.Controllers
{
    public class WorkerController : ApiController
    {
        private NpgsqlConnection connection;

        public WorkerController()
        {
        
        }

        // GET api/worker
        public HttpResponseMessage GetWorkers()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB"))
                {
                    connection.Open();
                    
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM \"Worker\";", connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            List<Worker> workersList = new List<Worker>();
                            while (reader.Read())
                            {
                                Worker worker = new Worker
                                {
                                    Id = reader.GetGuid(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Gender = reader.GetChar(3)
                                };
                                workersList.Add(worker);
                            }
                            return Request.CreateResponse(HttpStatusCode.OK, workersList);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET api/worker/5
        public HttpResponseMessage GetWorker(Guid id)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB"))
                {
                    connection.Open();
                    
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM \"Worker\" WHERE \"Id\" = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Worker worker = new Worker
                                {
                                    Id = reader.GetGuid(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Gender = reader.GetChar(3)
                                };
                                return Request.CreateResponse(HttpStatusCode.OK, worker);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.NotFound, "Worker not found");
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST api/worker
        public HttpResponseMessage Post([FromBody] Worker worker)
        {
            try
            {
                worker.Id = Guid.NewGuid();
                using (NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB"))
                {
                    connection.Open();
                    
                    using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO \"Worker\" (\"Id\", \"FirstName\", \"LastName\", \"Gender\") VALUES (@WorkerId, @FirstName, @LastName, @Gender)", connection))
                    {
                        command.Parameters.AddWithValue("@WorkerId", worker.Id);
                        command.Parameters.AddWithValue("@FirstName", worker.FirstName);
                        command.Parameters.AddWithValue("@LastName", worker.LastName);
                        command.Parameters.AddWithValue("@Gender", worker.Gender);
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, worker);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Error inserting worker");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // PUT api/worker/5
        public HttpResponseMessage Put(Guid id, [FromBody] Worker worker)
        {
            try
            {
                // Instanciram connection objekt unutar metode
                using (NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB"))
                {
                    connection.Open();
                    
                    using (NpgsqlCommand getCommand = new NpgsqlCommand("SELECT * FROM \"Worker\" WHERE \"Id\" = @id", connection))
                    {
                        getCommand.Parameters.AddWithValue("@id", id);

                        using (NpgsqlDataReader reader = getCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Ažuriram promijenjena polja
                                worker.FirstName = reader.GetString(1);
                                worker.LastName = reader.GetString(2);
                                worker.Gender = reader.GetChar(3);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.NotFound, "Worker not found");
                            }
                        }
                    }
                    
                    using (NpgsqlCommand updateCommand = new NpgsqlCommand("UPDATE \"Worker\" SET \"FirstName\" = @FirstName, \"LastName\" = @LastName, \"Gender\" = @Gender WHERE \"Id\" = @id", connection))
                    {
                        updateCommand.Parameters.AddWithValue("@FirstName", worker.FirstName);
                        updateCommand.Parameters.AddWithValue("@LastName", worker.LastName);
                        updateCommand.Parameters.AddWithValue("@Gender", worker.Gender);
                        updateCommand.Parameters.AddWithValue("@id", id);

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, true);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Worker not found");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE api/worker/5
        public HttpResponseMessage Delete(Guid id)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB"))
                {
                    connection.Open();
                    
                    using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM \"Worker\" WHERE \"Id\" = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, true);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Worker not found");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // PUT api/worker/setjob/5
        [Route("api/worker/setjob/{workerId}")]
        public HttpResponseMessage SetJob(Guid workerId, [FromBody] Guid jobId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB"))
                {
                    connection.Open();
                    
                    using (NpgsqlCommand command = new NpgsqlCommand("UPDATE \"Worker\" SET \"JobId\" = @JobId WHERE \"Id\" = @WorkerId", connection))
                    {
                        command.Parameters.AddWithValue("@JobId", jobId);
                        command.Parameters.AddWithValue("@WorkerId", workerId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, true);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Worker not found");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
