namespace BallastLane.Test.Application.Common
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Items { get; set; }

        public int TotalRecords { get; set; }
    }
}
