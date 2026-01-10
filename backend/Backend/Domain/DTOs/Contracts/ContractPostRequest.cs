using System.Text.Json.Serialization;

namespace TrackForUBB.Domain.DTOs.Contracts;

public class ContractPostRequest
{
    [JsonPropertyName("promotionId")]
    public int PromotionId { get; set; }

    [JsonPropertyName("fields")]
    public required ContractFields Fields { get; set; }
}
