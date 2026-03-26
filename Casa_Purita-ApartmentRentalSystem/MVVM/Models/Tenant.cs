using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Casa_Purita_ApartmentRentalSystem.MVVM.Model
{
    public class FlexibleDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (DateTime.TryParse(str, out DateTime result))
                    return result;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                var ms = reader.GetInt64();
                return DateTimeOffset.FromUnixTimeMilliseconds(ms).DateTime;
            }

            reader.Skip();
            return DateTime.Today;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
        }
    }

    public class Tenant
    {
        // "id" is the exact key MockAPI returns — also handles "Id" via case-insensitive options
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("unitNumber")]
        public string UnitNumber { get; set; } = string.Empty;

        [JsonPropertyName("moveInDate")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime MoveInDate { get; set; }

        [JsonPropertyName("monthlyRent")]
        public decimal MonthlyRent { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; } = false;

        public string FullName => $"{FirstName} {LastName}";
    }
}