using System.Text.Json.Serialization;

namespace TrackForUBB.Domain.DTOs.Contracts;

public class ContractFields
{
    [JsonPropertyName("agree")]
    public required bool Agree { get; set; }

    [JsonPropertyName("cnp")]
    public required string CNP { get; set; }
    [JsonPropertyName("Email")]
    public required string Email { get; set; }
    [JsonPropertyName("fullName")]
    public required string FullName { get; set; }

    [JsonPropertyName("numar")]
    public required string IdCardNumber { get; set; }
    [JsonPropertyName("serie")]
    public required string IdCardSeries { get; set; }

    [JsonPropertyName("optional_0")]
    public int? Optional0Id { get; set; }
    [JsonPropertyName("optional_1")]
    public int? Optional1Id { get; set; }
    [JsonPropertyName("optional_2")]
    public int? Optional2Id { get; set; }
    [JsonPropertyName("optional_3")]
    public int? Optional3Id { get; set; }
    [JsonPropertyName("optional_4")]
    public int? Optional4Id { get; set; }
    [JsonPropertyName("optional_5")]
    public int? Optional5Id { get; set; }
    [JsonPropertyName("optional_6")]
    public int? Optional6Id { get; set; }

    [JsonPropertyName("phone")]
    public required string PhoneNumber { get; set; }

    [JsonPropertyName("yearNumber")]
    public int Year { get; set; }

    [JsonPropertyName("signature")]
    public string? SignatureBase64 { get; set;}
}
