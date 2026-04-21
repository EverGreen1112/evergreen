using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotcakes_Desktop
{
    // Címek (számlázási és szállítási)
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

    // Egyedi tulajdonságok
    public class CustomPropertyDTO
    {
        public string DeveloperId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    // Címek (számlázási és szállítási)
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

    // Rendelés
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

        // Lista a beágyazott egyedi paraméterekhez
        public List<CustomPropertyDTO> CustomProperties { get; set; }
        public List<LineItemDTO> Items { get; set; }

        public int PaymentStatus { get; set; }
        public int ShippingStatus { get; set; }
        public bool IsPlaced { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }

        // AddressDTO, mint osztály
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

    // Ez a modell reprezentál egyetlen terméket (tételt) a rendelésen belül
    // Termék tétel modell
    public class LineItemDTO
    {
        public int Id { get; set; }
        public string OrderBvin { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public int Quantity { get; set; }
        public decimal BasePricePerItem { get; set; }
        public decimal AdjustedPricePerItem { get; set; }
        public decimal LineTotal { get; set; }
    }
}
