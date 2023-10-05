uusing System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Example.WebApi.Models
{
    public class Job
    {

            public Guid Id { get; set; }
            public string Name { get; set; }
            public int Salary { get; set; }
            public string Type { get; set; }

    }
}
