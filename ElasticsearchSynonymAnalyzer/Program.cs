using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

			elasticsearchMemberProvider.CreateSomeMembers();

			// TODO create search with expected results
		}
	}

	public class Member
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public string FamilyName { get; set; }

		public string Info { get; set; }
	}
}
