
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;



namespace Calender2.Data
{
    public enum TimeZoneValues
    {
        Hawaii,
        PST,
        MST,
        CST,
        EST,
        UK,
        India,
        Malaysia,
        WAU,
        SAU,
        NT,
        QSLND,
        NSW,
        ACT,
        Victoria,
        NZ,
        Fiji,
        Unknown,
        MaxTimeZoneValue
    };

    public class StateOrCity
    {
        public String _Name;
    };

    public class City : StateOrCity
    {
        public String _UrlToken;// Valid if its a city
        public TimeZoneValues _timeZone;
        public City(String Name, String UrlToken)
        {
            _Name = Name;
            _UrlToken = UrlToken;
            _timeZone = TimeZoneValues.Unknown;
        }
        public City(String Name, String UrlToken, TimeZoneValues timeZone)
        {
            _Name = Name;
            _timeZone = timeZone;
            _UrlToken = UrlToken;
        }
    };

    public class State : StateOrCity
    {
        public City[] _cities; // Valid if its a state
        public TimeZoneValues _timeZone;
        public State(String Name, City[] cities, TimeZoneValues timeZone)
        {
            _Name = Name;
            _cities = cities;
            _timeZone = timeZone;
        }
    };

    public class SubContinent
    {
        public String _Name;
        public StateOrCity[] _stateOrCityList;
        public TimeZoneValues _timeZone;
        public SubContinent(String name, StateOrCity[] stateOrCityList, TimeZoneValues timeZone)
        {
            _Name = name;
            _stateOrCityList = stateOrCityList;
            _timeZone = timeZone;
        }
    };

    public class LatLong
    {
        public String _UrlToken;
        public double _Latitude;
        public double _Longtitude;
        public LatLong(String urlToken, double latitude, double longtitude)
        {
            _UrlToken = urlToken;
            _Latitude = latitude;
            _Longtitude = longtitude;
        }
    }

    public class CityData
    {

        static public City GetCityInformation(String cityToken)
        {
            SubContinent[] subContinents = CityData.GetCityData();
            foreach (SubContinent subContinent in subContinents)
            {
                StateOrCity[] stateOrCityList = subContinent._stateOrCityList;
                for (int i = 0; i < stateOrCityList.Length; i++)
                {
                    StateOrCity stateOrCity = stateOrCityList[i];
                    bool isState = stateOrCity is State;

                    if (isState)
                    {
                        State state = stateOrCity as State;
                        foreach (City city in state._cities)
                        {
                            if (city._UrlToken == cityToken)
                            {
                                return city;
                            }
                        }
                    }
                    else
                    {
                        City city = stateOrCity as City;
                        if (city._UrlToken == cityToken)
                        {
                            return city;
                        }
                    }
                }
            }
            return null;
        } 

        public static String FindClosestCity( double myLat, double myLong)
        {
            LatLong[] latLongData = {
                 new LatLong("CapeTown-SouthAfrica", -33.919090270996094, 18.421989440917969),
                 new LatLong("Cairo-Egypt", 30.049980163574219, 31.248600006103516),
                 new LatLong("Dar-es-salam,Tanzania", -6.7831997871398926, 39.267669677734375),
                 new LatLong("Durban-SouthAfrica", -29.855579376220703, 31.034420013427734),
                 new LatLong("Johannesburg-SouthAfrica", -26.204919815063477, 28.040019989013672),
                 new LatLong("Lusaka-Zambia", -15.407329559326172, 28.279979705810547),
                 new LatLong("Nairobi-Kenya", -1.2840800285339356, 36.822238922119141),
                 new LatLong("PortLouis-Mauritius", -20.161899566650391, 57.4989013671875),
                 new LatLong("Pretoria-SouthAfrica", -25.745870590209961, 28.187629699707031),
                 new LatLong("Victoria-Mahe-Seychelles", -4.5985198020935059, 55.453330993652344),
                 new LatLong("Birmingham-AL", 33.520290374755859, -86.8115005493164),
                 new LatLong("Troy-Alabama", 31.809669494628906, -85.97216796875),
                 new LatLong("Anchorage-AK", 61.217548370361328, -149.85838317871094),
                 new LatLong("Chandler-AZ", 33.303379058837891, -111.84082794189453),
                 new LatLong("Phoenix-AZ", 33.448261260986328, -112.07576751708984),
                 new LatLong("Scottsdale-AZ", 33.493999481201172, -111.92069244384766),
                 new LatLong("Tuscon-AZ", 32.221549987792969, -110.96975708007813),
                 new LatLong("Fayetville-AR", 36.063201904296875, -94.157913208007812),
                 new LatLong("Jonesboro-AR-USA", 35.835250854492188, -90.705070495605469),
                 new LatLong("LittleRock-AR", 34.748649597167969, -92.274490356445312),
                 new LatLong("Concord-CA", 37.980808258056641, -122.02519226074219),
                 new LatLong("Covina-California", 34.087421417236328, -117.88902282714844),
                 new LatLong("Cupertino-CA", 37.318840026855469, -122.02924346923828),
                 new LatLong("DiamondBar-CA", 33.999248504638672, -117.83145904541016),
                 new LatLong("Fremont-CA-USA", 37.550910949707031, -121.98217010498047),
                 new LatLong("Hayward-California", 37.671188354492188, -122.08612823486328),
                 new LatLong("Irvine-California", 33.6868782043457, -117.82534027099609),
                 new LatLong("Livermore-CA-USA", 37.675239562988281, -121.75782775878906),
                 new LatLong("LosAngeles-CA", 34.053489685058594, -118.24532318115234),
                 new LatLong("Malibu-CA-USA", 34.036170959472656, -118.68956756591797),
                 new LatLong("Milpitas-CA", 37.43280029296875, -121.89726257324219),
                 new LatLong("Montclair-CA", 34.074008941650391, -117.69125366210938),
                 new LatLong("MountShasta-CA", 41.313068389892578, -122.31256103515625),
                 new LatLong("MountainView-CA", 37.389671325683594, -122.08159637451172),
                 new LatLong("Napa-CA-USA", 38.298858642578125, -122.28521728515625),
                 new LatLong("Riverside-California", 33.98162841796875, -117.37387847900391),
                 new LatLong("Roseville(PlacerCounty)-CA", 38.748550415039062, -121.28457641601563),
                 new LatLong("Sacramento-CA(US)", 38.579059600830078, -121.49101257324219),
                 new LatLong("SanDiego-CA", 32.715690612792969, -117.16172027587891),
                 new LatLong("SanFrancisco-CA", 37.777118682861328, -122.41963958740234),
                 new LatLong("SanJose-CA", 37.338581085205078, -121.88556671142578),
                 new LatLong("SanMateo-CA", 37.547031402587891, -122.31482696533203),
                 new LatLong("SanRamon-CA", 37.778961181640625, -121.96868133544922),
                 new LatLong("SantaClara-CA", 37.355499267578125, -121.95426177978516),
                 new LatLong("Stockton-CA", 37.953670501708984, -121.29074859619141),
                 new LatLong("Sunnyvale-CA", 37.371608734130859, -122.03825378417969),
                 new LatLong("Waterford-CA", 37.641090393066406, -120.76204681396484),
                 new LatLong("Denver-CO", 39.740009307861328, -104.99230194091797),
                 new LatLong("Hartford-CT", 41.763309478759766, -72.674079895019531),
                 new LatLong("Stamford-CT", 41.05181884765625, -73.542228698730469),
                 new LatLong("Wilmington-DE", 39.740230560302734, -75.55084228515625),
                 new LatLong("FortLauderdale-FL", 26.12367057800293, -80.143562316894531),
                 new LatLong("Jacksonville-FL", 30.331470489501953, -81.656219482421875),
                 new LatLong("Miami-FL", 25.774810791015625, -80.1977310180664),
                 new LatLong("Ocala-FL", 29.187519073486328, -82.140388488769531),
                 new LatLong("Orlando-FL-USA", 28.538230895996094, -81.377388000488281),
                 new LatLong("Stuart-FL", 27.199989318847656, -80.255363464355469),
                 new LatLong("Tampa-FL", 27.946529388427734, -82.459266662597656),
                 new LatLong("Titusville-FL", 28.608030319213867, -80.8079833984375),
                 new LatLong("Atlanta-Georgia", 33.748310089111328, -84.39111328125),
                 new LatLong("Honolulu-Hawai", 21.304849624633789, -157.85775756835938),
                 new LatLong("Boise-Idaho", 43.606979370117188, -116.19341278076172),
                 new LatLong("Chicago-IL", 41.884151458740234, -87.632408142089844),
                 new LatLong("Peoria-IL", 40.692138671875, -89.587760925292969),
                 new LatLong("Springfield-IL", 39.801048278808594, -89.643600463867188),
                 new LatLong("Bedford-IN", 38.861019134521484, -86.4894027709961),
                 new LatLong("Evansville-Indiana-USA", 37.976909637451172, -87.564117431640625),
                 new LatLong("FortWayne-IN", 41.08026123046875, -85.138313293457031),
                 new LatLong("Indianapolis-IN", 39.766910552978516, -86.14996337890625),
                 new LatLong("Mishawaka-SouthBend-Notredame-IN", 41.700279235839844, -86.238609313964844),
                 new LatLong("Richmond-Indiana", 39.830108642578125, -84.890327453613281),
                 new LatLong("Coralville-Iowa", 41.676948547363281, -91.586479187011719),
                 new LatLong("DesMoines-Iowa", 41.589759826660156, -93.615646362304688),
                 new LatLong("Madrid-Iowa-USA", 41.876838684082031, -93.816780090332031),
                 new LatLong("KansasCity-KS", 39.113521575927734, -94.626823425292969),
                 new LatLong("Wichita-Kansas-USA", 37.686981201171875, -97.335578918457031),
                 new LatLong("Lexington-KY", 38.048591613769531, -84.500320434570312),
                 new LatLong("Louisville-KY", 38.254859924316406, -85.766403198242188),
                 new LatLong("BatonRouge-Louisiana-USA", 30.443340301513672, -91.1869888305664),
                 new LatLong("NewOrleans-Louisiana-USA", 29.953699111938477, -90.077751159667969),
                 new LatLong("Augusta-ME", 44.318031311035156, -69.776206970214844),
                 new LatLong("Baltimore-MD-USA", 39.290580749511719, -76.609260559082031),
                 new LatLong("Lanham-MD", 38.961910247802734, -76.862342834472656),
                 new LatLong("Ashland-MA", 42.261299133300781, -71.465789794921875),
                 new LatLong("Boston-MA", 42.3586311340332, -71.05670166015625),
                 new LatLong("Marlborough-MA(US)", 42.3468017578125, -71.547988891601562),
                 new LatLong("Springfield-MA", 42.101249694824219, -72.589286804199219),
                 new LatLong("Detroit-MI", 42.331680297851562, -83.047920227050781),
                 new LatLong("Kalamazoo-MI", 42.292430877685547, -85.601112365722656),
                 new LatLong("Lansing-Michigan", 42.731941223144531, -84.552253723144531),
                 new LatLong("Novi-MI", 42.466259002685547, -83.486282348632812),
                 new LatLong("Minneapolis-MN", 44.979030609130859, -93.264930725097656),
                 new LatLong("Minnesota-MN-USA", 44.089668273925781, -91.752250671386719),
                 new LatLong("StPaul-MN", 44.943820953369141, -93.09332275390625),
                 new LatLong("Brandon-MS", 32.273120880126953, -89.988639831542969),
                 new LatLong("St.Louis-Missouri", 38.627738952636719, -90.199508666992188),
                 new LatLong("Helena-MT", 46.589759826660156, -112.02120208740234),
                 new LatLong("Omaha-NE", 41.260688781738281, -95.940589904785156),
                 new LatLong("LasVegas-Nevada", 36.171909332275391, -115.13996887207031),
                 new LatLong("Reno-NV", 39.527381896972656, -119.81343841552734),
                 new LatLong("Nashua-NH", 42.75836181640625, -71.4642105102539),
                 new LatLong("Chatham-NJ", 40.737930297851562, -74.384452819824219),
                 new LatLong("Edison-NJ", 40.504501342773438, -74.347000122070312),
                 new LatLong("NewJersey-NJ", 40.082771301269531, -74.649917602539062),
                 new LatLong("Albuquerque-NewMexico", 35.084178924560547, -106.64864349365234),
                 new LatLong("Gallup-NewMexico", 35.527839660644531, -108.74356079101563),
                 new LatLong("Albany-NY", 42.65142822265625, -73.7552719116211),
                 new LatLong("Buffalo-NY(US)", 42.885440826416016, -78.878463745117188),
                 new LatLong("NewYork-NY", 40.714550018310547, -74.007118225097656),
                 new LatLong("Rochester-NY", 43.155490875244141, -77.616058349609375),
                 new LatLong("Asheville-NorthCarolina-USA", 35.598461151123047, -82.553138732910156),
                 new LatLong("Cary-NC", 35.789329528808594, -78.781173706054688),
                 new LatLong("Charlotte-NC-USA", 35.222858428955078, -80.837959289550781),
                 new LatLong("Greensboro-NC", 36.068988800048828, -79.7947006225586),
                 new LatLong("Greenville-NorthCarolina-USA", 35.607200622558594, -77.380233764648438),
                 new LatLong("Raleigh-NC", 35.7855110168457, -78.642669677734375),
                 new LatLong("Winston-Salem-NC", 39.576450347900391, -75.357940673828125),
                 new LatLong("Fargo-ND", 46.875911712646484, -96.7817611694336),
                 new LatLong("Cincinnati-OH", 39.107128143310547, -84.5041275024414),
                 new LatLong("Cleveland-OH", 41.5047492980957, -81.690719604492188),
                 new LatLong("Columbus-OH", 39.961990356445312, -83.00274658203125),
                 new LatLong("Dayton-OH", 39.759109497070312, -84.194442749023438),
                 new LatLong("Mason-OH-USA", 39.356971740722656, -84.297622680664062),
                 new LatLong("Toledo-OH", 41.65380859375, -83.536262512207031),
                 new LatLong("Stillwater-OK(US)", 36.116420745849609, -97.0586166381836),
                 new LatLong("Tulsa-Oklahoma", 36.149738311767578, -95.993331909179688),
                 new LatLong("Corvalis-OR", 44.565040588378906, -123.26352691650391),
                 new LatLong("Portland-OR-USA", 45.511791229248047, -122.67562866210938),
                 new LatLong("Lancaster-PA", 40.038040161132812, -76.30126953125),
                 new LatLong("NewCumberland-PA-USA", 40.229221343994141, -76.868690490722656),
                 new LatLong("Philadelphia-PA", 39.952278137207031, -75.1624526977539),
                 new LatLong("Pittsburgh-PA", 40.438331604003906, -79.9974594116211),
                 new LatLong("SanJuan-PuertoRico", 39.450000762939453, -98.907997131347656),
                 new LatLong("Providence-RI", 41.823871612548828, -71.4119873046875),
                 new LatLong("Greenville-SC", 34.848270416259766, -82.400108337402344),
                 new LatLong("Summerville-SouthCarolina", 33.018970489501953, -80.176010131835938),
                 new LatLong("Sumter-SC", 33.924339294433594, -80.342361450195312),
                 new LatLong("Pierre-SD", 44.368919372558594, -100.35015106201172),
                 new LatLong("Chattanooga-TN(US)", 35.046440124511719, -85.309463500976562),
                 new LatLong("Knoxville-TN", 35.960491180419922, -83.920913696289062),
                 new LatLong("Memphis-TN", 35.149761199951172, -90.049247741699219),
                 new LatLong("Nashville-TN", 36.167839050292969, -86.778160095214844),
                 new LatLong("Austin-TX", 30.267599105834961, -97.74298095703125),
                 new LatLong("Beaumont-TX(US)", 30.086149215698242, -94.101577758789062),
                 new LatLong("Dallas-Texas", 32.778148651123047, -96.795402526855469),
                 new LatLong("ElPaso-TX", 31.759159088134766, -106.48748779296875),
                 new LatLong("Houston-Texas", 29.76045036315918, -95.369781494140625),
                 new LatLong("Irving-TX", 32.813510894775391, -96.955497741699219),
                 new LatLong("Irvington-TX", 31.436729431152344, -99.306922912597656),
                 new LatLong("Lubbock-TX", 33.584510803222656, -101.84500885009766),
                 new LatLong("Midland-Texas", 32.000438690185547, -102.07568359375),
                 new LatLong("Navasota-TX", 30.388149261474609, -96.087799072265625),
                 new LatLong("Pearland-TX", 29.550930023193359, -95.256866455078125),
                 new LatLong("Plano-TX", 33.020790100097656, -96.699249267578125),
                 new LatLong("SanAntonio-TX", 29.424579620361328, -98.494613647460938),
                 new LatLong("Temple-TX", 31.097469329833984, -97.343032836914062),
                 new LatLong("SouthJordan-UT-USA", 40.548019409179688, -111.93869018554688),
                 new LatLong("Montpelier-VT", 44.260288238525391, -72.576263427734375),
                 new LatLong("Arlington-Virginia-USA", 38.890510559082031, -77.086288452148438),
                 new LatLong("Ashburn-VA", 39.051628112792969, -77.483146667480469),
                 new LatLong("Richmond-VA-USA", 37.540699005126953, -77.433647155761719),
                 new LatLong("Vienna-VA", 38.900371551513672, -77.2636489868164),
                 new LatLong("Bellingham-WA-USA", 48.752349853515625, -122.47122192382813),
                 new LatLong("Seattle-WA-USA", 47.603561401367188, -122.32943725585938),
                 new LatLong("Spokane-WA", 47.657260894775391, -117.41227722167969),
                 new LatLong("Yakima-WA", 46.604129791259766, -120.50704956054688),
                 new LatLong("Washington-DC", 38.8903694152832, -77.0319595336914),
                 new LatLong("Charleston-WV", 38.350139617919922, -81.638931274414062),
                 new LatLong("Madison-WI", 43.072948455810547, -89.386688232421875),
                 new LatLong("Milwaukee-WI", 43.04180908203125, -87.9068374633789),
                 new LatLong("Pewaukee-Wisconsin", 43.082210540771484, -88.259162902832031),
                 new LatLong("Laramie-Wyoming", 41.310798645019531, -105.59031677246094),
                 new LatLong("Bridgetown-Barbados", 13.112090110778809, -59.612678527832031),
                 new LatLong("Georgetown-Guyana", 6.8085498809814453, -58.161251068115234),
                 new LatLong("Kingston-Jamaica", 17.970970153808594, -76.788238525390625),
                 new LatLong("Nassau-Bahamas", 25.07819938659668, -77.3438491821289),
                 new LatLong("PortofSpain-TrinidadandTobago-WestIndies", 10.661029815673828, -61.516078948974609),
                 new LatLong("Bogota-Columbia-SouthAmerica", 4.6149501800537109, -74.069381713867188),
                 new LatLong("BuenosAires-Argentina", -34.6085205078125, -58.373538970947266),
                 new LatLong("Paramaribo-Surinam", 5.820310115814209, -55.165420532226562),
                 new LatLong("Amsterdam-Netherlands", 52.373088836669922, 4.8933000564575195),
                 new LatLong("Berlin-Germany", 52.516071319580078, 13.376979827880859),
                 new LatLong("Bonn-Germany", 50.732421875, 7.1018600463867188),
                 new LatLong("Bremen-Germany", 53.075099945068359, 8.80469036102295),
                 new LatLong("Copenhagen-Denmark", 55.675678253173828, 12.567600250244141),
                 new LatLong("Czarnow-Poland", 52.529590606689453, 14.757390022277832),
                 new LatLong("Dublin-Ireland", 53.348068237304688, -6.2482700347900391),
                 new LatLong("Fort-de-France-Martinique", 14.642999649047852, -60.978000640869141),
                 new LatLong("FrankfurtamMain-Germany", 50.112079620361328, 8.6834096908569336),
                 new LatLong("Geneva-Switzerland", 46.208351135253906, 6.1427497863769531),
                 new LatLong("Hamburg-Germany", 53.553340911865234, 9.9924697875976562),
                 new LatLong("Hannover-Germany", 52.372268676757812, 9.7381496429443359),
                 new LatLong("Helsinki-Finland", 60.171161651611328, 24.932649612426758),
                 new LatLong("Homburg-Saarland-Germany", 51.201999664306641, 10.381999969482422),
                 new LatLong("Ipswich-UK", 52.057910919189453, 1.1454399824142456),
                 new LatLong("Leicester-England-UK", 52.637981414794922, -1.1404399871826172),
                 new LatLong("Kiev-Ukraine", 50.4547004699707, 30.523799896240234),
                 new LatLong("London-UnitedKingdom", 51.506320953369141, -0.12714000046253204),
                 new LatLong("Maastricht-Netherlands", 50.849830627441406, 5.688270092010498),
                 new LatLong("Madrid-Spain", 40.4202995300293, -3.7057700157165527),
                 new LatLong("Malaga-Spain", 36.718318939208984, -4.4201598167419434),
                 new LatLong("Manchester-UK", 53.479591369628906, -2.2487399578094482),
                 new LatLong("Milano-Italy", 45.468940734863281, 9.1810302734375),
                 new LatLong("Munich-Germany", 48.136409759521484, 11.577529907226563),
                 new LatLong("Nuremberg-Germany", 49.454341888427734, 11.073490142822266),
                 new LatLong("Odense-Denmark", 55.396171569824219, 10.390789985656738),
                 new LatLong("Oslo-Norway", 59.912281036376953, 10.749979972839356),
                 new LatLong("Regensburg-Germany", 49.014919281005859, 12.101730346679688),
                 new LatLong("Rome-Italy", 41.903049468994141, 12.495800018310547),
                 new LatLong("Rotterdam-Netherlands", 51.9228515625, 4.47845983505249),
                 new LatLong("Stockholm-Sweden", 59.332328796386719, 18.062929153442383),
                 new LatLong("Warsaw-Poland", 52.2287483215332, 21.006250381469727),
                 new LatLong("Zurich-Switzerland", 47.377071380615234, 8.5395603179931641),
                 new LatLong("AbuDhabi-UAE", 23.684999465942383, 54.203998565673828),
                 new LatLong("Doha-Qatar", 25.294570922851562, 51.519439697265625),
                 new LatLong("Dubai-UAE", 25.269439697265625, 55.308650970458984),
                 new LatLong("KuwaitCity-Kuwait", 29.374300003051758, 47.974849700927734),
                 new LatLong("Manama-Bahrain", 26.22953987121582, 50.576000213623047),
                 new LatLong("Muscat-Oman", 23.615190505981445, 58.591190338134766),
                 new LatLong("Riyadh-SaudiArabia", 24.647449493408203, 46.714530944824219),
                 new LatLong("SanAYemen", 15.340999603271484, 47.908000946044922),
                 new LatLong("Sharjah-UAE", 25.352329254150391, 55.3922004699707),
                 new LatLong("Tehran-Iran", 35.689540863037109, 51.4146842956543),
                 new LatLong("Agra-UP", 27.191930770874023, 77.997528076171875),
                 new LatLong("Ahemadabad-Gujarat", 23.027992248535156, 72.594123840332031),
                 new LatLong("Ajmer", 26.4655704498291, 74.631683349609375),
                 new LatLong("Aligarh-UP", 27.830549240112305, 78.210662841796875),
                 new LatLong("Allahabad-UP", 25.446210861206055, 81.807701110839844),
                 new LatLong("Ahemadabad-Gujarat", 23.027992248535156, 72.594123840332031),
                 new LatLong("Amaravati-Maharastra", 16.579383850097656, 80.362785339355469),
                 new LatLong("Amritsar", 31.635040283203125, 74.88690185546875),
                 new LatLong("Anand-Gujarat-India", 22.549819946289062, 73.035186767578125),
                 new LatLong("Aurangabad-Maharastra", 19.872739791870117, 75.347442626953125),
                 new LatLong("Badrinath-Uttarakhand", 30.73381233215332, 79.489059448242188),
                 new LatLong("Varanasi-India", 25.329160690307617, 82.990478515625),
                 new LatLong("Bangalore-India", 12.966970443725586, 77.5872802734375),
                 new LatLong("Baroda-India", 22.31117057800293, 73.195709228515625),
                 new LatLong("Belgaum-India", 15.801759719848633, 74.56951904296875),
                 new LatLong("Bharuch-India", 21.727550506591797, 73.014053344726562),
                 new LatLong("Bhatinda-India", 30.202714920043945, 74.940338134765625),
                 new LatLong("Bhavnagar-Gujarat-India", 21.770069122314453, 72.1458511352539),
                 new LatLong("Bhopal-India", 23.252189636230469, 77.400077819824219),
                 new LatLong("Bhubaneswar-India", 20.2674503326416, 85.829963684082031),
                 new LatLong("Bhuj-India", 23.381780624389648, 69.706260681152344),
                 new LatLong("Bikaner", 28.007030487060547, 73.3189468383789),
                 new LatLong("Bilaspur-Chhattisgarh-India", 22.0693302154541, 82.158561706542969),
                 new LatLong("Mumbai-Maharastra-India", 18.975419998168945, 72.8297119140625),
                 new LatLong("Chandigarh-India", 30.744840621948242, 76.771469116210938),
                 new LatLong("Chennai-India", 13.091059684753418, 80.291191101074219),
                 new LatLong("Coimbatore-India", 11.016420364379883, 76.989753723144531),
                 new LatLong("Kolkata-India", 22.568899154663086, 88.349403381347656),
                 new LatLong("Dakor-Kheda-Gujarat", 22.753580093383789, 73.153167724609375),
                 new LatLong("Darjiling-WestBengal", 27.034650802612305, 88.265960693359375),
                 new LatLong("DehraDun", 30.322900772094727, 78.031700134277344),
                 new LatLong("NewDelhi-India", 28.644720077514648, 77.216392517089844),
                 new LatLong("Deoria-UP-India", 26.739799499511719, 83.763481140136719),
                 new LatLong("Dharmapuri", 12.119379997253418, 78.150650024414062),
                 new LatLong("Dharwar-India", 15.457530021667481, 74.99774169921875),
                 new LatLong("Dwarka-India", 22.249309539794922, 68.963363647460938),
                 new LatLong("Ernakulam-India", 9.9693899154663086, 76.288948059082031),
                 new LatLong("Gangotri-Uttarakhand", 30.994699478149414, 78.940078735351562),
                 new LatLong("Gauhati-India", 26.132070541381836, 91.790191650390625),
                 new LatLong("Gaya(HolyGayaJi)-Bihar", 24.779289245605469, 84.980072021484375),
                 new LatLong("Ghaziabad-UP", 28.782249450683594, 77.278671264648438),
                 new LatLong("Godhra-Gujarat", 22.779809951782227, 73.590911865234375),
                 new LatLong("Guntur-AndraPradesh-India", 16.282379150390625, 80.401161193847656),
                 new LatLong("Gurgaon-Haryana", 28.45404052734375, 77.026657104492188),
                 new LatLong("Haridwar-Uttarakhand", 29.912990570068359, 78.1290283203125),
                 new LatLong("Himatnagar-Gujarat", 23.581279754638672, 72.96063232421875),
                 new LatLong("Hubli-India", 15.355830192565918, 75.133186340332031),
                 new LatLong("Hyderabad-AP-India", 17.347139358520508, 78.428131103515625),
                 new LatLong("Idar-Gujarat", 23.836320877075195, 73.002113342285156),
                 new LatLong("Imphal-Manipur", 24.813030242919922, 93.933021545410156),
                 new LatLong("Indore-India", 22.716220855712891, 75.865119934082031),
                 new LatLong("Jaipur", 26.896699905395508, 75.874237060546875),
                 new LatLong("Jaisalmer", 26.918600082397461, 70.942138671875),
                 new LatLong("Jullundhur", 31.5516300201416, 75.640937805175781),
                 new LatLong("Jammu-Kashmir-India", 32.736671447753906, 74.819259643554688),
                 new LatLong("Jamnagar-India", 22.470720291137695, 70.0457992553711),
                 new LatLong("Jamshedpur", 22.813800811767578, 86.207878112792969),
                 new LatLong("Jhansi-UP", 25.44420051574707, 78.577850341796875),
                 new LatLong("Jodhpur", 26.26692008972168, 73.030532836914062),
                 new LatLong("Jullundhur", 31.5516300201416, 75.640937805175781),
                 new LatLong("Kakinada-AP-India", 16.958139419555664, 82.24664306640625),
                 new LatLong("Kalyan-India", 19.232370376586914, 73.142181396484375),
                 new LatLong("Kallakkurichchi-India", 11.739069938659668, 78.962188720703125),
                 new LatLong("Kanchipuram-India", 12.844229698181152, 79.7052001953125),
                 new LatLong("Kanpur-UP", 26.431230545043945, 80.331947326660156),
                 new LatLong("Kedarnath-Uttarakhand", 30.734140396118164, 79.066680908203125),
                 new LatLong("Khedabrahma-Sabarkantha-Gujarat", 24.03196907043457, 73.048637390136719),
                 new LatLong("Kolhapur-Maharastra", 16.716770172119141, 74.230712890625),
                 new LatLong("Kolkata-India", 22.568899154663086, 88.349403381347656),
                 new LatLong("Kota-Rajasthan", 25.171079635620117, 75.852676391601562),
                 new LatLong("Kumbakonam-India", 10.960320472717285, 79.389411926269531),
                 new LatLong("Kurukshetra-India", 29.973100662231445, 76.827507019042969),
                 new LatLong("Lucknow-India", 26.854709625244141, 80.921348571777344),
                 new LatLong("Ludhiana-India", 30.901960372924805, 75.855400085449219),
                 new LatLong("Machilipatnam-AP-India", 16.142669677734375, 81.028282165527344),
                 new LatLong("Madurai-India", 9.9367799758911133, 78.119873046875),
                 new LatLong("Manali-HimachalPradesh-India", 32.266700744628906, 77.166702270507812),
                 new LatLong("Mandi-HimachalPradesh-India", 31.716699600219727, 76.916702270507812),
                 new LatLong("Mangalore-Karnataka-India", 12.890999794006348, 74.853469848632812),
                 new LatLong("Mathura-UP", 27.493209838867188, 77.67230224609375),
                 new LatLong("Meerut-UP", 29.009300231933594, 77.7488784790039),
                 new LatLong("Mehsana-India", 23.531469345092773, 72.385223388671875),
                 new LatLong("Mumbai-Maharastra-India", 18.975419998168945, 72.8297119140625),
                 new LatLong("Mysore-Karnataka-India", 12.310799598693848, 76.64544677734375),
                 new LatLong("Nagpur-Maharastra", 21.156320571899414, 79.082000732421875),
                 new LatLong("Nasik-Maharastra", 19.959009170532227, 73.831939697265625),
                 new LatLong("Nainital-Uttarakhand", 29.392860412597656, 79.45220947265625),
                 new LatLong("Nathdwara(LordSrinathjiTemple)-India", 24.939640045166016, 73.820182800292969),
                 new LatLong("Nellore-India", 14.414679527282715, 79.9844970703125),
                 new LatLong("NewDelhi-India", 22.461959838867188, 88.302978515625),
                 new LatLong("Noida-UP", 28.5970401763916, 77.350357055664062),
                 new LatLong("Palakkad(Palghat)-Kerala", 10.801779747009277, 76.6301498413086),
                 new LatLong("Panaji(Panjim)-Goa", 15.458760261535645, 73.807830810546875),
                 new LatLong("Pandharpur-Maharastra", 17.677400588989258, 75.3232421875),
                 new LatLong("Patan-Gujarat", 23.852470397949219, 72.137680053710938),
                 new LatLong("Patiala-Punjab-India", 30.344850540161133, 76.4000015258789),
                 new LatLong("Patna-Bihar-India", 25.600429534912109, 85.18682861328125),
                 new LatLong("Pondicherry-India", 11.923580169677734, 79.818778991699219),
                 new LatLong("Proddatur-AndhraPradesh-India", 14.744259834289551, 78.561996459960938),
                 new LatLong("Pune-Maharastra-India", 18.504190444946289, 73.85308837890625),
                 new LatLong("Puri(CityofLordJaggnath)-India", 19.83173942565918, 85.87689208984375),
                 new LatLong("PutturnearMangalore", 12.761899948120117, 75.202720642089844),
                 new LatLong("Raipur-Chhattisgarh", 21.247280120849609, 81.641273498535156),
                 new LatLong("Rajahmundry-India", 16.999530792236328, 81.804801940917969),
                 new LatLong("Rajkot-India", 22.296770095825195, 70.801567077636719),
                 new LatLong("Ranchi-Jharkhand", 23.360740661621094, 85.323402404785156),
                 new LatLong("Rishikesh-Uttarakhand", 30.091440200805664, 78.2750473022461),
                 new LatLong("Rohtak-Hariyana", 28.896579742431641, 76.580818176269531),
                 new LatLong("Roorkee-India", 29.857450485229492, 77.888313293457031),
                 new LatLong("Saharanpur-UP", 29.9483699798584, 77.546150207519531),
                 new LatLong("Sangli-Maharastra", 16.856563568115234, 74.568534851074219),
                 new LatLong("Satara-Maharastra", 17.685039520263672, 74.019500732421875),
                 new LatLong("Secunderabad-AndraPradesh-India", 17.505340576171875, 78.5064468383789),
                 new LatLong("Shimla-HimachalPradesh", 31.11359977722168, 77.105148315429688),
                 new LatLong("Shiradi(SaibabaTemple)-Maharastra", 12.83329963684082, 75.5167007446289),
                 new LatLong("Srirangam(Thiruvarangam)-India", 9.4238796234130859, 78.618217468261719),
                 new LatLong("Surat-India", 21.19219970703125, 72.835029602050781),
                 new LatLong("Thiruvananthapuram-Kerala", 8.5986604690551758, 76.896720886230469),
                 new LatLong("Srirangam(Thiruvarangam)-India", 10.856960296630859, 78.693130493164062),
                 new LatLong("Tirumala-AP-India", 13.678489685058594, 79.352210998535156),
                 new LatLong("Udaipur", 24.579780578613281, 73.697090148925781),
                 new LatLong("Udipi-Karnataka-India", 13.3256196975708, 74.739303588867188),
                 new LatLong("Ujjain-India", 23.179319381713867, 75.786300659179688),
                 new LatLong("Uttarkashi-Uttarakhand", 30.730810165405273, 78.443763732910156),
                 new LatLong("Baroda-India", 22.31117057800293, 73.195709228515625),
                 new LatLong("Varanasi-India", 25.329160690307617, 82.990478515625),
                 new LatLong("Vijayawada-India", 16.512479782104492, 80.7494888305664),
                 new LatLong("Vishakhapatnam-India", 17.723112106323242, 83.306396484375),
                 new LatLong("Zirapur-Rajgarh-MP", 24.016700744628906, 76.36669921875),
                 new LatLong("Karachi-Pakistan", 24.92671012878418, 67.034370422363281),
                 new LatLong("Islamabad-Pakistan", 33.707679748535156, 73.070472717285156),
                 new LatLong("Lahore-Pakistan", 31.539449691772461, 74.303482055664062),
                 new LatLong("Colombo-Srilanka", 6.9319601058959961, 79.845573425292969),
                 new LatLong("Jaffna-Srilanka", 9.6592597961425781, 80.018707275390625),
                 new LatLong("Kandy-SriLanka", 7.2950301170349121, 80.638618469238281),
                 new LatLong("Shanghai-China", 31.255159378051758, 121.47470092773438),
                 new LatLong("Tokyo-Japan", 35.683208465576172, 139.80894470214844),
                 new LatLong("Ulaanbaatar-Mongolia", 47.922100067138672, 106.91696929931641),
                 new LatLong("Yiwu(Zhejiang)-China", 30.50115966796875, 111.31362915039063),
                 new LatLong("Antipolo-Philippines", 14.62162971496582, 121.16795349121094),
                 new LatLong("Bali-Indonesia", -6.1166658401489258, 106.98332977294922),
                 new LatLong("Dhaka-Bangladesh", 23.70918083190918, 90.405059814453125),
                 new LatLong("Hanoi-Vietnam", 21.025949478149414, 105.84712219238281),
                 new LatLong("Jakarta-Indonesia", -6.17149019241333, 106.82752227783203),
                 new LatLong("KualaLumpur-Malaysia", 3.1047699451446533, 101.69200134277344),
                 new LatLong("Singapore", 1.2901599407196045, 103.85199737548828),
                 new LatLong("Adelaide-Australia", -34.926101684570312, 138.59988403320313),
                 new LatLong("Brisbane-Australia", -27.468460083007812, 153.02342224121094),
                 new LatLong("Canberra-Australia", -35.306541442871094, 149.12655639648438),
                 new LatLong("Darwin-Australia", -12.4610595703125, 130.84165954589844),
                 new LatLong("Melbourne-Australia", -37.817531585693359, 144.96714782714844),
                 new LatLong("Perth-Australia", -31.952640533447266, 115.85740661621094),
                 new LatLong("Sydney-Australia", -33.86962890625, 151.20695495605469),
                 new LatLong("Auckland-NZ", -36.884109497070312, 174.77041625976563),
                 new LatLong("Hamilton-NZ", -37.7831916809082, 175.2769775390625),
                 new LatLong("Nadi-Fiji", -17.739650726318359, 177.46632385253906),
                 new LatLong("Suva-Fiji", -18.118789672851562, 178.43696594238281),
            };

            double minDistance = 6731; // set this to the radius of the earth
            String nearestCity = String.Empty;
            foreach (LatLong latlong in latLongData)
            {
                double dist = ComputeDistance(myLat, myLong, latlong._Latitude, latlong._Longtitude);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestCity = latlong._UrlToken;
                }
            }
            return nearestCity;
        }

        public static double ComputeDistance(double lat1, double long1, double lat2, double long2)
        {
            double R = 6731; // mi
            double lat1Rad = lat1 * (Math.PI / 180);
            double lat2Rad = lat2 * (Math.PI / 180);
            double long1Rad = long1 * (Math.PI / 180);
            double long2Rad = long2 * (Math.PI / 180);
            double dLat = Math.Abs(lat2Rad - lat1Rad);
            double dLon = Math.Abs(long2Rad - long1Rad);

            // a is the haversine formula
            double a = (Math.Sin(dLat / 2) * Math.Sin(dLat / 2)) +
                    (Math.Sin(dLon / 2) * Math.Sin(dLon / 2)) * Math.Cos(lat1Rad) * Math.Cos(lat2Rad);
            double  c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = R * c;
            return d;
        }

        public static SubContinent[] GetCityData()
        {
            City[] AL ={
                new City ("Birmingham, AL", "Birmingham-AL"),
                new City ("Troy, Alabama", "Troy-Alabama")
            };

            City[] AK ={
                new City ("Anchorage, AK",  "Anchorage-AK" )
            };

            City[] AZ = {
                new City ("Chandler, AZ", "Chandler-AZ"),
                new City ("Phoenix, AZ", "Phoenix-AZ"),
                new City ("Scottsdale, AZ", "Scottsdale-AZ"),
                new City ("Tuscon, AZ", "Tuscon-AZ")
            };

            City[] AR = {
                new City ("Fayetville, AR", "Fayetville-AR"),
                new City ("Jonesboro, AR", "Jonesboro-AR-USA"),
                new City ("LittleRock, AR", "LittleRock-AR")
            };

            City[] CA = {
                new City ("Concord, CA", "Concord-CA"),
                new City ("Covina, California", "Covina-California"),
                new City ("Cupertino, CA", "Cupertino-CA"),
                new City ("DiamondBar, CA", "DiamondBar-CA"),
                new City ("Fremont, CA", "Fremont-CA-USA"),
                new City ("Hayward, CA", "Hayward-California"),
                new City ("Irvine, California", "Irvine-California"),
                new City ("Livermore, CA", "Livermore-CA-USA"),
                new City ("Los Angeles, CA", "LosAngeles-CA"),
                new City ("Malibu, CA", "Malibu-CA-USA"),
                new City ("Milpitas, CA", "Milpitas-CA"),
                new City ("Montclair, CA", "Montclair-CA"),
                new City ("Mount Shasta, CA", "MountShasta-CA"),
                new City ("Mountain View, CA", "MountainView-CA"),
                new City ("Napa, CA", "Napa-CA-USA"),
                new City ("Riverside, California", "Riverside-California"),
                new City ("Roseville(PlacerCounty), CA", "Roseville(PlacerCounty)-CA"),
                new City ("Sacramento, CA", "Sacramento-CA(US)"),
                new City ("San Diego, CA", "SanDiego-CA"),
                new City ("San Francisco, CA", "SanFrancisco-CA"),
                new City ("San Jose, CA", "SanJose-CA"),
                new City ("San Mateo, CA", "SanMateo-CA"),
                new City ("San Ramon, CA", "SanRamon-CA"),
                new City ("SantaClara, CA", "SantaClara-CA"),
                new City ("Stockton, CA", "Stockton-CA"),
                new City ("Sunnyvale, CA", "Sunnyvale-CA"),
                new City ("Waterford, CA", "Waterford-CA")
            };

            City[] CO = {
                new City ("Denver, CO", "Denver-CO")
            };

            City[] CT = {
                new City ("Hartford, CT", "Hartford-CT"),
                new City ("Stamford, CT", "Stamford-CT")
            };

            City[] DE = {
                new City ("Wilmington, DE", "Wilmington-DE")
            };

            City[] FL = {
                new City ("Fort Lauderdale, FL", "FortLauderdale-FL"),
                new City ("Jacksonville, FL", "Jacksonville-FL"),
                new City ("Miami, FL", "Miami-FL"),
                new City ("Ocala, FL", "Ocala-FL"),
                new City ("Orlando, FL", "Orlando-FL-USA"),
                new City ("Stuart, FL", "Stuart-FL"),
                new City ("Tampa, FL", "Tampa-FL"),
                new City ("Titusville, FL", "Titusville-FL")
            };

            City[] GA = {
                new City ("Atlanta, GA", "Atlanta-Georgia")
            };

            City[] HI = {
                new City ("Honolulu, Hawai", "Honolulu-Hawai")
            };

            City[] ID = {
                new City ("Boise, Idaho", "Boise-Idaho")
            };

            City[] IL = {
                new City ("Chicago, IL", "Chicago-IL"),
                new City ("Peoria, IL", "Peoria-IL"),
                new City ("Springfield, IL", "Springfield-IL")
            };

            City[] IN = {
                new City ("Bedford, IN", "Bedford-IN"),
                new City ("Evansville, IN", "Evansville-Indiana-USA"),
                new City ("FortWayne, IN", "FortWayne-IN"),
                new City ("Indianapolis, IN", "Indianapolis-IN"),
                new City ("Mishawaka / South Bend / Notre Dame, IN", "Mishawaka-SouthBend-Notredame-IN"),
                new City ("Richmond, Indiana", "Richmond-Indiana")
            };

            City[] IA = {
                new City ("Coralville, Iowa", "Coralville-Iowa"),
                new City ("DesMoines, Iowa", "DesMoines-Iowa"),
                new City ("Madrid, Iowa", "Madrid-Iowa-USA")
            };

            City[] KS = {
                new City ("Kansas City, KS", "KansasCity-KS"),
                new City ("Wichita, Kansas", "Wichita-Kansas-USA")
            };

            City[] KY = {
                new City ("Lexington, KY", "Lexington-KY"),
                new City ("Louisville, KY", "Louisville-KY")
            };

            City[] LA = {
                new City ("Baton Rouge, Louisiana", "BatonRouge-Louisiana-USA"),
                new City ("New Orleans, Louisiana", "NewOrleans-Louisiana-USA")
            };

            City[] ME = {
                new City ("Augusta, ME", "Augusta-ME")
            };

            City[] MD = {
                new City ("Baltimore, MD", "Baltimore-MD-USA"),
                new City ("Lanham, MD", "Lanham-MD")
            };

            City[] MA = {
                new City ("Ashland, MA", "Ashland-MA"),
                new City ("Boston, MA", "Boston-MA"),
                new City ("Marlborough, MA", "Marlborough-MA(US)"),
                new City ("Springfield, MA", "Springfield-MA")
            };

            City[] MI = {
                new City ("Detroit, MI", "Detroit-MI"),
                new City ("Kalamazoo, MI", "Kalamazoo-MI"),
                new City ("Lansing, Michigan, USA", "Lansing-Michigan" ),
                new City ("Novi, MI", "Novi-MI")
            };

            City[] MN = {
                new City ("Minneapolis, MN", "Minneapolis-MN"),
                new City ("Minnesota, MN", "Minnesota-MN-USA"),
                new City ("St. Paul, MN", "StPaul-MN")
            };

            City[] MS = {
                new City ("Brandon, MS", "Brandon-MS")
            };

            City[] MO = {
                new City ("St. Louis, Missouri", "St.Louis-Missouri")
            };

            City[] MT = {
                new City ("Helena, MT", "Helena-MT")
            };

            City[] NE = {
                new City ("Omaha, NE", "Omaha-NE")
            };

            City[] NV = {
                new City ("Las Vegas, NV", "LasVegas-Nevada"),
                new City ("Reno, NV", "Reno-NV")
            };

            City[] NH = {
                new City ("Nashua, NH", "Nashua-NH")
            };

            City[] NJ = {
                new City ("Chatham, NJ", "Chatham-NJ"),
                new City ("Edison, NJ", "Edison-NJ"),
                new City ("New Jersey, NJ", "NewJersey-NJ")
            };

            City[] NM = {
                new City ("Albuquerque, NewMexico", "Albuquerque-NewMexico"),
                new City ("Gallup, New Mexico", "Gallup-NewMexico")
            };

            City[] NY = {
                new City ("Albany, NY", "Albany-NY"),
                new City ("Buffalo, NY", "Buffalo-NY(US)"),
                new City ("New York, NY", "NewYork-NY"),
                new City ("Rochester, NY", "Rochester-NY")
            };

            City[] NC = {
                new City ("Asheville, North Carolina", "Asheville-NorthCarolina-USA"),
                new City ("Cary, NC", "Cary-NC"),
                new City ("Charlotte, NC", "Charlotte-NC-USA"),
                new City ("Greensboro, NC", "Greensboro-NC"),
                new City ("Greenville, North Carolina", "Greenville-NorthCarolina-USA"),
                new City ("Raleigh, NC", "Raleigh-NC"),
                new City ("Winston, Salem, NC", "Winston-Salem-NC")
            };

            City[] ND = {
                new City ("Fargo, ND", "Fargo-ND")
            };

            City[] OH = {
            	new City ("Cincinnati, OH", "Cincinnati-OH"),
            	new City ("Cleveland, OH", "Cleveland-OH"),
            	new City ("Columbus, OH", "Columbus-OH"),
            	new City ("Dayton, OH", "Dayton-OH"),
            	new City ("Mason, OH, USA", "Mason-OH-USA"),
            	new City ("Toledo, OH", "Toledo-OH")
            };

            City[] OK = {
            	new City ("Stillwater, OK", "Stillwater-OK(US)"),
            	new City ("Tulsa, Oklahoma", "Tulsa-Oklahoma")
            };

            City[] OR = {
               new City("Corvalis, OR",  "Corvalis-OR" ),
            	new City ("Portland, OR", "Portland-OR-USA")
            };

            City[] PA = {
            	new City ("Lancaster, PA", "Lancaster-PA"),
            	new City ("New Cumberland, PA", "NewCumberland-PA-USA"),
            	new City ("Philadelphia, PA", "Philadelphia-PA"),
            	new City ("Pittsburgh, PA", "Pittsburgh-PA")
            };

            City[] RI = {
            	new City ("Providence, RI", "Providence-RI")
            };

            City[] SC = {
            	new City ("Greenville, South Carolina", "Greenville-SC"),
            	new City ("Summerville, South Carolina", "Summerville-SouthCarolina"),
            	new City ("Sumter, SC", "Sumter-SC")
            };

            City[] SD = {
            	new City ("Pierre, SD", "Pierre-SD")
            };

            City[] TN = {
            	new City ("Chattanooga, TN", "Chattanooga-TN(US)"),
            	new City ("Knoxville, TN", "Knoxville-TN"),
            	new City ("Memphis, TN", "Memphis-TN"),
            	new City ("Nashville, TN", "Nashville-TN")
            };

            City[] TX = {
            	new City ("Austin, TX", "Austin-TX"),
            	new City ("Beaumont, TX", "Beaumont-TX(US)"),
            	new City ("Dallas, TX", "Dallas-Texas"),
            	new City ("El Paso, TX", "ElPaso-TX"),
            	new City ("Houston, Texas", "Houston-Texas"),
            	new City ("Irving, TX", "Irving-TX"),
            	new City ("Irvington, TX", "Irvington-TX"),
            	new City ("Lubbock, TX", "Lubbock-TX"),
            	new City ("Midland, Texas", "Midland-Texas"),
            	new City ("Navasota, TX", "Navasota-TX"),
            	new City ("Pearland, TX", "Pearland-TX"),
            	new City ("Plano, TX", "Plano-TX"),
            	new City ("San Antonio, TX", "SanAntonio-TX"),
            	new City ("Temple, TX", "Temple-TX")
            };

            City[] UT = {
            	new City ("South Jordan, UT", "SouthJordan-UT-USA")
            };

            City[] VT = {
            	new City ("Montpelier, VT", "Montpelier-VT")
            };

            City[] VA = {
            	new City ("Arlington, Virginia, USA", "Arlington-Virginia-USA" ),
            	new City ("Ashburn, VA", "Ashburn-VA"),
            	new City ("Richmond, VA", "Richmond-VA-USA"),
            	new City ("Vienna, VA", "Vienna-VA")
            };

            City[] WA = {
            	new City ("Bellingham, WA", "Bellingham-WA-USA"),
            	new City ("Seattle, WA", "Seattle-WA-USA"),
            	new City ("Spokane, WA", "Spokane-WA"),
            	new City ("Yakima, WA", "Yakima-WA")
            };

            City[] DC = {
            	new City ("Washington, DC", "Washington-DC")
            };

            City[] WV = {
              new City ("Charleston, WV",  "Charleston-WV" )
            };

            City[] WI = {
            	new City ("Madison, WI", "Madison-WI"),
            	new City ("Milwaukee, WI", "Milwaukee-WI"),
            	new City ( "Pewaukee, Wisconsin", "Pewaukee-Wisconsin" )
            };

            City[] WY = {
            	new City ("Laramie, Wyoming", "Laramie-Wyoming")
            };

            City[] PRico = {
            	new City ("San Juan, Puerto Rico", "SanJuan-PuertoRico")
            };

            City[] Africa = {
                new City ( "CapeTown, South Africa", "CapeTown-SouthAfrica" ),
                new City ( "Cairo, Egypt", "Cairo-Egypt" ),
                new City ( "Dar es salam,Tanzania", "Dar-es-salam,Tanzania" ),
                new City ( "Durban, South Africa", "Durban-SouthAfrica" ),
                new City ( "Johannesburg, South Africa", "Johannesburg-SouthAfrica" ),
                new City ( "Kinshasa, Congo/Zaire", "Kinshasa-Congo-Zaire" ),
                new City ( "Lusaka, Zambia", "Lusaka-Zambia" ),
                new City ( "Nairobi, Kenya", "Nairobi-Kenya" ),
                new City ( "PortLouis, Mauritius", "PortLouis-Mauritius" ),
                new City ( "Pretoria, South Africa", "Pretoria-SouthAfrica" ),
                new City ( "Victoria, Mahe, Seychelles", "Victoria-Mahe-Seychelles" )
            };

            City[] SouthAmerica = {
                new City ("Bogota, Columbia", "Bogota-Columbia-SouthAmerica"),
                new City ("BuenosAires, Argentina", "BuenosAires-Argentina"),
                new City ("Paramaribo, Surinam", "Paramaribo-Surinam")
            };

            City[] Canada = {
                new City("Calgary, AB", "Calgary-AB-Canada"),
                new City( "Edmonton, Alberta",  "Edmonton-Alberta" ),
                new City( "Halifax, NovaScotia, Canada",  "Halifax-NovaScotia-Canada" ),
                new City( "Montreal, Quebec",  "Montreal-Quebec-Canada" ),
                new City( "Ottawa, ON",  "Ottawa-ON-Canada" ),
                new City( "Saskatoon, Saskatchewan, Canada",  "Saskatoon-Saskatchewan-Canada" ), 
                new City( "Scarborough, Ontario, Canada","Scarborough-OntarioCanada"),
                new City( "St.John's, Newfoundland, Canada",  "St.Johns-Newfoundland-Canada" ),
                new City( "Toronto, ON",  "Toronto-ON-Canada" ),
                new City("Vancouver, BC", "Vancouver-BC-Canada"),
                new City("Victoria, BC", "Victoria-BC-Canada"),
                new City("Winnipeg, MB", "Winnipeg-MB-Canada")
            };

            City[] Carribean = {
                new City("Bridgetown, Barbados", "Bridgetown-Barbados"),
                new City("Georgetown, Guyana", "Georgetown-Guyana"),
                new City("Kingston, Jamaica", "Kingston-Jamaica"),
                new City("Nassau, Bahamas", "Nassau-Bahamas"),
                new City("Port of Spain, Trinidad and Tobago", "PortofSpain-TrinidadandTobago-WestIndies")
            };
            City[] Australia = {
                new City("Adelaide, Australia", "Adelaide-Australia", TimeZoneValues.SAU),
                new City("Brisbane, Australia", "Brisbane-Australia", TimeZoneValues.QSLND),
                new City("Canberra, Australia", "Canberra-Australia", TimeZoneValues.NSW),
                new City("Darwin, Australia", "Darwin-Australia", TimeZoneValues.NT),
                new City("Melbourne, Australia", "Melbourne-Australia", TimeZoneValues.Victoria),
                new City("Perth, Australia", "Perth-Australia", TimeZoneValues.WAU),
                new City("Sydney, Australia", "Sydney-Australia", TimeZoneValues.NSW)};

            City[] Newzealand = {
                new City("Auckland, New Zealand", "Auckland-NZ"),
                new City("Hamilton, New Zealand", "Hamilton-NZ")
            };

            City[] Fiji = {
                new City("Nadi, Fiji", "Nadi-Fiji"),
                new City("Suva, Fiji", "Suva-Fiji")
            };

            City[] Europe = {
                new City("Aberdeen, Scotland (UK)", "Aberdeen-Scotland-UK"),
                new City("Amsterdam, Netherlands", "Amsterdam-Netherlands"),
                new City("Berlin, Germany", "Berlin-Germany"),
                new City("Birmingham, England, UK", "Birmingham-England-UK"),
                new City("Bonn, Germany", "Bonn-Germany"),
                new City("Bremen, Germany", "Bremen-Germany"),
                new City("Budapest, Hungary", "Budapest-Hungary"),
                new City("Copenhagen, Denmark", "Copenhagen-Denmark"),
                new City("Czarnow, Poland", "Czarnow-Poland"),
                new City("Dublin, Ireland", "Dublin-Ireland"),
                new City("Edinburgh, Scotland, UK", "Edinburgh-Scotland-UK"),
                new City("Fort-de-France Martinique", "Fort-de-France-Martinique"),
                new City("Frankfurt, Germany", "FrankfurtamMain-Germany"),
                new City("Geneva, Switzerland", "Geneva-Switzerland"),
                new City("Glasgow, Scotland, UK", "Glasgow-Scotland-UK"),
                new City("Hamburg, Germany", "Hamburg-Germany"),
                new City("Hannover, Germany", "Hannover-Germany"),
                new City("Helsinki, Finland", "Helsinki-Finland"),
                new City("Homburg-Saarland, Germany", "Homburg-Saarland-Germany"),
                new City("Ipswich, UK", "Ipswich-UK"),
                new City("Istanbul, Turkey", "Istanbul-Turkey"),
                new City("Leicester, UK", "Leicester-England-UK"),
                new City("Kiev, Ukraine", "Kiev-Ukraine"),
                new City("London, UK", "London-UnitedKingdom"),
                new City("Maastricht, Netherlands", "Maastricht-Netherlands"),
                new City("Madrid, Spain", "Madrid-Spain"),
                new City("Malaga, Spain", "Malaga-Spain"),
                new City("Manchester, UK", "Manchester-UK"),
                new City("Milano, Italy", "Milano-Italy"),
                new City("Munich, Germany", "Munich-Germany"),
                new City("Nuremberg, Germany", "Nuremberg-Germany"),
                new City("Odense, Denmark", "Odense-Denmark"),
                new City("Oslo, Norway", "Oslo-Norway"),
                new City("Paris, France", "Paris-France"),
                new City("Plovdiv, Bulgaria", "Plovdiv-Bulgaria"),
                new City("Regensburg, Germany", "Regensburg-Germany"),
                new City("Rome, Italy", "Rome-Italy"),
                new City("Rotterdam, Netherlands", "Rotterdam-Netherlands"),
                new City("Stockholm, Sweden", "Stockholm-Sweden"),
                new City("Warsaw, Poland", "Warsaw-Poland"),
                new City("Zurich, Switzerland", "Zurich-Switzerland")
            };

            City[] ArabianGulf ={
                new City("AbuDhabi, UAE", "AbuDhabi-UAE"),
                new City("Doha, Qatar", "Doha-Qatar"),
                new City("Dubai, UAE", "Dubai-UAE"),
                new City("Kuwait City, Kuwait", "KuwaitCity-Kuwait"),
                new City("Manama, Bahrain", "Manama-Bahrain"),
                new City("Muscat, Oman", "Muscat-Oman"),
                new City("Riyadh, Saudi Arabia", "Riyadh-SaudiArabia"),
                new City("San'A' , Yemen", "SanAYemen"),
                new City("Sharjah, UAE", "Sharjah-UAE"),
                new City("Tehran, Iran", "Tehran-Iran")
            };

            City[] India = {
                new City("Agra, UP", "Agra-UP"),
                new City("Ahemadabad (Amadavad), Gujarat", "Ahemadabad-Gujarat"),
                new City("Ajmer", "Ajmer"),
                new City("Aligarh, UP", "Aligarh-UP"),
                new City("Allahabad, UP", "Allahabad-UP"),
                new City("Amadavad, Gujarat", "Ahemadabad-Gujarat"),
                new City("Amaravati, Maharastra", "Amaravati-Maharastra"),
                new City("Amritsar", "Amritsar"),
                new City("Anand, Gujarat", "Anand-Gujarat-India"),
                new City("Aurangabad, Maharastra", "Aurangabad-Maharastra"),
                new City("Badrinath, Uttarakhand", "Badrinath-Uttarakhand"),
                new City("Banaras", "Varanasi-India"),
                new City("Bangalore / Bengaluru", "Bangalore-India"),
                new City("Baroda", "Baroda-India"),
                new City("Belgaum", "Belgaum-India"),
                new City("Bharuch", "Bharuch-India"),
                new City("Bhatinda, Punjab, India", "Bhatinda-India"),
                new City("Bhavnagar, Gujarat", "Bhavnagar-Gujarat-India"),
                new City("Bhopal, India", "Bhopal-India"),
                new City("Bhubaneswar", "Bhubaneswar-India"),
                new City("Bhuj", "Bhuj-India"),
                new City("Bikaner", "Bikaner"),
                new City("Bilaspur, Chhattisgarh", "Bilaspur-Chhattisgarh-India"),
                new City("Bombay / Mumbai, Maharastra", "Mumbai-Maharastra-India"),
                new City("Chandigarh", "Chandigarh-India"),
                new City("Chennai", "Chennai-India"),
                new City("Coimbatore", "Coimbatore-India"),
                new City("Culcutta / Kolkata", "Kolkata-India"),
                new City("Dakor, Kheda, Gujarat", "Dakor-Kheda-Gujarat"),
                new City("Darjiling, West Bengal", "Darjiling-WestBengal"),
                new City("Dehradun", "DehraDun"),
                new City("Delhi / New Delhi ", "NewDelhi-India"),
                new City("Deoria, UP", "Deoria-UP-India"),
                new City("Dharmapuri, AP", "Dharmapuri"),
                new City("Dharwad, Karnataka", "Dharwar-India"),
                new City("Dwarka", "Dwarka-India"),
                new City("Ernakulam", "Ernakulam-India"),
                new City("Gangotri, Uttarakhand", "Gangotri-Uttarakhand"),
                new City("Gauhati", "Gauhati-India"),
                new City("Gaya (Holy Gaya Ji), Bihar", "Gaya(HolyGayaJi)-Bihar"),
                new City("Ghaziabad, UP", "Ghaziabad-UP"),
                new City("Godhra, Gujarat", "Godhra-Gujarat"),
                new City("Guntur, Andra Pradesh", "Guntur-AndraPradesh-India"),
                new City("Gurgaon, Haryana", "Gurgaon-Haryana"),
                new City("Haridwar, Uttarakhand", "Haridwar-Uttarakhand"),
                new City("Himatnagar, Gujarat", "Himatnagar-Gujarat"),
                new City("Hubli", "Hubli-India"),
                new City("Hyderabad, AP", "Hyderabad-AP-India"),
                new City("Idar, Gujarat", "Idar-Gujarat"),
                new City("Imphal, Manipur",  "Imphal-Manipur" ),
                new City("Indore, MP", "Indore-India"),
                new City("Jaipur", "Jaipur"),
                new City("Jaisalmer", "Jaisalmer"),
                new City("Jalandhar (Punjab)", "Jullundhur"),
                new City("Jammu, Kashmir", "Jammu-Kashmir-India"),
                new City("Jamnagar", "Jamnagar-India"),
                new City("Jamshedpur", "Jamshedpur"),
                new City("Jhansi, UP", "Jhansi-UP"),
                new City("Jodhpur", "Jodhpur"),
                new City("Jullundhur (Punjab)", "Jullundhur"),
                new City("Kakinada, AP", "Kakinada-AP-India"),
                new City("Kalyan, India", "Kalyan-India"),
                new City("Kallakkurichchi, Viluppuram, India", "Kallakkurichchi-India" ),
                new City("Kanchipuram, India", "Kanchipuram-India" ),
                new City("Kanpur, UP", "Kanpur-UP"),
                new City("Kedarnath, Uttarakhand", "Kedarnath-Uttarakhand"),
                new City("Khedabrahma, Gujarat", "Khedabrahma-Sabarkantha-Gujarat"),
                new City("Kolhapur, Maharastra", "Kolhapur-Maharastra"),
                new City("Kolkata", "Kolkata-India"),
                new City("Kota, Rajasthan", "Kota-Rajasthan"),
                new City("Kumbakonam", "Kumbakonam-India"),
                new City("Kurukshetra", "Kurukshetra-India" ),
                new City("Lucknow", "Lucknow-India"),
                new City("Ludhiana", "Ludhiana-India"),
                new City("Machilipatnam, AP ", "Machilipatnam-AP-India"),
                new City("Madurai", "Madurai-India"),
                new City("Manali, Himachal Pradesh", "Manali-HimachalPradesh-India"),
                new City("Mandi, Himachal Pradesh", "Mandi-HimachalPradesh-India"),
                new City("Mangalore, Karnataka", "Mangalore-Karnataka-India"),
                new City("Mathura, UP", "Mathura-UP"),
                new City("Meerut, UP", "Meerut-UP"),
                new City("Mehsana", "Mehsana-India"),
                new City("Mumbai, Maharastra", "Mumbai-Maharastra-India"),
                new City("Mysore, Karnataka, India", "Mysore-Karnataka-India"),
                new City("Nagpur, Maharastra", "Nagpur-Maharastra"),
                new City("Nasik, Maharastra", "Nasik-Maharastra"),
                new City("Nainital, Uttarakhand", "Nainital-Uttarakhand"),
                new City("Nathdwara (Lord Srinathji Temple)", "Nathdwara(LordSrinathjiTemple)-India"),
                new City("Nellore", "Nellore-India"),
                new City("New Delhi", "NewDelhi-India"),
                new City("Noida, UP", "Noida-UP"),
                new City("Palakkad(Palghat), Kerala", "Palakkad(Palghat)-Kerala"),
                new City("Panaji (Panjim), Goa", "Panaji(Panjim)-Goa"),
                new City("Pandharpur, Maharastra", "Pandharpur-Maharastra"),
                new City("Patan, Gujarat", "Patan-Gujarat"),
                new City("Patiala, Punjab, India", "Patiala-Punjab-India"),
                new City("Patna, Bihar", "Patna-Bihar-India"),
                new City("Pondicherry", "Pondicherry-India"),
                new City("Proddatur, Andhra Pradesh", "Proddatur-AndhraPradesh-India"),
                new City("Pune, Maharastra", "Pune-Maharastra-India"),
                new City("Puri (Jagganath Puri)", "Puri(CityofLordJaggnath)-India"),
                new City("Puttur (Near Mangalore)", "PutturnearMangalore"),
                new City("Raipur, Chhattisgarh", "Raipur-Chhattisgarh"),
                new City("Rajahmundry", "Rajahmundry-India"),
                new City("Rajkot", "Rajkot-India"),
                new City("Ranchi, Jharkhand", "Ranchi-Jharkhand"),
                new City("Rishikesh, Uttarakhand", "Rishikesh-Uttarakhand"),
                new City("Rohtak, Hariyana", "Rohtak-Hariyana"),
                new City("Roorkee", "Roorkee-India"),
                new City("Saharanpur, UP", "Saharanpur-UP"),
                new City("Sangli, Maharastra", "Sangli-Maharastra"),
                new City("Satara, Maharastra", "Satara-Maharastra"),
                new City("Secunderabad, Andra Pradesh", "Secunderabad-AndraPradesh-India"),
                new City("Shimla, Himachal Pradesh", "Shimla-HimachalPradesh"),
                new City("Shiradi (Saibaba Temple)", "Shiradi(SaibabaTemple)-Maharastra"),
                new City("Srirangam / Thiruvarangam, Tamilnadu", "Srirangam(Thiruvarangam)-India"),
                new City("Surat", "Surat-India"),
                new City("Thiruvananthapuram, Kerala", "Thiruvananthapuram-Kerala"),
                new City("Thiruvarangam / Srirangam, Tamilnadu", "Srirangam(Thiruvarangam)-India"),
                new City("Tirumala, AP", "Tirumala-AP-India"),
                new City("Udaipur", "Udaipur"),
                new City("Udipi, Karnataka, India", "Udipi-Karnataka-India"),
                new City("Ujjain", "Ujjain-India"),
                new City("Uttarkashi, Uttarakhand", "Uttarkashi-Uttarakhand"),
                new City("Vadodara", "Baroda-India"),
                new City("Varanasi", "Varanasi-India"),
                new City("Vijayawada", "Vijayawada-India"),
                new City("Vishakhapatnam", "Vishakhapatnam-India"),
                new City("Zirapur, Rajgarh, MP", "Zirapur-Rajgarh-MP")
            };

            City[] Srilanka = {
                new City( "Colombo, Srilanka",  "Colombo-Srilanka" ),
                new City( "Jaffna, Srilanka",  "Jaffna-Srilanka" ),
                new City( "Kandy, SriLanka",  "Kandy-SriLanka" )
            };

            City[] Pakistan = {
                new City( "Karachi, Pakistan",  "Karachi-Pakistan" ),
                new City( "Islamabad, Pakistan",  "Islamabad-Pakistan" ),
                new City( "Lahore, Pakistan",  "Lahore-Pakistan" )
            };

            City[] SouthEastAsia = {
                new City( "Antipolo, Philippines",  "Antipolo-Philippines" ),
                new City( "Bali, Indonesia",  "Bali-Indonesia" ),
                new City( "Bangkok, Thailand",  "Bangkok-Thailand" ),
                new City( "Dhaka, Bangladesh",  "Dhaka-Bangladesh" ),
                new City( "Hanoi, Vietnam",  "Hanoi-Vietnam" ),
                new City( "HongKong, China",  "HongKong-China" ),
                new City( "Jakarta, Indonesia",  "Jakarta-Indonesia" ),
                new City( "Kuala Lumpur, Malaysia",  "KualaLumpur-Malaysia" ),
                new City( "Singapore",  "Singapore" )
            };

            City[] ChinaJapan = {
                new City( "Shanghai, China",  "Shanghai-China" ),
                new City( "Tokyo, Japan",  "Tokyo-Japan" ),
                new City( "Ulaanbaatar, Mongolia",  "Ulaanbaatar-Mongolia" ),
                new City( "Yiwu(Zhejiang), China",  "Yiwu(Zhejiang)-China" )
            };

            State[] Americas = {
                new State ( "Alabama (AL)", AL, TimeZoneValues.CST ),
                new State ( "Alaska (AK)", AK, TimeZoneValues.PST ),
                new State ( "Arizona (AZ)", AZ , TimeZoneValues.MST),
                new State ( "Arkansas (AR)", AR , TimeZoneValues.CST),
                new State ( "California (CA)", CA, TimeZoneValues.PST ),
                new State ( "Colorado (CO)", CO , TimeZoneValues.MST),
                new State ( "Connecticut (CT)", CT, TimeZoneValues.EST ),
                new State ( "Delaware (DE)", DE, TimeZoneValues.EST ),
                new State ( "Florida (FL)", FL, TimeZoneValues.EST ),
                new State ( "Georgia (GA)", GA, TimeZoneValues.EST ),
                new State ( "Hawaii (HI)", HI, TimeZoneValues.Hawaii ),
                new State ( "Idaho (ID)", ID, TimeZoneValues.MST ),
                new State ( "Illinois (IL)", IL , TimeZoneValues.CST),
                new State ( "Indiana (IN)", IN , TimeZoneValues.EST),
                new State ( "Iowa (IA)", IA , TimeZoneValues.CST),
                new State ( "Kansas (KS)", KS , TimeZoneValues.CST),
                new State ( "Kentucky (KY)", KY , TimeZoneValues.EST),
                new State ( "Louisiana (LA)", LA , TimeZoneValues.CST),
                new State ( "Maine (ME)", ME , TimeZoneValues.EST),
                new State ( "Maryland (MD)", MD, TimeZoneValues.EST ),
                new State ( "Massachusetts  (MA)", MA , TimeZoneValues.EST),
                new State ( "Michigan   (MI)", MI , TimeZoneValues.EST),
                new State ( "Minnesota (MN)", MN, TimeZoneValues.CST ),
                new State ( "Mississippi (MS)", MS , TimeZoneValues.CST),
                new State ( "Missouri  (MO)", MO , TimeZoneValues.CST),
                new State ( "Montana (MT)", MT , TimeZoneValues.MST),
                new State ( "Nebraska (NE)", NE, TimeZoneValues.CST ),
                new State ( "Nevada  (NV)", NV , TimeZoneValues.PST),
                new State ( "New Hampshire (NH)", NH , TimeZoneValues.EST),
                new State ( "New Jersey (NJ)", NJ , TimeZoneValues.EST),
                new State ( "New Mexico (NM)", NM , TimeZoneValues.MST),
                new State ( "New York (NY)", NY , TimeZoneValues.EST),
                new State ( "North Carolina (NC)", NC , TimeZoneValues.EST),
                new State ( "North Dakota (ND)", ND, TimeZoneValues.CST ),
                new State ( "Ohio (OH)", OH , TimeZoneValues.EST),
                new State ( "Oklahoma (OK)", OK , TimeZoneValues.CST),
                new State ( "Oregon (OR)", OR, TimeZoneValues.PST ),
                new State ( "Pennsylvania  (PA)", PA , TimeZoneValues.EST),
                new State ( "Puerto Rico", PRico , TimeZoneValues.CST),
                new State ( "Rhode Island (RI)", RI , TimeZoneValues.EST),
                new State ( "South Carolina (SC)", SC , TimeZoneValues.EST),
                new State ( "South Dakota (SD)", SD, TimeZoneValues.MST ),
                new State ( "Tennessee (TN)", TN , TimeZoneValues.CST),
                new State ( "Texas (TX)", TX , TimeZoneValues.CST),
                new State ( "Utah (UT)", UT , TimeZoneValues.MST),
                new State ( "Vermont (VT)", VT , TimeZoneValues.EST),
                new State ( "Virginia (VA)", VA , TimeZoneValues.EST),
                new State ( "Washington (WA)", WA , TimeZoneValues.PST),
                new State ( "Washington, D.C.  (DC)", DC , TimeZoneValues.PST),
                new State ( "West Virginia  (WV)", WV , TimeZoneValues.EST),
                new State ( "Wisconsin (WI)", WI , TimeZoneValues.CST),
                new State ( "Wyoming (WY)", WY, TimeZoneValues.MST )
            };

            SubContinent[] subContinents = {
                new SubContinent (  "Africa", Africa , TimeZoneValues.Unknown),
                new SubContinent (  "Americas ", Americas, TimeZoneValues.Unknown ),
                new SubContinent (  "Canada ", Canada, TimeZoneValues.Unknown ),
                new SubContinent (  "Carribean", Carribean , TimeZoneValues.Unknown),
                new SubContinent (  "South America ", SouthAmerica , TimeZoneValues.Unknown),
                new SubContinent (  "Europe ", Europe , TimeZoneValues.Unknown),
                new SubContinent (  "Middle East", ArabianGulf , TimeZoneValues.Unknown),
                new SubContinent (  "India", India , TimeZoneValues.India),
                new SubContinent (  "Pakistan", Pakistan, TimeZoneValues.India),
                new SubContinent (  "Srilanka", Srilanka, TimeZoneValues.India ),
                new SubContinent (  "China & Japan", ChinaJapan, TimeZoneValues.Unknown ),
                new SubContinent (  "South East Asia ", SouthEastAsia, TimeZoneValues.Malaysia ),
                new SubContinent (  "Australia ", Australia, TimeZoneValues.Unknown),
                new SubContinent (  "Newzealand", Newzealand, TimeZoneValues.NZ ),
                new SubContinent (  "Fiji", Fiji, TimeZoneValues.Fiji )
            };

            return subContinents;
        }
    }
}