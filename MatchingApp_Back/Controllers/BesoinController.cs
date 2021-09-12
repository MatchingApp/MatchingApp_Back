using MatchingApp_Back.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchingApp_Back.Controllers
{
    [Route("api/besoins")]
    [ApiController]
    public class BesoinController : ControllerBase
    {

        private readonly IElasticClient elasticClient;
        public BesoinController(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }
        [HttpGet]
        public async Task<IEnumerable<Besoin>> GetAll()
        {
            var response = await elasticClient.SearchAsync<Besoin>(s =>
            s.Index("besoins")
            .Size(100));
            return response?.Documents;
        }
        [HttpPost("addbesoin", Name = "addBesoin")]
        public async Task<string> addBesoin([FromBody] Besoin value)
        {
            Guid g = Guid.NewGuid();
            value.Id = g;
            var response = await elasticClient.IndexAsync<Besoin>(value, x => x.Index("besoins"));
            return response.Id;
        }
        [HttpPost("updatebesoin", Name = "updateBesoin")]
        public async Task<string> updateBesoin([FromBody] Besoin value)
        {
            var response = await elasticClient.UpdateAsync<Besoin>(value.Id, u =>
            u.Index("besoins")
            .Doc(value)
            );

            return response.Id;

        }
        [HttpDelete("deletebesoin/{id}", Name = "deleteBesoin")]
        public async Task<DeleteResponse> deleteBesoin(string id)
        {
            var response = await elasticClient.DeleteAsync<Besoin>(id, x => x.Index("besoins"));
            return response;
        }

    }
}
