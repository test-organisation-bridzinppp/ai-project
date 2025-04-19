using BuildingBlocks.Application.Ports;

namespace Infrastructure
{
    public class PdfRecognizer : IPdfRecognizer
    {
        public Task<RecognizedDocument> Recognize(byte[] content)
        {
            throw new NotImplementedException();
            /*
            string endpoint = "<your-form-recognizer-endpoint>";
        string apiKey = "<your-form-recognizer-key>";
        string pdfPath = "sample.pdf"; // Replace with your PDF file path

        var credential = new AzureKeyCredential(apiKey);
        var client = new DocumentAnalysisClient(new Uri(endpoint), credential);

        using var stream = File.OpenRead(pdfPath);
        var operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-document", stream);
        var result = operation.Value;

        foreach (var page in result.Pages)
        {
            Console.WriteLine($"Page {page.PageNumber}:");

            foreach (var line in page.Lines)
            {
                Console.WriteLine(line.Content);
            }
        }
        }
            */
        }
    }
}