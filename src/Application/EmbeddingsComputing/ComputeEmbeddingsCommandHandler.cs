using BuildingBlocks.Application.Ports;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.EmbeddingsComputing
{
    public class ComputeEmbeddingsCommandHandler : IRequestHandler<ComputeEmbeddingsCommand, int>
    {
        private readonly IStorage _storage;
        private readonly IPdfRecognizer _pdfRecognizer;
        private readonly ITextEmbeddingsProvider _textEmbeddingsProvider;
        private readonly IVectorDatabase _vectorDatabase;
        private readonly ILogger<ComputeEmbeddingsCommandHandler> _logger;
        private const int BatchSize = 10;

        public ComputeEmbeddingsCommandHandler(
            IStorage storage,
            IPdfRecognizer pdfRecognizer,
            ITextEmbeddingsProvider textEmbeddingsProvider,
            IVectorDatabase vectorDatabase,
            ILogger<ComputeEmbeddingsCommandHandler> logger)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _pdfRecognizer = pdfRecognizer ?? throw new ArgumentNullException(nameof(pdfRecognizer));
            _textEmbeddingsProvider = textEmbeddingsProvider ?? throw new ArgumentNullException(nameof(textEmbeddingsProvider));
            _vectorDatabase = vectorDatabase ?? throw new ArgumentNullException(nameof(vectorDatabase));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(ComputeEmbeddingsCommand request, CancellationToken cancellationToken)
        {
            var embeddings = new List<Embedding>();
            IEnumerable<string> files = Array.Empty<string>();

            try
            {
                _logger.LogInformation("Starting embedding computation process");
                files = await _storage.GetFilesNames(@"/documents", "pdf");

                if (!files.Any())
                {
                    _logger.LogInformation("No PDF files found to process");
                    return 0;
                }

                embeddings = await ProcessFiles(files, cancellationToken);
                await SaveEmbeddingsInBatches(embeddings, cancellationToken);

                if (embeddings.Any())
                {
                    await DeleteProcessedFiles(files);
                }

                _logger.LogInformation("Completed embedding computation. Processed {Count} embeddings", embeddings.Count);
                return embeddings.Count;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation was cancelled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error during embedding computation");
                throw;
            }
        }

        private async Task<List<Embedding>> ProcessFiles(IEnumerable<string> files, CancellationToken cancellationToken)
        {
            var embeddings = new List<Embedding>();
            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var fileEmbeddings = await ProcessSingleFile(file, cancellationToken);
                    embeddings.AddRange(fileEmbeddings);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Failed to process file {FileName}", file);
                }
            }
            return embeddings;
        }

        private async Task<List<Embedding>> ProcessSingleFile(string file, CancellationToken cancellationToken)
        {
            var fileEmbeddings = new List<Embedding>();
            var storageFile = await _storage.GetFile(@"/documents", file);
            var recognizedDocument = await _pdfRecognizer.Recognize(storageFile.Content);

            int pageNo = 0;
            foreach (var page in recognizedDocument.Pages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var computedPageEmbedding = await _textEmbeddingsProvider.GetEmbeddings(page);
                    fileEmbeddings.Add(new Embedding
                    {
                        FileName = file,
                        Page = pageNo,
                        Vectors = computedPageEmbedding,
                        PageContent = page
                    });
                    _logger.LogDebug("Processed page {PageNo} of file {FileName}", pageNo, file);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Failed to process page {PageNo} of file {FileName}", pageNo, file);
                }
                pageNo++;
            }
            return fileEmbeddings;
        }

        private async Task SaveEmbeddingsInBatches(List<Embedding> embeddings, CancellationToken cancellationToken)
        {
            for (int i = 0; i < embeddings.Count; i += BatchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var batch = embeddings.Skip(i).Take(BatchSize);
                await SaveEmbeddingsBatch(batch, cancellationToken);
            }
        }

        private async Task SaveEmbeddingsBatch(IEnumerable<Embedding> batch, CancellationToken cancellationToken)
        {
            foreach (var embedding in batch)
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
                    _logger.LogError(ex, 
                        "Failed to save embedding for file {FileName}, page {Page}", 
                        embedding.FileName, 
                        embedding.Page);
                }
            }
        }

        private async Task DeleteProcessedFiles(IEnumerable<string> files)
        {
            try
            {
                _storage.DeleteFile(@"/documents", files);
                _logger.LogInformation("Successfully deleted processed files");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete processed files");
            }
        }
    }
}