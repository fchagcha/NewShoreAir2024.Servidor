using MediatR;
using Moq;
using NewShoreAir.Api.Controllers;
using NewShoreAir.Business.Application.Models;

namespace NewShoreAir.Api.Test
{
    public class CalcularRutaTest
    {
        [Test]
        public async Task TestCalcularRutaRequest()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var controller = new CalcularRutaController(mediatorMock.Object);
            var request = new CalcularRutaRequest()
            {
                Origen = "MZL",
                Destino = "BCN",
                NumeroMaximoDeVuelos = 0
            };

            // Act
            var result = await controller.CalcularRuta(request);

            // Assert
            mediatorMock.Verify(m => m.Send(request, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}