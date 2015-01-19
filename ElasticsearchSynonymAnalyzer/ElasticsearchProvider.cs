using System.Collections.Generic;
using ElasticsearchCRUD;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters;
using ElasticsearchCRUD.ContextSearch;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchSynonymAnalyzer
{
	public class ElasticsearchMemberProvider
	{
		public ElasticsearchMemberProvider()
		{
			_context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver))
			{
				TraceProvider = new ConsoleTraceProvider()
			};
		}
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost:9200";
		private readonly ElasticsearchContext _context;

		public IndexDefinition CreateNewIndexDefinitionForSynonymFilter()
		{
			return new IndexDefinition
			{
				IndexSettings =
				{
					Analysis = new Analysis
					{
						Analyzer =
						{
							Analyzers = new List<AnalyzerBase>
							{
								new CustomAnalyzer("john_analyzer")
								{
									Tokenizer = DefaultTokenizers.Whitespace,
									Filter = new List<string> {DefaultTokenFilters.Lowercase, "john_synonym"}
								}
							}
						},
						Filters =
						{
							CustomFilters = new List<AnalysisFilterBase>
							{
								new SynonymTokenFilter("john_synonym")
								{
									Synonyms = new List<string>
									{
										"sean  => john, sean, séan",
										"séan => john, sean, séan",
										"johny => john",
									}
								}
							}
						}
					},
					NumberOfShards = 3,
					NumberOfReplicas = 1
				},
			};

		}

		public void CreateIndex(IndexDefinition indexDefinition)
		{
			_context.IndexCreate<Member>(indexDefinition);
		}

		public void CreateSomeMembers()
		{
			var jm = new Member {Id = 1, FamilyName = "Moore", Info = "In the club since 1976", Name = "John"};
			_context.AddUpdateDocument(jm, jm.Id);
			var jj = new Member { Id = 2, FamilyName = "Jones", Info = "A great help for the background staff", Name = "Johny" };
			_context.AddUpdateDocument(jj, jj.Id);
			var pm = new Member { Id = 3, FamilyName = "Murphy", Info = "Likes to take control", Name = "Paul" };
			_context.AddUpdateDocument(pm, pm.Id);
			var sm = new Member { Id = 4, FamilyName = "McGurk", Info = "Fresh and fit", Name = "Séan" };
			_context.AddUpdateDocument(sm, sm.Id);
			var sob = new Member { Id = 5, FamilyName = "O'Brien", Info = "Not much use, bit of a problem", Name = "Sean" };
			_context.AddUpdateDocument(sob, sob.Id);
			var tmc = new Member { Id = 6, FamilyName = "McCauley", Info = "Couldn't ask for anyone better", Name = "Tadhg" };
			_context.AddUpdateDocument(tmc, tmc.Id);

			_context.SaveChanges();
		}

		//{
		//  "query": {
		//		"match": {"name": "sean"}
		//	 }
		//  }
		//}
		public SearchResult<Member> Search(string name)
		{
			var search = new Search
			{
				Query = new Query(new MatchQuery("name", name))
			};

			return _context.Search<Member>(search).PayloadResult;
		}

	}
}
