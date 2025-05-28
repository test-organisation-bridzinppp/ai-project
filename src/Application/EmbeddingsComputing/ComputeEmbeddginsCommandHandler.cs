using BuildingBlocks.Application.Ports;
using Domain;
using Infrastructure;
using MediatR;

namespace Application.EmbeddingsComputing
{
    public class ComputeEmbeddginsCommandHandler(IStorage storage, IPdfRecognizer pdfRecognizer, ITextEmbeddingsProvider textEmbeddingsProvider, IVectorDatabase vectorDatabase) : IRequestHandler<ComputeEmbeddingsCommand, int>
    {
        private readonly IStorage _storage = storage;
        private readonly IPdfRecognizer _pdfRecognizer = pdfRecognizer;
        private readonly ITextEmbeddingsProvider _textEmbeddingsProvider = textEmbeddingsProvider;
        private readonly IVectorDatabase _vectorDatabase = vectorDatabase;

        public async Task<int> Handle(ComputeEmbeddingsCommand request, CancellationToken cancellationToken)
        {
            var embeddings = new List<Embedding>();
            IEnumerable<string> files = Array.Empty<string>();

            try
            {
                files = await _storage.GetFilesNames(@"/documents", "pdf");

                foreach (var file in files)
                {
                    try
                    {
                        var storageFile = await _storage.GetFile(@"/documents", file);
                        var recognizedDocument = await _pdfRecognizer.Recognize(storageFile.Content);
                        int pageNo = 0;

                        foreach (var page in recognizedDocument.Pages)
                        {
                            try
                            {
                                var computedPageEmbedding = await _textEmbeddingsProvider.GetEmbeddings(page);
                                embeddings.Add(new Embedding 
                                { 
                                    FileName = file, 
                                    Page = pageNo, 
                                    Vectors = computedPageEmbedding, 
                                    PageContent = page 
                                });
                                pageNo++;
                            }
                            catch (Exception ex) when (ex is not OperationCanceledException)
                            {
                                // Log error but continue processing other pages
                                // TODO: Add proper logging
                                Console.WriteLine($"Failed to process page {pageNo} of file {file}: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        // Log error but continue processing other files
                        // TODO: Add proper logging
                        Console.WriteLine($"Failed to process file {file}: {ex.Message}");
                    }
                }

                // Save successfully processed embeddings
                foreach (var embedding in embeddings)
                {
                    try
                    {
                        await _vectorDatabase.SaveDocument(
                            embedding.PageContent, 
                            embedding.Page.ToString(), 
                            embedding.Vectors.ToArray());
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        // Log error but continue saving other embeddings
                        // TODO: Add proper logging
                        Console.WriteLine($"Failed to save embedding for file {embedding.FileName}, page {embedding.Page}: {ex.Message}");
                    }
                }

                // Only delete files if we have successfully processed some embeddings
                if (embeddings.Any())
                {
                    _storage.DeleteFile(@"/documents", files);
                }

                return embeddings.Count;
            }
            catch (OperationCanceledException)
            {
                throw; // Propagate cancellation
            }
            catch (Exception ex)
            {
                // Log critical error
                // TODO: Add proper logging
                Console.WriteLine($"Critical error during embedding computation: {ex.Message}");
                throw; // Rethrow to maintain the error state
            }
        }
    }
}
