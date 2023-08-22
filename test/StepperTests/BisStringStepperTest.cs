namespace StepperTests;

using BisUtils.Core.ParsingFramework.Steppers.Immutable;
using Moq;

[TestFixture]
public class BisStringStepperTest
{
    private const string TestData = "Test Step";
    private Mock<Microsoft.Extensions.Logging.ILogger> logger = null!;

    [SetUp]
    public void Setup() => logger = new Mock<Microsoft.Extensions.Logging.ILogger>();

    [Test]
    public void TestMoveForward()
    {
        using var stepper = new BisStringStepper(TestData, logger.Object);
        Assert.Multiple(() =>
        {
            Assert.That(stepper.PreviousChar, Is.EqualTo(null));
            Assert.That(stepper.CurrentChar, Is.EqualTo(null));
            Assert.That(stepper.MoveForward(), Is.EqualTo('T'));
            Assert.That(stepper.CurrentChar, Is.EqualTo('T'));
            Assert.That(stepper.MoveForward(), Is.EqualTo('e'));
            Assert.That(stepper.MoveForward(), Is.EqualTo('s'));
            Assert.That(stepper.MoveForward(), Is.EqualTo('t'));
            Assert.That(stepper.MoveForward(), Is.EqualTo(' '));
            Assert.That(stepper.MoveForward(), Is.EqualTo('S'));
            Assert.That(stepper.MoveForward(), Is.EqualTo('t'));
            Assert.That(stepper.MoveForward(), Is.EqualTo('e'));
            Assert.That(stepper.MoveForward(), Is.EqualTo('p'));
        });
    }

    [Test]
    public void TestLinearMovement()
    {
        using var stepper = new BisStringStepper(TestData, logger.Object);


        stepper.JumpTo(stepper.Length);
        Assert.Multiple(() =>
        {
            Assert.That(stepper.CurrentChar, Is.EqualTo('p'));
            Assert.That(stepper.PreviousChar, Is.EqualTo('e'));
            Assert.That(stepper.MoveBackward(), Is.EqualTo('e'));
            Assert.That(stepper.MoveBackward(), Is.EqualTo('t'));
            Assert.That(stepper.MoveBackward(), Is.EqualTo('S'));
            Assert.That(stepper.MoveBackward(), Is.EqualTo(' '));
            Assert.That(stepper.MoveForward(), Is.EqualTo('S'));
            Assert.That(stepper.MoveForward(), Is.EqualTo('t'));
            Assert.That(stepper.MoveForward(), Is.EqualTo('e'));
            Assert.That(stepper.MoveForward(), Is.EqualTo('p'));
        });

    }
}
