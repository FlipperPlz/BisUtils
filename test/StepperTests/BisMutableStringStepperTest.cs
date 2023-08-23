namespace StepperTests;

using BisUtils.Core.ParsingFramework.Extensions;
using BisUtils.Core.ParsingFramework.Steppers.Mutable;
using Moq;

[TestFixture]
public class BisMutableStringStepperTest
{

    private const string TestData = "The quick, brown fox jumps over the lazy dog.";
    private Mock<Microsoft.Extensions.Logging.ILogger> logger = null!;

    [SetUp]
    public void Setup() => logger = new Mock<Microsoft.Extensions.Logging.ILogger>();

    [Test]
    public void TestMoveForward()
    {
        using var stepper = new BisMutableStringStepper(TestData, logger.Object);
        stepper.ScanUntil(it => it == ' ');
        Assert.That(stepper.Position, Is.EqualTo(3));
        var quickStart = stepper.Position + 1;
        Assert.That(stepper.ScanUntil(it => it == ' '), Is.EqualTo("quick, "));
        var quickEnd = stepper.Position - 1;
        Assert.That(stepper.Position, Is.EqualTo(10));
        stepper.ReplaceRange(quickStart..quickEnd, "lazy", out var replaced);
        Assert.Multiple(() =>
        {
            Assert.That(replaced, Is.EqualTo("quick"));
            Assert.That(stepper.MoveBackwardMulti(4), Is.EqualTo("lazy"));
            Assert.That(stepper.ToString(), Is.EqualTo("The lazy, brown fox jumps over the lazy dog."));
        });
    }
}
