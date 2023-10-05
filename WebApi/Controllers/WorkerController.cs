uusing Example.WebApi.Models;
using Npgsql;
using NpgsqlTypes;
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
            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB";
            connection = new NpgsqlConnection(connectionString);
            connection.Open();
        }

        // GET api/worker
        public HttpResponseMessage GetWorkers()
        {
            try
            {
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
                                TypeOfJob = reader.GetString(1)
                            };
                            workersList.Add(worker);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, workersList);
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
                                TypeOfJob = reader.GetString(1)
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
                using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO \"Worker\" (\"Id\", \"TypeOfJob\") VALUES (@WorkerId, @TypeOfJob)", connection))
                {
                    command.Parameters.AddWithValue("@WorkerId", worker.Id);
                    command.Parameters.AddWithValue("@TypeOfJob", worker.TypeOfJob);
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
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // PUT api/worker/5
        public HttpResponseMessage Put(int id, [FromBody] Worker worker)
        {
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand("UPDATE Worker SET typeOfJob = @TypeOfJob,  WHERE workerId = @id", connection))
                {
                    command.Parameters.AddWithValue("@TypeOfJob", worker.TypeOfJob);
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@id", id);
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
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE api/worker/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Worker WHERE workerId = @id", connection))
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
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
