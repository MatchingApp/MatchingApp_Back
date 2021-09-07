using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MatchingApp_Back.Models
{
    public class Candidat
    {
        
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Bio { get; set; }
        public int Phone { get; set; }
        public string  Address { get; set; }
        public string Languages { get; set; }
        public int Experience { get; set; }
        public string Education { get; set; }
        public string Skills { get; set; }
        public string Title { get; set; }
        public string Photo { get; set; }

        public double? Score { get; set; } = null;

    }
}
