namespace TrackForUBB.Domain.DTOs;

public class PromotionOfUserResponse
{
    public class Promotion
    {
        public int Id { get; set; }
        public required string PrettyName { get; set; }
        public int YearStart { get; set; }
        public int YearEnd { get; set; }
        public required int YearDuration { get; set; }
    }

    public required List<Promotion> Promotions { get; set; }
}
