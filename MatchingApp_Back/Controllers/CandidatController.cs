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
    public int MinExperience { get; set; }
    public int MaxExperience { get; set; }

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
        
        [HttpGet("score", Name = "GetScore")]
       
        public  ActionResult GetScore( )
        {

            var response = elasticClient.Search<Candidat>(s => s
          .Index("candidats")
          .Query(esQuery => esQuery
              .MoreLikeThis(mlt => mlt
                      .Include(true)
                      .Fields(f => f.Field(ff => ff.Skills))
                      .Like(l => l
                      .Text("css html angular")
                      )
                      .MinTermFrequency(1)
                      .MinDocumentFrequency(1)
              ) 
          )
      );
            // var response = await elasticClient.SearchAsync<Candidat>(s => s
            //.Query(q => q.Term(t => t.Name, ) ||
            //q.Match(m => m.Field(f => f.Name).Query())));
            //var response =  elasticClient.Search<Candidat>(q => q
            //      .Index("candidats")
            //            .Query(q => q
            //                //.Fuzzy(c => c
            //                //    .Field(p => p.Bio)
            //                //    .Value("learn")
            //                ////    )


            //                .MoreLikeThis(sn => sn
            //                    .Like(l => l
            //                        .Text("css html   ")
            //                    )
            //                   .Fields(f => f.Field(p => p.Skills))
            //                    //.Analyzer("some_analyzer")
            //                    //.BoostTerms(1.1)
            //                    //.Include()
            //                    //.MaxDocumentFrequency(12)
            //                    //.MaxQueryTerms(12)
            //                    //.MaxWordLength(300)
            //                    //.MinDocumentFrequency(1)
            //                    //.MinTermFrequency(1)
            //                    //.MinWordLength(10)
            //                    //.MinimumShouldMatch(1)

            //                    //.Unlike(l => l
            //                    //    .text("not like this text")

            //                    )
            //                //    ) &&
            //                //    q .Range(c => c
            //                //   .Field(p => p.Experience)
            //                //   .GreaterThanOrEquals(4)
            //                //   .LessThanOrEquals(20)
            //                //   )
            //                //&& q.Match(m => m.Field(f => f.Skills).Query("css html"))

            //                )
            //     );
            var results = response.Documents;
            var maxScore = response.MaxScore;
            var hits = response.Hits;
            foreach (Candidat result in results)
            {
        
                int index = results.ToList().FindIndex(m=> m.Id == result.Id);
                result.Score = Math.Round((hits.ToList()[index].Score * 100 / maxScore).Value, 2) ; 
            }
            return Ok(results);
        }
        
        
        [HttpGet("skills", Name = "GetSkills")]
       
        public   ActionResult GetSkills( )
        {
            string[] skills = { };
            var response =  elasticClient.Search<Candidat>(s =>
            s.Size(100));
            var results = response.Documents;

            foreach (Candidat result in results)
            {
                string[] newSkills =  result.Skills.Split(" ");
                skills = skills.Concat(newSkills).Distinct().ToArray();
            }

            return Ok(skills);
        }

        // [HttpGet("match/{​​​skills}​​​/{​​​min}​​​", Name = "GetBySkills")]
        //public async Task<IEnumerable<Candidat>> GetBySkills(string skills, int min)
        //{​​​
        //var response = await elasticClient.SearchAsync<Candidat>(s => s
        //        .Query(q => q.Term(t => t.Skills, skills) || q.Match(m => m.Field(f => f.Skills).Query(skills).MinimumShouldMatch(min))));
        //            return response?.Documents;
        //}​​​


        [HttpPost("search", Name = "SearchCandidats")]
        public  ActionResult SearchCandidats(Search value)
        {
           
            var response =  elasticClient.Search<Candidat>
                (s => s
                .Query(q => q
                     .Bool(b => b
                            //.Must(
                            //    m => m.Term(p => p.Title, value.Title),
                            //    m => m.Match(m => m.Field(f => f.Skills).Query(value.Skills)),
                            //    m => m.Match(m => m.Field(f => f.Address).Query(value.Location)),
                            //    q => q.MoreLikeThis(mlt => mlt
                            //              .Include(true)
                            //              .Fields(f => f.Field(ff => ff.Bio))
                            //              .Like(l => l
                            //              .Text(value.Bio)
                            //              )
                            //              .MinTermFrequency(1)
                            //              .MinDocumentFrequency(1)
                            //             )
                               
                            //    //m => m.Term(p => p.Bio, value.Bio)
                            //    )
                            .Should(


                                m => m.Term(p => p.Title, value.Title),
                                m => m.Match(m => m.Field(f => f.Skills).Query(value.Skills)),
                                m => m.Match(m => m.Field(f => f.Address).Query(value.Location)),
                                 q => q.MoreLikeThis(mlt => mlt
                                          .Include(true)
                                          .Fields(f => f.Field(ff => ff.Bio))
                                          .Like(l => l
                                          .Text(value.Bio)
                                          )
                                          .MinTermFrequency(1)
                                          .MinDocumentFrequency(1)
                                         ),

                                q => q
                                .Range(c => c
                                   .Field(p => p.Experience)
                                   .GreaterThanOrEquals(value.MinExperience)
                                   .LessThanOrEquals(value.MaxExperience)
                                   ),
                                q => q.Fuzzy(c => c
                                    .Field(p => p.Bio)
                                    .Fuzziness(Fuzziness.Auto)
                                    .Value(value.Bio)
                                    .MaxExpansions(100)
                                    .PrefixLength(3)
                                    .Rewrite(MultiTermQueryRewrite.ConstantScore)
                                    .Transpositions())
                                

                                )
                            //.MustNot(
                            //   bs => bs.Term(p => p.Skills, value.MustNot)
                            //    )
                            //.Filter(f => f.MatchAll())
                            //.MinimumShouldMatch(1)
    
                        )
                     )
                );

            var results = response.Documents;
            var maxScore = response.MaxScore;
            var hits = response.Hits;
            foreach (Candidat result in results)
            {

                int index = results.ToList().FindIndex(m => m.Id == result.Id);
                result.Score = Math.Round((hits.ToList()[index].Score * 100 / maxScore).Value, 2);
            }
          
            return Ok(results);
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
