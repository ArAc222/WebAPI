// JobController

using Example.WebApi.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Example.WebApi.Controllers
{
    public class JobController : ApiController
    {
        private NpgsqlConnection connection;

        public JobController()
        {
        
        }

        // GET api/job
        public HttpResponseMessage GetJobs()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB"))
                {
                    connection.Open();
                    
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM \"Job\";", connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            List<Job> jobsList = new List<Job>();
                            while (reader.Read())
                            {
                                Job job = new Job
                                {
                                    Id = reader.GetGuid(0),
                                    Name = reader.GetString(1),
                                    Salary = reader.GetInt32(2),
                                    Type = reader.GetString(3)
                                };
                                jobsList.Add(job);
                            }
                            return Request.CreateResponse(HttpStatusCode.OK, jobsList);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET api/job/5
        public HttpResponseMessage GetJob(Guid id)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB"))
                {
                    connection.Open();
                    
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM \"Job\" WHERE \"Id\" = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Job job = new Job
                                {
                                    Id = reader.GetGuid(0),
                                    Name = reader.GetString(1),
                                    Salary = reader.GetInt32(2),
                                    Type = reader.GetString(3)
                                };
                                return Request.CreateResponse(HttpStatusCode.OK, job);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.NotFound, "Job not found");
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

        // POST api/job
        public HttpResponseMessage Post([FromBody] Job job)
        {
            try
            {
                job.Id = Guid.NewGuid();
                using (NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB"))
                {
                    connection.Open();
                    
                    using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO \"Job\" (\"Id\", \"Name\", \"Salary\", \"Type\") VALUES (@Id, @Name, @Salary, @Type)", connection))
                    {
                        command.Parameters.AddWithValue("@Id", job.Id);
                        command.Parameters.AddWithValue("@Name", job.Name);
                        command.Parameters.AddWithValue("@Salary", job.Salary);
                        command.Parameters.AddWithValue("@Type", job.Type);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, job);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Error inserting job");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // PUT api/job/5
        public HttpResponseMessage Put(Guid id, [FromBody] Job job)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB"))
                {
                    connection.Open();
                    
                    using (NpgsqlCommand getCommand = new NpgsqlCommand("SELECT * FROM \"Job\" WHERE \"Id\" = @id", connection))
                    {
                        getCommand.Parameters.AddWithValue("@id", id);

                        using (NpgsqlDataReader reader = getCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Ažuriram promijenjena polja
                                job.Name = reader.GetString(1);
                                job.Salary = reader.GetInt32(2);
                                job.Type = reader.GetString(3);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.NotFound, "Job not found");
                            }
                        }
                    }
                    
                    using (NpgsqlCommand updateCommand = new NpgsqlCommand("UPDATE \"Job\" SET \"Name\" = @Name, \"Salary\" = @Salary, \"Type\" = @Type WHERE \"Id\" = @id", connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Name", job.Name);
                        updateCommand.Parameters.AddWithValue("@Salary", job.Salary);
                        updateCommand.Parameters.AddWithValue("@Type", job.Type);
                        updateCommand.Parameters.AddWithValue("@id", id);

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, true);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Job not found");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE api/job/5
        public HttpResponseMessage Delete(Guid id)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB"))
                {
                    connection.Open();
                    
                    using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM \"Job\" WHERE \"Id\" = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, true);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Job not found");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        
        // GET api/job/workers/5
        [Route("api/job/workers/{jobId}")]
        public HttpResponseMessage GetWorkersForJob(Guid jobId)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=student1;Database=workerDB"))
                {
                    connection.Open();
                    
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM \"Worker\" WHERE \"JobId\" = @JobId", connection))
                    {
                        command.Parameters.AddWithValue("@JobId", jobId);

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
                                    Gender = reader.GetChar(3),
                                    JobId = reader.GetGuid(4) // Dodajte JobId za povezivanje s poslom
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
    }
}
