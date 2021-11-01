using GoogleMaps.LocationServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace S2TAnalytics.ExistingDatasourcesELT.Helpers
{
 public static   class Location
    {
        public static string CountryCodeByCountryName(string countryName)
        {

            try
            {
                if (countryName == "United arab emirates")
                {
                    countryName = "U.A.E.";
                    var a = "";
                }
                var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID));
                var englishRegion = regions.FirstOrDefault(region => region.EnglishName.Contains(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(countryName)));
                if (englishRegion == null)
                {
                    return null;
                }
                else
                    return englishRegion.TwoLetterISORegionName;
            }
            catch (Exception ex)
            {

                throw;
            }
            //return "";

        }
        public static Dictionary<string, double> GetCityLongLat(string cityName, string countryName)
        {
            try
            {
                //if (countryName == "Australia")
                //{
                //    cityName = "New South Wales";
                //}
                var address = cityName + ", " + countryName;
                var locationService = new GoogleLocationService();
                var point = locationService.GetLatLongFromAddress(address);

                if (cityName == "N/A" || point == null)
                {
                    return new Dictionary<string, double>() {
                        { "Lat", 0 },
                        { "Long", 0 },
                    };
                }

                var latitude = point.Latitude;
                var longitude = point.Longitude;
                return new Dictionary<string, double>() {
                         { "Lat", latitude },
                         { "Long", longitude },
                    };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static string GetStateNameByLatLong(double latitude, double longitude)
        {
            var Address_administrative_area_level_1 = "";
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load("http://maps.googleapis.com/maps/api/geocode/xml?latlng=" + latitude + "," + longitude + "&sensor=false");
                XmlNode element = doc.SelectSingleNode("//GeocodeResponse/status");
                if (element.InnerText == "ZERO_RESULTS")
                {
                    return ("No data available for the specified location");
                }
                else
                {
                    element = doc.SelectSingleNode("//GeocodeResponse/result/formatted_address");

                    string longname = "";
                    string shortname = "";
                    string typename = "";
                    bool fHit = false;

                    XmlNodeList xnList = doc.SelectNodes("//GeocodeResponse/result/address_component");
                    foreach (XmlNode xn in xnList)
                    {
                        try
                        {
                            longname = xn["long_name"].InnerText;
                            shortname = xn["short_name"].InnerText;
                            typename = xn["type"].InnerText;

                            fHit = true;
                            switch (typename)
                            {
                                case "administrative_area_level_1":
                                    {
                                        Address_administrative_area_level_1 = longname;
                                        break;
                                    }
                                default:
                                    fHit = false;
                                    break;
                            }

                            if (fHit)
                            {
                                Console.Write(typename);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("\tL: " + longname + "\tS:" + shortname + "\r\n");
                                Console.ForegroundColor = ConsoleColor.Gray;
                            }
                        }

                        catch (Exception e)
                        {
                            //Node missing either, longname, shortname or typename
                            fHit = false;
                            Console.Write(" Invalid data: ");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("\tX: " + xn.InnerXml + "\r\n");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                    }
                    //Console.ReadKey();
                    return (element.InnerText);
                }
            }
            catch (Exception ex)
            {
                return ("(Address lookup failed: ) " + ex.Message);
            }
        }
    }
}
