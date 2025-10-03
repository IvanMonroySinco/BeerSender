namespace BeerSender.Domain.Boxes.Commands;

public record CreateBox(
    Guid BoxId,
    int DesiredNumberOfSpots 
);

public class CreateBoxHandler(IEventStore eventStore)
    : CommandHandler<CreateBox>(eventStore)
{
    public override void Handle(CreateBox command)
    {
        var boxStream = GetStream<Box>(command.BoxId);
        var box = boxStream.GetEntity();
        
        if (box.IsCreated)
        {
            boxStream.Append(new FailedToCreateBox(FailedToCreateBox.FailReason.BoxItWasAlreadyCreated));
            return;
        }
        
        var capacity = BoxCapacity.Create(command.DesiredNumberOfSpots);
        boxStream.Append(new BoxCreated(capacity));
    }
}