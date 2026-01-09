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

    [JsonPropertyName("optional-semester-1-package-1")]
    public int? OptionalSem1Pak1Id { get; set; }
    [JsonPropertyName("optional-semester-1-package-2")]
    public int? OptionalSem1Pak2Id { get; set; }
    [JsonPropertyName("optional-semester-1-package-3")]
    public int? OptionalSem1Pak3Id { get; set; }
    [JsonPropertyName("optional-semester-1-package-4")]
    public int? OptionalSem1Pak4Id { get; set; }
    [JsonPropertyName("optional-semester-1-package-5")]
    public int? OptionalSem1Pak5Id { get; set; }
    [JsonPropertyName("optional-semester-1-package-6")]
    public int? OptionalSem1Pak6Id { get; set; }

    [JsonPropertyName("optional-semester-2-package-1")]
    public int? OptionalSem2Pak1Id { get; set; }
    [JsonPropertyName("optional-semester-2-package-2")]
    public int? OptionalSem2Pak2Id { get; set; }
    [JsonPropertyName("optional-semester-2-package-3")]
    public int? OptionalSem2Pak3Id { get; set; }
    [JsonPropertyName("optional-semester-2-package-4")]
    public int? OptionalSem2Pak4Id { get; set; }
    [JsonPropertyName("optional-semester-2-package-5")]
    public int? OptionalSem2Pak5Id { get; set; }
    [JsonPropertyName("optional-semester-2-package-6")]
    public int? OptionalSem2Pak6Id { get; set; }

    [JsonPropertyName("phone")]
    public required string PhoneNumber { get; set; }

    [JsonPropertyName("yearNumber")]
    public int Year { get; set; }

    [JsonPropertyName("signature")]
    public string? SignatureBase64 { get; set;}
}
