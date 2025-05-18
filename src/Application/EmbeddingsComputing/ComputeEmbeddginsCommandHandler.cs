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
            var files = await _storage.GetFilesNames("/documents", "pdf");
            
            foreach (var file in files)
            {
                var storageFile = await _storage.GetFile("/documents", file);
                var recognizedDocument = await _pdfRecognizer.Recognize(storageFile.Content);
                int pageNo = 0;

                foreach (var page in recognizedDocument.Pages)
                {
                    var computedPageEmbedding = await _textEmbeddingsProvider.GetEmbeddings(page);
                    embeddings.Add(new Embedding { FileName = file, Page = pageNo, Vectors = computedPageEmbedding, PageContent = page });
                    pageNo++;
                }
            }
            
            foreach (var embedding in embeddings)
            {
                await _vectorDatabase.SaveDocument(embedding.PageContent, embedding.Page.ToString(), embedding.Vectors.ToArray());
            }

            _storage.DeleteFile("/documents", files);

            return files.Count();
        }
    }
    
}
