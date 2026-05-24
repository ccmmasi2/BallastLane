namespace BallastLane.Test.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entity, int id)
            : base($"{entity} with id {id} was not found.") { }

        public NotFoundException(string message)
            : base(message) { }
    }
}
