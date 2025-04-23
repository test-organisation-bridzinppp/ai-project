using BuildingBlocks.Application.Ports;


namespace Infrastructure
{
    public class PdfRecognizer : IPdfRecognizer
    {
        private readonly string _endpoint;
        private readonly string _apiKey;

        public PdfRecognizer(string endpoint, string apiKey)
        {
            _endpoint = endpoint;
            _apiKey = apiKey;
        }

        public async Task<RecognizedDocument> Recognize(byte[] content)
        {
            // generate code using Azure Form recognizer
            return null;
        }
    }
}