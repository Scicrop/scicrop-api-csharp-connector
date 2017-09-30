// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    var data = ScicropEntity.FromJson(jsonString);
//
//
namespace SciCrop.AgroAPI.Connector.Entities
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class ScicropEntity
    {
        [JsonProperty("authEntity")]
        public AuthEntity AuthEntity { get; set; }

        [JsonProperty("actionEntityLst")]
        public object[] ActionEntityLst { get; set; }

        [JsonProperty("payloadEntity")]
        public PayloadEntity PayloadEntity { get; set; }

        [JsonProperty("responseEntity")]
        public ResponseEntity ResponseEntity { get; set; }
    }

    public partial class AuthEntity
    {
        [JsonProperty("userEntity")]
        public UserEntity UserEntity { get; set; }
    }

    public partial class UserEntity
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("api_id")]
        public string ApiId { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }
    }


    public partial class ScicropEntity
    {
        public static ScicropEntity FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ScicropEntity>(json, Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this ScicropEntity self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }

    public partial class PayloadEntity
    {

        [JsonProperty("freightLst")]
        public List<Freight> FreightLst { get; set; }

    }

    public partial class Freight
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("date")]
        public String Date { get; set; }

        [JsonProperty("currency")]
        public object Currency { get; set; }

        [JsonProperty("destinationCity")]
        public City DestinationCity { get; set; }

        [JsonProperty("load")]
        public Load Load { get; set; }

        [JsonProperty("priceKmTon")]
        public double PriceKmTon { get; set; }

        [JsonProperty("km")]
        public long Km { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("sourceCity")]
        public City SourceCity { get; set; }

        [JsonProperty("unit")]
        public Unit Unit { get; set; }
    }

    public partial class City
    {
        [JsonProperty("countryName")]
        public object CountryName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cityId")]
        public double CityId { get; set; }

        [JsonProperty("cityDistance")]
        public double CityDistance { get; set; }

        [JsonProperty("countryAcro")]
        public object CountryAcro { get; set; }

        [JsonProperty("location")]
        public object Location { get; set; }

        [JsonProperty("ibge_code")]
        public long IbgeCode { get; set; }

        [JsonProperty("locationId")]
        public double LocationId { get; set; }

        [JsonProperty("provinceId")]
        public double ProvinceId { get; set; }

        [JsonProperty("provinceAcro")]
        public string ProvinceAcro { get; set; }

        [JsonProperty("provinceName")]
        public object ProvinceName { get; set; }
    }

    public partial class Load
    {
        [JsonProperty("loadName")]
        public string LoadName { get; set; }

        [JsonProperty("id")]
        public double Id { get; set; }

        [JsonProperty("loadRaw")]
        public string LoadRaw { get; set; }
    }

    public partial class Unit
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public double Id { get; set; }

        [JsonProperty("useType")]
        public double UseType { get; set; }
    }

    public partial class ResponseEntity
    {
        [JsonProperty("id")]
        public double Id { get; set; }

        [JsonProperty("epochTime")]
        public long EpochTime { get; set; }

        [JsonProperty("returnId")]
        public long ReturnId { get; set; }

        [JsonProperty("returnMsg")]
        public string ReturnMsg { get; set; }
    }
}
