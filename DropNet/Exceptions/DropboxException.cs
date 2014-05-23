using System;
using System.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace DropNet.Exceptions
{
    public class DropboxException : Exception
    {
        public DropboxException()
        {
            
        }

        public DropboxException(string message) : base(message)
        {
        }
    }

    public class DropboxRestException : DropboxException
    {
        /// <summary>
        /// Returned status code from the request
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
		
        /// <summary>
        /// Expected status codes to have seen instead of the one recieved. 
        /// </summary>
        public HttpStatusCode[] ExpectedCodes { get; private set; }
		
        /// <summary>
        /// The response of the error call (for Debugging use)
        /// </summary>
        public IRestResponse Response { get; private set; }
        
        public DropboxRestException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a DropboxRestException with the rest response which caused the exception, and the status codes which were expected. 
        /// </summary>
        /// <param name="r">Rest Response which was not expected.</param>
        /// <param name="expectedCodes">The expected status codes which were not found.</param>
        public DropboxRestException(IRestResponse r, params HttpStatusCode[] expectedCodes)  
        {
            Response = r;
			StatusCode = r.StatusCode;
            ExpectedCodes = expectedCodes;
        }

        /// <summary>
        /// Overridden message for Dropbox Exception. 
        /// <returns>
        /// The exception message in the format of "Received Response [{0}] : Expected to see [{1}]. The HTTP response was [{2}].
        /// </returns>
        /// </summary>
	    public override string Message
	    {
		    get
		    {
                string msg = String.Format("[{0}]", Response.StatusCode);
				Dictionary<string, string> parsedJson = null;
                try
                {
			RestSharp.Deserializers.JsonDeserializer r = new RestSharp.Deserializers.JsonDeserializer();
			parsedJson = r.Deserialize<Dictionary<string, string>>(Response);
                }
                catch (SerializationException) { }

                if (parsedJson != null && parsedJson.ContainsKey("error") && parsedJson["error"] != null)
                    msg += ": " + parsedJson["error"];
                else
                {
                    if (Response.ErrorException != null)
                        msg += Response.ErrorMessage;
                    else
                        msg += Response.Content;
                }
                return msg;
		    }
	    }        
    }
}
