using NUnit.Framework;
using EpiLastic.Helpers;
using EpiLastic.Indexing.Services;

namespace EpiLastic.Test.For_SuggestionHelper
{
    [TestFixture]
    public class when_GeneratePageSuggestions
    {
        private ISuggestionHelper _suggestionHelper;

        public when_GeneratePageSuggestions()
        {
            _suggestionHelper = new SuggestionHelper();
        }

        [Test]
        public void for_suggestionhelper_when_GeneratePageSuggestions_it_should_generate_suggestions()
        {
            var result = _suggestionHelper.GeneratePageSuggestions("pagename");

            Assert.That(result[0] == "pagename");
        }

        [Test]
        public void for_suggestionhelper_when_GeneratePageSuggestions_multiterms_it_should_generate_suggestions()
        {
            var result = _suggestionHelper.GeneratePageSuggestions("a pagename is");

            Assert.That(result[0] == "a");
            Assert.That(result[1] == "pagename");
            Assert.That(result[2] == "is");
            Assert.That(result[3] == "a pagename is");
        }

        [Test]
        public void for_suggestionhelper_when_GeneratePageSuggestions_max_terms_it_should_generate_suggestions()
        {
            var result = _suggestionHelper.GeneratePageSuggestions("a pagename is the best");

            Assert.That(result[0] == "a");
            Assert.That(result[1] == "pagename");
            Assert.That(result[2] == "is");
            Assert.That(result[3] == "the");
            Assert.That(result[4] == "best");
            Assert.That(result[5] == "a pagename is the best");
        }

        [Test]
        public void for_suggestionhelper_when_GeneratePageSuggestions_empty_it_should_generate_suggestions()
        {
            var result = _suggestionHelper.GeneratePageSuggestions("");
        }

        [Test]
        public void for_suggestionhelper_when_GeneratePageSuggestions_over_size_limit_it_should_generate_suggestions_and_concatenade_extras()
        {
            var result = _suggestionHelper.GeneratePageSuggestions("a pagename is the best extra");

            Assert.That(result[0] == "a");
            Assert.That(result[1] == "pagename");
            Assert.That(result[2] == "is");
            Assert.That(result[3] == "the");
            Assert.That(result[4] == "best extra");
            Assert.That(result[5] == "a pagename is the best extra");
        }
    }
}
