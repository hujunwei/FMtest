﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace FMtest
{
    public class FaciliityMasterClient
    {
        /// <summary>
        /// The default address
        /// </summary>
        private const string FacilityMasterBaseUrl = "https://facilitymastertest.cloudapp.net/";
        /// <summary>
        /// The certficate thumbprint
        /// </summary>
        //private const string CertficateThumbprint = "4AE733DB8F10E8BCC555FF17AE3FCB8096234960";
        private const string CertficateThumbprint = "90128E7726EF898920B9CBBE724FCA3BE97C2ED2";



        /// <summary>
        /// Gets the qualifier value from facility master using JArray to query the target date.
        /// </summary>
        /// <param name="qualifierName">Name of the qualifier.</param>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// List of target result as string.
        /// </returns>
        public static List<string> GetQualifierValueFromFacilityMaster(string qualifierName, string url)
        {
            if (string.IsNullOrEmpty(qualifierName))
                throw new ArgumentException();

            if (string.IsNullOrEmpty(url))
                throw new ArgumentException();

            var result = new HashSet<string>();
            var jArrayResult = GetJArrayResultFromFacilityMaster(url);
            if (jArrayResult == null)
                throw new Exception("Parse to JArray fail");
            foreach (var jt in jArrayResult)
            {
                //string value = jArrayResult[i][qualifierName]["Name"].ToString();

                var item = jt;
                if (item == null) continue;
                var qualifierItem = jt[qualifierName];
                if (qualifierItem == null) continue;
                var qualifierItemName = jt[qualifierName]["Name"];
                if (qualifierItemName == null) continue;
                if (string.IsNullOrEmpty(qualifierItemName.ToString()))
                    continue;
                result.Add(qualifierItemName.ToString());
            }
            return result.ToList();
        }

        /// <summary>
        /// Gets the JArray from facility master.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// JArray of the response content.
        /// </returns>
        private static JArray GetJArrayResultFromFacilityMaster(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException();

            var response = QueryFacilityMaster(url);
            if (response == null)
                return null;
            var responseContent = response.Content.ReadAsStreamAsync().Result;
            JsonReader jsonReader = new JsonTextReader(new StreamReader(responseContent));
            var jArrayResult = (JArray)JToken.ReadFrom(jsonReader);
            return jArrayResult;
        }




        /// <summary>
        /// Gets the qualifier value from facility master using list of facility instance.
        /// </summary>
        /// <param name="qualifierName">Name of the qualifier.</param>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// List of target result as string.
        /// </returns>
        public static List<string> GetQualifierValueFromFacilityMaster2(string qualifierName, string url)
        {
            if (string.IsNullOrEmpty(qualifierName))
                throw new ArgumentException();
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException();

            var result = new HashSet<string>();
            var facilities = GetFacilityListFromFacilityMaster(url);
            if (qualifierName.Equals("AzureGeo"))
            {
                foreach (var f in facilities.Where(f => f.AzureGeo != null && f.AzureGeo.Name != null))
                {
                    result.Add(f.AzureGeo.Name);
                }
            }
            else if (qualifierName.Equals("AzureRegion"))
            {
                foreach (var f in facilities.Where(f => f.AzureRegion != null && f.AzureRegion.Name != null))
                {
                    result.Add(f.AzureRegion.Name);
                }
            }
            return result.ToList();
        }


        /// <summary>
        /// Gets the facility list from facility master.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// List of facility instance of the response content
        /// </returns>
        private static IEnumerable<Facility> GetFacilityListFromFacilityMaster(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException();

            var response = QueryFacilityMaster(url);
            if (response == null)
                return null;
            var responseContent = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<List<Facility>>(responseContent);
        }










        /// <summary>
        /// Gets the facility data that will be used for Qualifier.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>A List of string used for Qualifier value</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception">
        /// QueryFacilityMaster returns insuccess response
        /// </exception>
        public static List<string> GetFacilityData(string attributeType)
        {
            if (string.IsNullOrEmpty(attributeType))
                throw new ArgumentException();

            var result = new HashSet<string>();
            if (attributeType == "AzureGeo")
            {
                var response = QueryFacilityMaster("api/Attribute?Type=" + attributeType);
                if (response == null)
                    throw new Exception("QueryFacilityMaster returns insuccess response");

               
                //Parse response to JArray 
                var responseContent = response.Content.ReadAsStreamAsync().Result;
                JsonReader jsonReader = new JsonTextReader(new StreamReader(responseContent));
                var jArrayResult = (JArray)JToken.ReadFrom(jsonReader);

                foreach (var jToken in jArrayResult)
                {
                    if (jToken == null) continue;
                    var attributeItemName = jToken["Name"];
                    if (attributeItemName == null) continue;
                    if (string.IsNullOrEmpty(attributeItemName.ToString()))
                        continue;
                    result.Add(attributeItemName.ToString());
                }
            }
            else if (attributeType == "AzureRegion")
            {
                var response = QueryFacilityMaster("api/Attribute?Type=AzureGeo");
                if (response == null)
                    throw new Exception("QueryFacilityMaster returns insuccess response");

                //Parse response to JArray 
                var responseContent = response.Content.ReadAsStreamAsync().Result;
                JsonReader jsonReader = new JsonTextReader(new StreamReader(responseContent));
                var jArrayResult = (JArray)JToken.ReadFrom(jsonReader);
                foreach (var jToken in jArrayResult)
                {
                    if (jToken == null) continue;
                    var azureRegions = jToken["AzureRegions"];
                    if (azureRegions == null) continue;
                    foreach (var jToken2 in azureRegions)
                    {
                        //if (jToken2["Type"].ToString() != attributeType) continue;
                        var azureRegionName = jToken2["Name"];
                        if (string.IsNullOrEmpty(azureRegionName.ToString()))
                            continue;
                        result.Add(azureRegionName.ToString());
                    }
                }
            }
            return result.ToList();
        }











        /// <summary>
        /// Queries the result.
        /// </summary>
        /// <param name="facilityMasterApi">The facility master API.</param>
        /// <returns>
        /// The httpResponseMessage instance
        /// </returns>
        /// <exception cref="Exception">Certificate not found</exception>
        private static HttpResponseMessage QueryFacilityMaster(string facilityMasterApi)
        {
            if (string.IsNullOrEmpty(facilityMasterApi))
                throw new ArgumentException();

            var certificate = GetCertificateFromStore(CertficateThumbprint);


            if (certificate == null)
                throw new Exception("Certificate not found");

            //Setting the certificate and httpClient
            var handler = new WebRequestHandler();
            handler.ClientCertificates.Add(certificate);
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri(FacilityMasterBaseUrl) };


            //Request based on Facility Master api
            var response = httpClient.GetAsync(facilityMasterApi).Result;
            return !response.IsSuccessStatusCode ? null : response;
        }


        /// <summary>
        /// Gets the certificate from store.
        /// </summary>
        /// <param name="thumbprint">The thumbprint for the certificate.</param>
        /// <returns>
        /// The X509Certificate2 instance
        /// </returns>
        private static X509Certificate2 GetCertificateFromStore(string thumbprint)
        {
            if (string.IsNullOrEmpty(thumbprint))
                throw new ArgumentException();

            var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true);
            certStore.Close();
            return certCollection.Count > 0 ? certCollection[0] : null;
        }



    }
}
