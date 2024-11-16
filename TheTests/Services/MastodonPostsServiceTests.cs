using Mastonet;
using Mastonet.Entities;
using NSubstitute;
using TeletekstBotHangfire.Models.Ef;
using TeletekstBotHangfire.Services;
using TeletekstBotHangfire.Utils;

namespace TheTests.Services;

[TestFixture]
public class MastodonPostsServiceTests
{
    private IMastodonClient _mockMastodonClient;
    private MastodonPostsService _service;
    private const double FloatingPointTolerance = 0.01;

    [SetUp]
    public void Setup()
    {
        _mockMastodonClient = Substitute.For<IMastodonClient>();
        _service = new MastodonPostsService(_mockMastodonClient);
    }

    [Test]
    public async Task SendTeletekstPageAsync_ShouldUploadMediaAndPublishStatus()
    {
        // Arrange
        var page = new TeletekstPage
        {
            Screenshot = [0x1, 0x2, 0x3],
            Title = "Test Page",
            Content = "Test Content"
        };

        var mockAttachment = new Attachment { Id = "12345" };

        _mockMastodonClient.UploadMedia(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<AttachmentFocusData>())
            .Returns(Task.FromResult(mockAttachment));

        // Act
        await _service.SendTeletekstPageAsync(page);

        // Assert
        await _mockMastodonClient.Received(1).UploadMedia(
            Arg.Any<Stream>(),
            "screenshot.jpg",
            TeletekstPageUtils.AltText(page),
            Arg.Is<AttachmentFocusData>(focus => focus.X == 0.0 && Math.Abs(focus.Y - 1.0) < FloatingPointTolerance)
        );

        await _mockMastodonClient.Received(1).PublishStatus(
            TeletekstPageUtils.TitleForSocialMedia(page),
            Visibility.Unlisted,
            null,
            Arg.Is<List<string>>(list => list.Count == 1 && list[0] == mockAttachment.Id)
        );
    }
}