using System;
using System.IO;
using Newtonsoft.Json;


namespace FMtest
{
    class Program
    {
        static void Main(string[] args)
        {


            var resultAzureGeo = FaciliityMasterClient.GetQualifierValueFromFacilityMaster("AzureGeo", "api/Facility");
            var resultAzureGeo2 = FaciliityMasterClient.GetQualifierValueFromFacilityMaster2("AzureGeo", "api/Facility");
            var resultAzureRegion = FaciliityMasterClient.GetQualifierValueFromFacilityMaster("AzureRegion", "api/Facility");
            var resultAzureRegion2 = FaciliityMasterClient.GetQualifierValueFromFacilityMaster2("AzureRegion", "api/Facility");

            
            using (var file = new StreamWriter(@"C:\Users\junweihu\Desktop\AzureGeo.txt"))
            {
                file.Write(JsonConvert.SerializeObject(resultAzureGeo));
            }

            using (var file = new StreamWriter(@"C:\Users\junweihu\Desktop\AzureGeo2.txt"))
            {
                file.Write(JsonConvert.SerializeObject(resultAzureGeo2));
            }
            using (var file = new StreamWriter(@"C:\Users\junweihu\Desktop\AzureRegion.txt"))
            {
                file.Write(JsonConvert.SerializeObject(resultAzureRegion));
            }

            using (var file = new StreamWriter(@"C:\Users\junweihu\Desktop\AzureRegion2.txt"))
            {
                file.Write(JsonConvert.SerializeObject(resultAzureRegion2));
            }

            Console.WriteLine("Finish!");
            Console.ReadLine();


        }


    




       



    }
}
