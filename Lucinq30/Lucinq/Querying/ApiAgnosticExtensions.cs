using Lucene.Net.Search;
using Lucene.Net.Documents;

namespace Lucinq.Querying
{
    /// <summary>
    /// The api agnostic extensions.
    /// </summary>
    public static class ApiAgnosticExtensions
    {
        /// <summary>
        /// The get document index.
        /// </summary>
        /// <param name="doc">
        /// The doc.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetDocumentIndex(this ScoreDoc doc)
        {
            return doc.Doc;
        }

        /// <summary>
        /// Gets the value from a field
        /// </summary>
        /// <param name="field">The field</param>
        /// <returns></returns>
        public static string GetValue(this Field field)
        {
            return field.StringValue;
        }
    }
}
