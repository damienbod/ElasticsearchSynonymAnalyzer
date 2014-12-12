﻿using System;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;

namespace ElasticsearchSynonymAnalyzer
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var elasticsearchMemberProvider = new ElasticsearchMemberProvider();

			// Create the index defintion
			var indexDefinition = elasticsearchMemberProvider.CreateNewIndexDefinitionForSynonymFilter();

			// create a new index and type mapping in elasticseach
			elasticsearchMemberProvider.CreateIndex(indexDefinition);

			Console.ReadLine();

			elasticsearchMemberProvider.CreateSomeMembers();

			Console.ReadLine();

			//{
			//  "query": {
			//	"query_string": {
			//	  "query": "jo*",
			//	  "analyzer": "my_analyzer"
			//	}
			//  }
			//}

		}
	}

	public class Member
	{
		public long Id { get; set; }

		[ElasticsearchString(Index = StringIndex.analyzed, Fields = typeof(FieldDataDefinition))]
		public string Name { get; set; }

		public string FamilyName { get; set; }

		public string Info { get; set; }
	}

	
	public class FieldDataDefinition
	{
		[ElasticsearchString(Index=StringIndex.not_analyzed)]
		public string Raw { get; set; }
	}
}
