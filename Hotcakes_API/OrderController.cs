using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Hotcakes_API
{
    namespace videoAPI.Controllers
    {
        // Modellek (json alapján) 
        public class HotcakesResponse<T>
        {
            public T Content { get; set; }
            public List<HotcakesError> Errors { get; set; }
        }

        public class HotcakesError
        {
            public string Code { get; set; }
            public string Description { get; set; }
        }

        // 1. A beágyazott egyedi tulajdonságok modellje
        public class CustomPropertyDTO
        {
            public string DeveloperId { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }

        // 2. A címek modellje (számlázási és szállítási cím)
        public class AddressDTO
        {
            public string Bvin { get; set; }
            public string LastUpdatedUtc { get; set; }
            public int StoreId { get; set; }
            public string NickName { get; set; }
            public string FirstName { get; set; }
            public string MiddleInitial { get; set; }
            public string LastName { get; set; }
            public string Company { get; set; }
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string Line3 { get; set; }
            public string City { get; set; }
            public string RegionName { get; set; }
            public string RegionBvin { get; set; }
            public string PostalCode { get; set; }
            public string CountryName { get; set; }
            public string CountryBvin { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public string WebSiteUrl { get; set; }
            public string UserBvin { get; set; }
            public int AddressType { get; set; }
        }

        // 3. A rendelés modell
        public class OrderDTO
        {
            public int Id { get; set; }
            public string bvin { get; set; }
            public int StoreId { get; set; }
            public string LastUpdatedUtc { get; set; }
            public string TimeOfOrderUtc { get; set; }
            public string OrderNumber { get; set; }
            public string ThirdPartyOrderId { get; set; }
            public string UserEmail { get; set; }
            public string UserID { get; set; }

            // Beágyazott egyedi paraméterek listában
            public List<CustomPropertyDTO> CustomProperties { get; set; }

            public int PaymentStatus { get; set; }
            public int ShippingStatus { get; set; }
            public bool IsPlaced { get; set; }
            public string StatusCode { get; set; }
            public string StatusName { get; set; }

            // AddressDTO osztály típusként
            public AddressDTO BillingAddress { get; set; }
            public AddressDTO ShippingAddress { get; set; }

            // Pénz és adó
            public decimal ItemsTax { get; set; }
            public decimal ShippingTax { get; set; }
            public decimal TotalTax { get; set; }
            public decimal TotalOrderBeforeDiscounts { get; set; }
            public decimal TotalShippingBeforeDiscounts { get; set; }
            public decimal TotalShippingDiscounts { get; set; }
            public decimal TotalOrderDiscounts { get; set; }
            public decimal TotalHandling { get; set; }
            public decimal TotalGrand { get; set; }

            public string AffiliateID { get; set; }
            public decimal FraudScore { get; set; }
            public string Instructions { get; set; }
            public string ShippingMethodId { get; set; }
            public string ShippingMethodDisplayName { get; set; }
            public string ShippingProviderId { get; set; }
            public string ShippingProviderServiceCode { get; set; }
        }

        // 2. CRUDE műveletek
        [Route("api/orders")]
        [ApiController]
        public class OrderController : ControllerBase
        {
            private readonly HttpClient httpClient;

            // Cseréld ki a saját adataidra!
            private readonly string apiKey = "1-c82617a1-53ab-41ce-bc85-d4d1efc12f9e";
            private readonly string baseUrl = "http://74.234.38.79/DesktopModules/Hotcakes/API/rest/v1/";

            public OrderController()
            {
                httpClient = new HttpClient();
            }

            // GET: api/orders
            [HttpGet]
            public async Task<IActionResult> Get()
            {
                var response = await httpClient.GetAsync($"{baseUrl}orders?key={apiKey}");
                if (!response.IsSuccessStatusCode) return StatusCode(500, "Hiba a Hotcakes API elérésekor.");

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<HotcakesResponse<List<OrderDTO>>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Ha a Hotcakes hibát dob (pl. rossz kulcs), akkor azt adjuk vissza a Swaggernek!
                if (result != null && result.Errors != null && result.Errors.Count > 0)
                {
                    return BadRequest(result.Errors);
                }

                return Ok(result?.Content);
            }

            // GET api/orders/{id}
            [HttpGet("{id}")]
            public async Task<IActionResult> Get(string id)
            {
                var response = await httpClient.GetAsync($"{baseUrl}orders/{id}?key={apiKey}");
                if (!response.IsSuccessStatusCode) return StatusCode(500, "Hiba a Hotcakes API elérésekor.");

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<HotcakesResponse<OrderDTO>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result?.Content == null) return NotFound("Nincs ilyen rendelés.");

                return Ok(result.Content);
            }

            // POST api/orders
            [HttpPost]
            public async Task<IActionResult> Post([FromBody] OrderDTO newOrder)
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(newOrder), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{baseUrl}orders?key={apiKey}", jsonContent);
                if (!response.IsSuccessStatusCode) return StatusCode(500, "Hiba a létrehozáskor.");

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<HotcakesResponse<OrderDTO>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Ok(result?.Content);
            }

            // DELETE api/orders/{id}
            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(string id)
            {
                var response = await httpClient.DeleteAsync($"{baseUrl}orders/{id}?key={apiKey}");
                if (!response.IsSuccessStatusCode) return StatusCode(500, "Hiba a törléskor.");

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<HotcakesResponse<bool>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result != null && result.Content == false) return BadRequest("Nem létezik, vagy nem törölhető.");

                return Ok("Sikeres törlés!");
            }
        }
    }
}