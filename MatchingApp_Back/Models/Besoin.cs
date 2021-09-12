using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchingApp_Back.Models
{
    public class Besoin
    {
        public Guid Id { get; set; }
        public string JobDescription { get; set; }
        public string JobLocation { get; set; }
        public string RequiredLanguages { get; set; }
        public int RequiredExperience { get; set; }
        public string RequiredEducation { get; set; }
        public string RequiredSkills { get; set; }
        public string JobTitle { get; set; }
    }
}
