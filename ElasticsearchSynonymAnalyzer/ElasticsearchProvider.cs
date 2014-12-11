using System.Collections.Generic;
using ElasticsearchCRUD;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters;
using ElasticsearchCRUD.Model;
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
								new CustomAnalyzer("my_analyzer")
								{
									Tokenizer = DefaultTokenizers.Standard,
									Filter = new List<string> {DefaultTokenFilters.Standard, DefaultTokenFilters.Lowercase, "john_synonym"}
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
										"sean  => john",
										"séan => john",
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

			_context.SaveChanges();
		}

	}
}
