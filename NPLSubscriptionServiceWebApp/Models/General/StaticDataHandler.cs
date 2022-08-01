using Newtonsoft.Json;
using NPLDataAccessLayer.DataTransferObjects;
using System.Net;

namespace NPLSubscriptionServiceWebApp.Models.General
{
    public class StaticDataHandler
    {
        public static async Task<IEnumerable<ClientTypeDTO>> GetClientTypes(string baseUrl)
        {
            var requestUrl = $"{baseUrl}ClientType/GetAllClientTypes";
            IEnumerable<ClientTypeDTO> clientTypes = new List<ClientTypeDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    clientTypes = JsonConvert.DeserializeObject<IEnumerable<ClientTypeDTO>>(data);
                }
            }
            return clientTypes;
        }

        public static async Task<IEnumerable<ClientDTO>> GetClients(string baseUrl)
        {
            var requestUrl = $"{baseUrl}Client/GetAllClients";
            IEnumerable<ClientDTO> clients = new List<ClientDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    clients = JsonConvert.DeserializeObject<IEnumerable<ClientDTO>>(data);
                }
            }
            return clients;
        }

        public static async Task<IEnumerable<DistrictDTO>> GetDistricts(string baseUrl)
        {
            var requestUrl = $"{baseUrl}District/GetAllDistricts";
            IEnumerable<DistrictDTO> districts = new List<DistrictDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    districts = JsonConvert.DeserializeObject<IEnumerable<DistrictDTO>>(data);
                }
            }
            return districts;
        }
        //public string RandomPassword()
        //{
        //    PasswordGenerator passwordGenerator = new PasswordGenerator();press admin project****************************************@@@$$$$$$%%%^##########
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append(passwordGenerator.RandomString(4, true));
        //    builder.Append(passwordGenerator.RandomNumber(1000, 9999));
        //    builder.Append(passwordGenerator.RandomString(2, false));
        //    return builder.ToString();
        //}
        public static async Task<IEnumerable<PaymentDTO>> GetPayments(string baseUrl)
        {
            var requestUrl = $"{baseUrl}Payment/GetAllPayments";
            IEnumerable<PaymentDTO> districts = new List<PaymentDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    districts = JsonConvert.DeserializeObject<IEnumerable<PaymentDTO>>(data);
                }
            }
            return districts;
        }

        public static async Task<IEnumerable<PaymentTypeDTO>> GetPaymentTypes(string baseUrl)
        {
            var requestUrl = $"{baseUrl}PaymentType/GetAllPaymentTypes";
            IEnumerable<PaymentTypeDTO> paymentTypes = new List<PaymentTypeDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    paymentTypes = JsonConvert.DeserializeObject<IEnumerable<PaymentTypeDTO>>(data);
                }
            }
            return paymentTypes;
        }


        public static async Task<IEnumerable<PromotionDTO>> GetPromotions(string baseUrl)
        {
            var requestUrl = $"{baseUrl}Promotion/GetAllPromotions";
            IEnumerable<PromotionDTO> promotions = new List<PromotionDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    promotions = JsonConvert.DeserializeObject<IEnumerable<PromotionDTO>>(data);
                }
            }
            return promotions;
        } 
        
        public static async Task<IEnumerable<PublicationDTO>> GetPublications(string baseUrl)
        {
            var requestUrl = $"{baseUrl}Publication/GetAllPublications";
            IEnumerable<PublicationDTO> promotions = new List<PublicationDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    promotions = JsonConvert.DeserializeObject<IEnumerable<PublicationDTO>>(data);
                }
            }
            return promotions;
        }

        public static async Task<IEnumerable<TypeOfDeliveryDTO>> GetTypesOfDelivery(string baseUrl)
        {
            var requestUrl = $"{baseUrl}TypeOfDelivery/GetAllTypeOfDeliveries";
            IEnumerable<TypeOfDeliveryDTO> promotions = new List<TypeOfDeliveryDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    promotions = JsonConvert.DeserializeObject<IEnumerable<TypeOfDeliveryDTO>>(data);
                }
            }
            return promotions;
        }

        public static async Task<IEnumerable<RegionDTO>> GetRegions(string baseUrl)
        {
            var requestUrl = $"{baseUrl}Region/GetAllRegions";
            IEnumerable<RegionDTO> regions = new List<RegionDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    regions = JsonConvert.DeserializeObject<IEnumerable<RegionDTO>>(data);
                }
            }
            return regions;
        }

        public static async Task<IEnumerable<CountryDTO>> GetCountries(string baseUrl)
        {
            var requestUrl = $"{baseUrl}Country/GetAllCountries";
            IEnumerable<CountryDTO> regions = new List<CountryDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    regions = JsonConvert.DeserializeObject<IEnumerable<CountryDTO>>(data);
                }
            }
            return regions;
        }
        public static async Task<IEnumerable<SubscriptionTypeDTO>> GetSubscriptionTypes(string baseUrl)
        {
            var requestUrl = $"{baseUrl}SubscriptionType/GetAllSubscriptionTypes";
            IEnumerable<SubscriptionTypeDTO> subscriptionTypes = new List<SubscriptionTypeDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    subscriptionTypes = JsonConvert.DeserializeObject<IEnumerable<SubscriptionTypeDTO>>(data);
                }
            }
            return subscriptionTypes;
        }

        public static async Task<IEnumerable<SubscriptionStatusDTO>> GetSubscriptionStatuses(string baseUrl)
        {
            var requestUrl = $"{baseUrl}SubscriptionStatus/GetAllSubscriptionStatuses";
            IEnumerable<SubscriptionStatusDTO> subscriptionStatus = new List<SubscriptionStatusDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    subscriptionStatus = JsonConvert.DeserializeObject<IEnumerable<SubscriptionStatusDTO>>(data);
                }
            }
            return subscriptionStatus;
        }
    }
}
