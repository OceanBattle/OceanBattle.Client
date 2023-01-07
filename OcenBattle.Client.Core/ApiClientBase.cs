using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;

namespace OcenBattle.Client.Core
{
    public abstract class ApiClientBase
    {
        protected readonly MediaTypeFormatter _jsonFormatter;
        protected readonly IEnumerable<MediaTypeFormatter> _formatters;
        protected readonly HttpClient _httpClient;

        public ApiClientBase(HttpClient httpClient)
        {
            _jsonFormatter = new JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    //DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    //DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    TypeNameHandling = TypeNameHandling.Auto,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    ContractResolver = new DefaultContractResolver(),
                    //Converters = new List<JsonConverter> { new Iso8601TimeSpanConverter() }
                }
            };

            _formatters = new List<MediaTypeFormatter> { _jsonFormatter };
            _httpClient = httpClient;
        }
    }
}
