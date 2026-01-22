using Best.HTTP;

namespace Rexee.AppCore.Repositories.Implementation.Extensions
{
    /// <summary>
    /// Extension method for <see cref="HTTPRequest"/> class.
    /// </summary>
    public static class HttpRequestExtensions
    {

        private const string ContentTypeHeaderName = "Content-Type";
        private const string ApplicationJsonContentTypeHeaderValue = "application/json; charset=utf-8";

        /// <summary>
        /// Adds 'Content-Type' header with value 'application/json; charset=utf-8'.
        /// </summary>
        /// <param name="self">The <see cref="HTTPRequest"/> instance.</param>
        public static void SetJsonContentTypeHeader(this HTTPRequest self)
        {
            self.SetHeader(ContentTypeHeaderName, ApplicationJsonContentTypeHeaderValue);
        }

    }
}
