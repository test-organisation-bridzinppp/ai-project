using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using BuildingBlocks.Application.Ports;
using System.ComponentModel.DataAnnotations;
using System.Text;


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
            var client = new DocumentAnalysisClient(new Uri(_endpoint), new AzureKeyCredential(_apiKey));
            using var stream = new MemoryStream(content);
            var operationResult = await client.AnalyzeDocumentAsync(WaitUntil.Completed,"prebuilt-document", stream);
            var result = operationResult.Value;           
            var recognizedDocument = new RecognizedDocument(result.Pages.SelectMany(p => p.Lines.Select(l => l.Content)));
            return recognizedDocument;
        }
    }
}