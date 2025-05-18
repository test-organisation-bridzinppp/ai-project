using Application.EmbeddingsComputing;
using BuildingBlocks.Application.Ports;
using Infrastructure;
using Moq;
using Xunit;

public class ComputeEmbeddginsCommandHandlerTests
{
    private readonly Mock<IStorage> _storageMock;
    private readonly Mock<IPdfRecognizer> _pdfRecognizerMock;
    private readonly Mock<ITextEmbeddingsProvider> _textEmbeddingsProviderMock;
    private readonly ComputeEmbeddginsCommandHandler _handler;
    private readonly Mock<IVectorDatabase> _vectorDatabaseMock;

    public ComputeEmbeddginsCommandHandlerTests()
    {
        _storageMock = new Mock<IStorage>();
        _pdfRecognizerMock = new Mock<IPdfRecognizer>();
        _textEmbeddingsProviderMock = new Mock<ITextEmbeddingsProvider>();
        _vectorDatabaseMock = new Mock<IVectorDatabase>();
        _handler = new ComputeEmbeddginsCommandHandler(
            _storageMock.Object,
            _pdfRecognizerMock.Object,
            _textEmbeddingsProviderMock.Object,
            _vectorDatabaseMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnFileCount_WhenFilesAreProcessed()
    {
        // Arrange
        var files = new List<string> { "file1.pdf", "file2.pdf" };
        var recognizedDocument = new RecognizedDocument(new List<string> { "Page 1 content", "Page 2 content" });
        
        var embeddings = new List<float> { 0.1f, 0.2f, 0.3f };

        _storageMock.Setup(s => s.GetFilesNames("/documents", "pdf"))
            .ReturnsAsync(files);
        _storageMock.Setup(s => s.GetFile("/documents", "file1.pdf"))
            .ReturnsAsync(new StorageFile("file1.pdf", new byte[0]));
        _storageMock.Setup(s => s.GetFile("/documents", "file2.pdf"))
            .ReturnsAsync(new StorageFile("file2.pdf", new byte[0]));
        _pdfRecognizerMock.Setup(p => p.Recognize(It.IsAny<byte[]>()))
            .ReturnsAsync(recognizedDocument);
        _textEmbeddingsProviderMock.Setup(t => t.GetEmbeddings(It.IsAny<string>()))
            .ReturnsAsync(embeddings);

        var command = new ComputeEmbeddingsCommand();

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        Assert.Equal(2, result);
        _storageMock.Verify(s => s.GetFilesNames("/documents", "pdf"), Times.Once);
        _storageMock.Verify(s => s.GetFile("/documents", It.IsAny<string>()), Times.Exactly(2));
        _pdfRecognizerMock.Verify(p => p.Recognize(It.IsAny<byte[]>()), Times.Exactly(2));
        _textEmbeddingsProviderMock.Verify(t => t.GetEmbeddings(It.IsAny<string>()), Times.Exactly(4));
    }
}
