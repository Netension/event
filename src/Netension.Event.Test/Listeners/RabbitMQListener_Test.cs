namespace Netension.Event.Test.Listeners
{
    //public class RabbitMQEventListener_Test
    //{
    //    private readonly ILogger<RabbitMQListener> _logger;
    //    private Mock<IConnection> _connectionMock;
    //    private RabbitMQListenerOptions _listenerOptions;
    //    private Mock<IRabbitMQEventReceiver> _eventReceiverMock;

    //    public RabbitMQEventListener_Test(ITestOutputHelper outputHelper)
    //    {
    //        _logger = new LoggerFactory()
    //                    .AddXUnit(outputHelper)
    //                    .CreateLogger<RabbitMQListener>();
    //    }

    //    private RabbitMQEventListener CreateSUT()
    //    {
    //        _connectionMock = new Mock<IConnection>();
    //        _listenerOptions = new RabbitMQListenerOptions();
    //        _eventReceiverMock = new Mock<IRabbitMQEventReceiver>();

    //        return new RabbitMQEventListener(_connectionMock.Object, _listenerOptions, _eventReceiverMock.Object, _logger);
    //    }

    //    [Fact(DisplayName = "RabbitMQEventListener - ListenAsync - Create channel")]
    //    public async Task RabbitMQListener_StartAsync_CreateChannel()
    //    {
    //        // Arrange
    //        var sut = CreateSUT();

    //        _connectionMock.Setup(c => c.CreateModel())
    //            .Returns(new Mock<IModel>().Object);

    //        _listenerOptions.Setup(lo => lo.Value)
    //            .Returns(new RabbitMQListenerOptions());

    //        // Act
    //        await sut.ListenAsync(CancellationToken.None);

    //        // Assert
    //        _connectionMock.Verify(c => c.CreateModel(), Times.Once);
    //    }

    //    [Fact(DisplayName = "RabbitMQEventListener - ListenAsync - Consume")]
    //    public async Task RabbitMQListener_StartAsync_Consume()
    //    {
    //        // Arrange
    //        var sut = CreateSUT();
    //        var channelMock = new Mock<IModel>();
    //        var options = new RabbitMQListenerOptions
    //        {
    //            Exchange = "TestExchange",
    //            Queue = "TestQueue",
    //            Tag = "TestTag"
    //        };

    //        _connectionMock.Setup(c => c.CreateModel())
    //            .Returns(channelMock.Object);

    //        _listenerOptions.Setup(lo => lo.Value)
    //            .Returns(options);

    //        // Act
    //        await sut.ListenAsync(CancellationToken.None);

    //        // Assert
    //        channelMock.Verify(c => c.BasicConsume(It.Is<string>(q => q.Equals(options.Queue)), It.Is<bool>(p => !p), It.Is<string>(t => t.Equals(options.Tag)), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<IBasicConsumer>()), Times.Once);
    //    }
    //}
}
