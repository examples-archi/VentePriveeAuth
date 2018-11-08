using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VentePriveeAuth.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
    }
}
