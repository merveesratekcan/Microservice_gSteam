using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FilterService.Models;

namespace FilterService.Services
{
    public class FilterGameService : IFilterGameService
    {
        private readonly ElasticsearchClient _elasticsearchClient;
        public string indexName;

        public FilterGameService(ElasticsearchClient elasticsearchClient,IConfiguration configuration)
        {
            _elasticsearchClient = elasticsearchClient;
            indexName = configuration.GetValue<string>("indexName");
        }

        public async Task<List<GameFilterItem>> SearchAsync(GameFilterItem gameFilterItem)
        {
            List<Action<QueryDescriptor<GameFilterItem>>> listQuery = new();
            if(gameFilterItem is null)
            {
                listQuery.Add(q=>q.MatchAll());
                return await CalculateResultSet(listQuery);
            }
            if(!string.IsNullOrEmpty(gameFilterItem.Price.ToString()) && gameFilterItem.Price != 0)
            {
                listQuery.Add((q)=> q.Range(r => r.NumberRange(f => f.Field(p => p.Price).Gte(Convert.ToDouble(gameFilterItem.Price)))));
            }
            if(!string.IsNullOrEmpty(gameFilterItem.Price.ToString()) && gameFilterItem.Price != 0)
            {
                listQuery.Add((q)=> q.Range(r => r.NumberRange(f => f.Field(p => p.Price).Lte(Convert.ToDouble(gameFilterItem.Price)))));
            }

            if(!string.IsNullOrEmpty(gameFilterItem.MinimumSystemRequirement))
            {
                string searchValue="*"+gameFilterItem.MinimumSystemRequirement+"*";
                //wilcard elastic search içinden bir kelimeyi aramak için kullanılır.
                listQuery.Add((q)=>q.Wildcard(w=>w.Field(p=>p.MinimumSystemRequirement).Value(searchValue)));
            }
             if(!string.IsNullOrEmpty(gameFilterItem.RecommendedSystemRequirement))
            {
                string searchValue="*"+gameFilterItem.RecommendedSystemRequirement+"*";
                listQuery.Add((q)=>q.Wildcard(w=>w.Field(p=>p.RecommendedSystemRequirement).Value(searchValue)));
            }
            if(!listQuery.Any())
            {
                listQuery.Add(q=>q.MatchAll());
            }
           
           return await CalculateResultSet(listQuery);
        }
        private async Task<List<GameFilterItem>> CalculateResultSet(List<Action<QueryDescriptor<GameFilterItem>>> listQuery)
        {
            var result =await _elasticsearchClient.SearchAsync<GameFilterItem>(x=>x.Index(indexName).Query(q=>q.Bool(b=>b.Must(listQuery.ToArray()))));
            return result.Documents.ToList();
            
        }
    }
}