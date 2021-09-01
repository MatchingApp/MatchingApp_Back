using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchingApp_Back.Models;
using Nest;

public class Search
{
    public string Should { get; set; }
    public string ShouldNot { get; set; }
    public string Must { get; set; }
    public string MustNot { get; set; }
    public string Title { get; set; }
    public string Skills { get; set; }
    public string Location { get; set; }
    public string Experience { get; set; }
    public string Bio { get; set; }
}
namespace MatchingApp_Back.Controllers
{
    [Route("api/candidates")]
    [ApiController]
    public class CandidatController : ControllerBase
    {

        private readonly IElasticClient elasticClient;
        public CandidatController(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }

        [HttpGet]
        public async Task<IEnumerable<Candidat>> GetAll()
        {
            var response = await elasticClient.SearchAsync<Candidat>(s =>
            s.Size(100));
            return response?.Documents;
        }

        [HttpGet("byname/{name}", Name = "GetByName")]
       
        public async Task<IEnumerable<Candidat>> GetByName(string name)
        {
            var response = await elasticClient.SearchAsync<Candidat>(s => s
           .Query(q => q.Term(t => t.Name, name) ||
           q.Match(m => m.Field(f => f.Name).Query(name))));
            return response?.Documents;
        }

        // [HttpGet("match/{​​​skills}​​​/{​​​min}​​​", Name = "GetBySkills")]
        //public async Task<IEnumerable<Candidat>> GetBySkills(string skills, int min)
        //{​​​
        //var response = await elasticClient.SearchAsync<Candidat>(s => s
        //        .Query(q => q.Term(t => t.Skills, skills) || q.Match(m => m.Field(f => f.Skills).Query(skills).MinimumShouldMatch(min))));
        //            return response?.Documents;
        //}​​​


        [HttpPost("search", Name = "SearchCandidats")]
        public async Task<IEnumerable<Candidat>> SearchCandidats(Search value)
        {
           
            var response = await elasticClient.SearchAsync<Candidat>
                (s => s
                .Query(q => q
                     .Bool(b => b
                            .Must(
                                m => m.Term(p => p.Title, value.Title),
                                m => m.Match(m => m.Field(f => f.Skills).Query(value.Skills)) ,
                                m => m.Match(m => m.Field(f => f.Address).Query(value.Location)),
                                m => m.Term(p => p.Experience, value.Experience),
                                m => m.Term(p => p.Bio, value.Bio)
                                )
                            .Should(
                                bs => bs.Term(p => p.Name,value.Should)
                                )
                            .MustNot(
                               bs => bs.Term(p => p.Skills, value.MustNot)
                                )
                            //.Filter(f => f.MatchAll())
                            //.MinimumShouldMatch(1)
    
                        )
                     )
                );



            return response?.Documents;
        }


        [HttpPost("addcandidat" , Name= "addCandidat")]
        public async Task<string> addCandidat([FromBody] Candidat value)
        {
            Guid g = Guid.NewGuid();
            value.Id = g;
              var response = await elasticClient.IndexAsync<Candidat>(value, x => x.Index("candidats"));
            return response.Id;
        }


        [HttpPost("updatecandidat", Name = "updateCandidat")]
        public async Task<string> updateCandidat([FromBody] Candidat value)
        {
            var response = await elasticClient.UpdateAsync<Candidat>(value.Id, u => 
            u.Index("candidats")
            .Doc(value)
            );

            return response.Id;

        }

        [HttpDelete("deletecandidat/{id}", Name= "deleteCandidat")]
        public async Task<DeleteResponse> deleteCandidat(string id)
        {
            var response = await elasticClient.DeleteAsync<Candidat>(id, x => x.Index("candidats"));
            return response;
        }
    }
}
