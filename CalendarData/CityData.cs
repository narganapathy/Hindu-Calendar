
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;
using HtmlAgilityPack;


namespace CalendarData
{
        class StateOrCity
        {
            public String _Name;
        };

        class City : StateOrCity
        {
            public String _UrlToken;// Valid if its a city
            public CalendarData.TimeZoneValues _timeZone;
            public City(String Name, String UrlToken)
            {
                _Name = Name;
                _UrlToken = UrlToken;
                _timeZone = TimeZoneValues.Unknown;
            }
            public City(String Name, String UrlToken, CalendarData.TimeZoneValues timeZone)
            {
                _Name = Name;
                _timeZone = timeZone;
                _UrlToken = UrlToken;
            }
        };

        class State : StateOrCity
        {
            public City[] _cities; // Valid if its a state
            public CalendarData.TimeZoneValues _timeZone;
            public State(String Name, City[] cities, TimeZoneValues timeZone)
            {
                _Name = Name;
                _cities = cities;
                _timeZone = timeZone;
            }
        };

        class SubContinent
        {
            public String _Name;
            public StateOrCity[] _stateOrCityList;
            public CalendarData.TimeZoneValues _timeZone;
            public SubContinent(String name, StateOrCity[] stateOrCityList, CalendarData.TimeZoneValues timeZone)
            {
                _Name = name;
                _stateOrCityList = stateOrCityList;
                _timeZone = timeZone;
            }
        };


    class CityData
    {
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
                new City ("Roseville, CA", "Roseville(PlacerCounty)-CA"),
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
                new City("Aberdeen, Scotland", "Aberdeen-Scotland-UK"),
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
                new City("Fort-de-France, Martinique", "Fort-de-France-Martinique"),
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
                new City( "Singapore, Singapore",  "Singapore" )
            };

            City[] ChinaJapan = {
                new City( "Shanghai, China",  "Shanghai-China" ),
                new City( "Tokyo, Japan",  "Tokyo-Japan" ),
                new City( "Ulaanbaatar, Mongolia",  "Ulaanbaatar-Mongolia" ),
                new City( "Yiwu Zhejiang, China",  "Yiwu(Zhejiang)-China" )
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
                new SubContinent (  "Africa new SubContinent (Daylight saving time adjusted)", Africa , TimeZoneValues.Unknown),
                new SubContinent (  "Americas new SubContinent (Daylight saving time adjusted)", Americas, TimeZoneValues.Unknown ),
                new SubContinent (  "Canada new SubContinent (Daylight saving time adjusted)", Canada, TimeZoneValues.Unknown ),
                new SubContinent (  "Carribean", Carribean , TimeZoneValues.Unknown),
                new SubContinent (  "South America new SubContinent (Daylight saving time adjusted)", SouthAmerica , TimeZoneValues.Unknown),
                new SubContinent (  "Europe new SubContinent (Daylight saving time adjusted)", Europe , TimeZoneValues.Unknown),
                new SubContinent (  "Middle East new SubContinent (Gulf)", ArabianGulf , TimeZoneValues.Unknown),
                new SubContinent (  "India", India , TimeZoneValues.India),
                new SubContinent (  "Pakistan", Pakistan, TimeZoneValues.India),
                new SubContinent (  "Srilanka", Srilanka, TimeZoneValues.India ),
                new SubContinent (  "China & Japan", ChinaJapan, TimeZoneValues.Unknown ),
                new SubContinent (  "South East Asia new SubContinent (Daylight saving time adjusted)", SouthEastAsia, TimeZoneValues.Malaysia ),
                new SubContinent (  "Australia new SubContinent (Daylight saving time adjusted)", Australia, TimeZoneValues.Unknown),
                new SubContinent (  "Newzealand new SubContinent (Daylight adjusted)", Newzealand, TimeZoneValues.NZ ),
                new SubContinent (  "Fiji", Fiji, TimeZoneValues.Fiji )
            };

            return subContinents;
        }
    }
}