using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace PhoneNumbersNA
{
    public static class Parse
    {
        public static IEnumerable<PhoneNumber> AsPhoneNumbers(string str)
        {
            return AreaCode.ExtractPhoneNumbers(str);
        }
        public static IEnumerable<string> AsDialedNumbers(string str)
        {
            return AreaCode.ExtractDialedNumbers(str);
        }
    }
    public static class AreaCode
    {
        /// <summary>
        /// All in service NANPA NPAs (Area Codes).
        /// https://nationalnanpa.com/enas/geoAreaCodeNumberReport.do
        /// </summary>
        public static readonly int[] All = new int[]
        {
            201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 223, 224, 225, 226, 227, 228, 229, 231, 234, 235, 236, 239, 240, 242, 246, 248, 249, 250, 251, 252, 253, 254, 256, 260, 262, 263, 264, 267, 268, 269, 270, 272, 274, 276, 278, 279, 281, 283, 284, 289,
            301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 323, 325, 326, 327, 330, 331, 332, 334, 336, 337, 339, 340, 341, 343, 345, 346, 347, 351, 352, 360, 361, 364, 365, 367, 368, 380, 381, 385, 386, 387,
            401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 423, 424, 425, 428, 430, 431, 432, 434, 435, 437, 438, 440, 441, 442, 443, 445, 447, 448, 450, 456, 458, 463, 464, 468, 469, 470, 473, 474, 475, 478, 479, 480, 484,
            500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524, 525, 526, 527, 528, 530, 531, 532, 533, 534, 535, 538, 539, 540, 541, 544, 548, 551, 559, 561, 562, 563, 564, 566, 567, 570, 571, 572, 573, 574, 575, 577, 578, 579, 580, 581, 582, 584, 585, 586, 587, 588,
            600, 601, 602, 603, 604, 605, 606, 607, 608, 609, 610, 612, 613, 614, 615, 616, 617, 618, 619, 620, 622, 623, 626, 628, 629, 630, 631, 636, 639, 640, 641, 646, 647, 649, 650, 651, 656, 657, 658, 659, 660, 661, 662, 664, 667, 669, 670, 671, 672, 678, 679, 680, 681, 682, 683, 684, 689,
            700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710, 711, 712, 713, 714, 715, 716, 717, 718, 719, 720, 721, 724, 725, 726, 727, 730, 731, 732, 734, 737, 740, 742, 743, 747, 753, 754, 757, 758, 760, 762, 763, 765, 767, 769, 770, 771, 772, 773, 774, 775, 778, 779, 780, 781, 782, 784, 785, 786, 787,
            800, 801, 802, 803, 804, 805, 806, 807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 825, 828, 829, 830, 831, 832, 833, 838, 839, 840, 843, 844, 845, 847, 848, 849, 850, 854, 855, 856, 857, 858, 859, 860, 862, 863, 864, 865, 866, 867, 868, 869, 870, 872, 873, 876, 877, 878, 879, 888, 889,
            900, 901, 902, 903, 904, 905, 906, 907, 908, 909, 910, 912, 913, 914, 915, 916, 917, 918, 919, 920, 925, 927, 928, 929, 930, 931, 934, 935, 936, 937, 938, 939, 940, 941, 943, 945, 947, 948, 949, 951, 952, 954, 956, 959, 970, 971, 972, 973, 978, 979, 980, 984, 985, 986, 989
        };

        /// <summary>
        /// All in service NANPA Tollfree NPAs (Area Codes).
        /// https://nationalnanpa.com/enas/nonGeoNpaServiceReport.do
        /// </summary>
        public static readonly int[] TollFree = new int[]
        {
            800, 833, 844, 855, 866, 877, 888
        };

        /// <summary>
        /// All in service NANPA Non-geographics NPAs (Area Codes) including tollfree numbers.
        /// https://nationalnanpa.com/enas/nonGeoNpaServiceReport.do
        /// </summary>
        public static readonly int[] NonGeographic = new int[]
        {
            500,  521,  522,  523,  524,  525,  526,  527,  528,  533,  544,  566,  577,  588,
            600,  622,
            700,  710,
            800,  833,  844,  855,  866,  877,  888,
            900
        };

        /// <summary>
        /// All in service NPAs maintained by the CNA ( Canadian Numbering Administrator ).
        /// https://cnac.ca/co_codes/co_code_lookup.htm
        /// </summary>
        public static readonly int[] Canadian = new int[]
        {
            204, 226, 236, 249, 250, 263, 289,
            306, 343, 354, 365, 367, 368, 382,
            403, 416, 418, 428, 431, 437, 438, 450, 468, 474,
            506, 514, 519, 548, 579, 581, 584, 587,
            604, 613, 639, 647, 672, 683,
            705, 709, 742, 753, 778, 780, 782,
            807, 819, 825, 867, 873, 879,
            902, 905
        };

        /// <summary>
        /// All in service NANPA Geographics NPAs (Area Codes) not directly in the United States or Canada. Includes sovereign nations and US territories.
        /// https://nationalnanpa.com/area_code_maps/area_code_maps_Country_Territory.html
        /// </summary>
        public static readonly int[] CountryOrTerritory = new int[]
        {
            242, 246, 264, 268, 284,
            340, 345,
            441, 473,
            649, 664, 670, 671, 684,
            721, 758, 767, 784, 787,
            809, 829, 849, 868, 869, 876,
            939
        };

        // Rather than directly looping over the area code arrays or storing them in a dictionary we can use a simple array of boolean as lookup.
        // Where the index of the array is the area code as an int and the boolean returned discribes if its valid or not.
        // Using the PhoneNumbersNA.Benchmark console app we verified that this flat lookup is faster than a simple loop or dictionary.
        public static readonly bool[] AllFlatLookup = AllFlat();
        public static readonly bool[] TollFreeFlatLookup = TollFreeFlat();
        public static readonly bool[] NonGeographicFlatLookup = NonGeographicFlat();
        public static readonly bool[] CanadianFlatLookup = CanadianFlat();
        public static readonly bool[] CountryOrTerritoryFlatLookup = CountryOrTerritoryFlat();

        public static Dictionary<int, int> AllDict = All.ToDictionary(x => x, y => y);

        // Generator methods for the flat lookups.
        private static bool[] AllFlat()
        {
            var array = new bool[999];

            foreach (var code in All)
            {
                array[code] = true;
            }

            return array;
        }

        private static bool[] TollFreeFlat()
        {
            var array = new bool[999];

            foreach (var code in TollFree)
            {
                array[code] = true;
            }

            return array;
        }

        private static bool[] NonGeographicFlat()
        {
            var array = new bool[999];

            foreach (var code in NonGeographic)
            {
                array[code] = true;
            }

            return array;
        }

        private static bool[] CanadianFlat()
        {
            var array = new bool[999];

            foreach (var code in Canadian)
            {
                array[code] = true;
            }

            return array;
        }

        private static bool[] CountryOrTerritoryFlat()
        {
            var array = new bool[999];

            foreach (var code in CountryOrTerritory)
            {
                array[code] = true;
            }

            return array;
        }

        public class AreaCodesByState
        {
            public string? State { get; set; }
            public string? StateShort { get; set; }
            public int[]? AreaCodes { get; set; }
        }

        /// <summary>
        /// NANPA AreaCodes by Location https://nationalnanpa.com/enas/geoAreaCodeNumberReport.do
        /// </summary>
        public static readonly AreaCodesByState[] States = new AreaCodesByState[]
        {
            new AreaCodesByState
            {
                State = "Alabama",
                StateShort = "AL",
                AreaCodes = new int[]
                {
                    205, 251, 256, 334, 938
                }
            },
            new AreaCodesByState
            {
                State = "Alaska",
                StateShort = "AK",
                AreaCodes = new int[]
                {
                    907
                }
            },
            new AreaCodesByState
            {
                State = "Arizona",
                StateShort = "AZ",
                AreaCodes = new int[]
                {
                    480, 520, 602, 623, 928
                }
            },
            new AreaCodesByState
            {
                State = "Arkansas",
                StateShort = "AR",
                AreaCodes = new int[]
                {
                    479, 501, 870
                }
            },
            new AreaCodesByState
            {
                State = "California",
                StateShort = "CA",
                AreaCodes = new int[]
                {
                    209, 213, 279, 310, 323, 408, 415, 424, 442, 510, 530, 559, 562, 619, 626, 628, 650, 657, 661, 669, 707, 714, 747, 760, 805, 818, 820, 831, 858, 909, 916, 925, 949, 951
                }
            },
            new AreaCodesByState
            {
                State = "Colorado",
                StateShort = "CO",
                AreaCodes = new int[]
                {
                    303, 719, 720, 970
                }
            },
            new AreaCodesByState
            {
                State = "Connecticut",
                StateShort = "CT",
                AreaCodes = new int[]
                {
                    203, 475, 860, 959
                }
            },
            new AreaCodesByState
            {
                State = "Delaware",
                StateShort = "DE",
                AreaCodes = new int[]
                {
                    302
                }
            },
            new AreaCodesByState
            {
                State = "Florida",
                StateShort = "FL",
                AreaCodes = new int[]
                {
                    239, 305, 321, 352, 386, 407, 561, 727, 754, 772, 786, 813, 850, 863, 904, 941, 954
                }
            },
            new AreaCodesByState
            {
                State = "Georgia",
                StateShort = "GA",
                AreaCodes = new int[]
                {
                    229, 404, 470, 478, 678, 706, 762, 770, 912, 943
                }
            },
            new AreaCodesByState
            {
                State = "Hawaii",
                StateShort = "HI",
                AreaCodes = new int[]
                {
                   808
                }
            },
            new AreaCodesByState
            {
                State = "Idaho",
                StateShort = "ID",
                AreaCodes = new int[]
                {
                   208, 986
                }
            },
            new AreaCodesByState
            {
                State = "Illinois",
                StateShort = "IL",
                AreaCodes = new int[]
                {
                   217, 224, 309, 312, 331, 618, 630, 708, 773, 779, 815, 847, 872
                }
            },
            new AreaCodesByState
            {
                State = "Indiana",
                StateShort = "IN",
                AreaCodes = new int[]
                {
                   219, 260, 317, 463, 574, 765, 812, 930
                }
            },
            new AreaCodesByState
            {
                State = "Iowa",
                StateShort = "IA",
                AreaCodes = new int[]
                {
                   319, 515, 563, 641, 712
                }
            },
            new AreaCodesByState
            {
                State = "Kansas",
                StateShort = "KS",
                AreaCodes = new int[]
                {
                    316, 620, 785, 913
                }
            },
            new AreaCodesByState
            {
                State = "Kentucky",
                StateShort = "KY",
                AreaCodes = new int[]
                {
                    270, 364, 502, 606, 859
                }
            },
            new AreaCodesByState
            {
                State = "Louisiana",
                StateShort = "LA",
                AreaCodes = new int[]
                {
                    225, 318, 337, 504, 985
                }
            },
            new AreaCodesByState
            {
                State = "Maine",
                StateShort = "ME",
                AreaCodes = new int[]
                {
                    207
                }
            },
            new AreaCodesByState
            {
                State = "Maryland",
                StateShort = "MD",
                AreaCodes = new int[]
                {
                    240, 301, 410, 443, 667
                }
            },
            new AreaCodesByState
            {
                State = "Massachusetts",
                StateShort = "MA",
                AreaCodes = new int[]
                {
                    339, 351, 413, 508, 617, 774, 781, 857, 978
                }
            },
            new AreaCodesByState
            {
                State = "Michigan",
                StateShort = "MI",
                AreaCodes = new int[]
                {
                    231, 248, 269, 313, 517, 586, 616, 734, 810, 906, 947, 989
                }
            },
            new AreaCodesByState
            {
                State = "Minnesota",
                StateShort = "MN",
                AreaCodes = new int[]
                {
                    218, 320, 507, 612, 651, 763, 952
                }
            },
            new AreaCodesByState
            {
                State = "Mississippi",
                StateShort = "MS",
                AreaCodes = new int[]
                {
                    228, 601, 662, 769
                }
            },
            new AreaCodesByState
            {
                State = "Missouri",
                StateShort = "MO",
                AreaCodes = new int[]
                {
                    314, 417, 573, 636, 660, 816
                }
            },
            new AreaCodesByState
            {
                State = "Montana",
                StateShort = "MT",
                AreaCodes = new int[]
                {
                    406
                }
            },
            new AreaCodesByState
            {
                State = "Nebraska",
                StateShort = "NE",
                AreaCodes = new int[]
                {
                    308, 402, 531
                }
            },
            new AreaCodesByState
            {
                State = "Nevada",
                StateShort = "NV",
                AreaCodes = new int[]
                {
                    702, 725, 775
                }
            },
            new AreaCodesByState
            {
                State = "New Hampshire",
                StateShort = "NH",
                AreaCodes = new int[]
                {
                    603
                }
            },
            new AreaCodesByState
            {
                State = "New Jersey",
                StateShort = "NJ",
                AreaCodes = new int[]
                {
                    201, 551, 609, 640, 732, 848, 856, 862, 908, 973
                }
            },
            new AreaCodesByState
            {
                State = "New Mexico",
                StateShort = "NM",
                AreaCodes = new int[]
                {
                    505, 575
                }
            },
            new AreaCodesByState
            {
                State = "New York",
                StateShort = "NY",
                AreaCodes = new int[]
                {
                    212, 315, 332, 347, 516, 518, 585, 607, 631, 646, 680, 716, 718, 838, 845, 914, 917, 929, 934
                }
            },
            new AreaCodesByState
            {
                State = "North Carolina",
                StateShort = "NC",
                AreaCodes = new int[]
                {
                    252, 336, 704, 743, 828, 910, 919, 980, 984
                }
            },
            new AreaCodesByState
            {
                State = "Ohio",
                StateShort = "OH",
                AreaCodes = new int[]
                {
                    216, 220, 234, 330, 380, 419, 440, 513, 567, 614, 740, 937
                }
            },
            new AreaCodesByState
            {
                State = "Oklahoma",
                StateShort = "OK",
                AreaCodes = new int[]
                {
                    405, 539, 580, 918
                }
            },
            new AreaCodesByState
            {
                State = "Oregon",
                StateShort = "OR",
                AreaCodes = new int[]
                {
                    458, 503, 541, 971
                }
            },
            new AreaCodesByState
            {
                State = "Pennsylvania",
                StateShort = "PA",
                AreaCodes = new int[]
                {
                    215, 223, 267, 272, 412, 445, 484, 570, 610, 717, 724, 814, 878
                }
            },
            new AreaCodesByState
            {
                State = "Rhode Island",
                StateShort = "RI",
                AreaCodes = new int[]
                {
                    401
                }
            },
            new AreaCodesByState
            {
                State = "South Carolina",
                StateShort = "SC",
                AreaCodes = new int[]
                {
                    803, 843, 854, 864
                }
            },
            new AreaCodesByState
            {
                State = "South Dakota",
                StateShort = "SD",
                AreaCodes = new int[]
                {
                    605
                }
            },
            new AreaCodesByState
            {
                State = "Tennessee",
                StateShort = "TN",
                AreaCodes = new int[]
                {
                    423, 615, 629, 731, 865, 901, 931
                }
            },
            new AreaCodesByState
            {
                State = "Texas",
                StateShort = "TX",
                AreaCodes = new int[]
                {
                    210, 214, 254, 281, 325, 346, 361, 409, 430, 432, 469, 512, 682, 713, 726, 737, 806, 817, 830, 832, 903, 915, 936, 940, 956, 972, 979
                }
            },
            new AreaCodesByState
            {
                State = "Utah",
                StateShort = "UT",
                AreaCodes = new int[]
                {
                    385, 435, 801
                }
            },
            new AreaCodesByState
            {
                State = "Vermont",
                StateShort = "VT",
                AreaCodes = new int[]
                {
                    802
                }
            },
            new AreaCodesByState
            {
                State = "Virginia",
                StateShort = "VA",
                AreaCodes = new int[]
                {
                    276, 434, 540, 571, 703, 757, 804
                }
            },
            new AreaCodesByState
            {
                State = "Washington",
                StateShort = "WA",
                AreaCodes = new int[]
                {
                    206, 253, 360, 425, 509, 564
                }
            },
            new AreaCodesByState
            {
                State = "Washington, DC",
                StateShort = "DC",
                AreaCodes = new int[]
                {
                    202
                }
            },
            new AreaCodesByState
            {
                State = "West Virginia",
                StateShort = "WV",
                AreaCodes = new int[]
                {
                    304, 681
                }
            },
            new AreaCodesByState
            {
                State = "Wisconsin",
                StateShort = "WI",
                AreaCodes = new int[]
                {
                    262, 414, 534, 608, 715, 920
                }
            },
            new AreaCodesByState
            {
                State = "Wyoming",
                StateShort = "WY",
                AreaCodes = new int[]
                {
                    307
                }
            }
        };

        /// <summary>
        /// Check if an NPA (Area Code) is in service and in Canada.
        /// </summary>
        /// <param name="npa"></param>
        /// <returns></returns>
        public static bool ValidCanadian(int npa)
        {
            return CanadianFlatLookup[npa];
        }

        /// <summary>
        /// Check if a string is an NPA (Area Code), is in service and in Canada.
        /// </summary>
        /// <param name="npa"></param>
        /// <returns></returns>
        public static bool ValidCanadian(string npa)
        {
            if (!string.IsNullOrWhiteSpace(npa) && npa.Length == 3)
            {
                var checkParse = int.TryParse(npa, out var canadian);

                if (checkParse)
                {
                    return ValidCanadian(canadian);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if an NPA or phone number is a valid Canadian number.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsCanadian(this string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                return str.Length switch
                {
                    10 => ValidCanadian(str[..3]),
                    _ => ValidCanadian(str),
                };
            }

            return false;
        }

        /// <summary>
        /// Check if an NPA (Area Code) is in service and in not in the US or Canada.
        /// </summary>
        /// <param name="npa"></param>
        /// <returns></returns>
        public static bool ValidCountryOrTerritory(int npa)
        {
            return CountryOrTerritoryFlatLookup[npa];
        }

        /// <summary>
        /// Check if a string is an NPA (Area Code), is in service and in not in the US or Canada.
        /// </summary>
        /// <param name="npa"></param>
        /// <returns></returns>
        public static bool ValidCountryOrTerritory(string npa)
        {
            if (!string.IsNullOrWhiteSpace(npa) && npa.Length == 3)
            {
                var checkParse = int.TryParse(npa, out var country);

                if (checkParse)
                {
                    return ValidCountryOrTerritory(country);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if an NPA or phone number is a valid non-US or Canadian number.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsCountryOrTerritory(this string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                return str.Length switch
                {
                    10 => ValidCountryOrTerritory(str[..3]),
                    _ => ValidCountryOrTerritory(str),
                };
            }

            return false;
        }

        /// <summary>
        /// Check if an NPA (Area Code) is in service and Non-geographic.
        /// </summary>
        /// <param name="npa"></param>
        /// <returns></returns>
        public static bool ValidNonGeographic(int npa)
        {
            return NonGeographicFlatLookup[npa];
        }

        /// <summary>
        /// Check if a string is an NPA (Area Code), is in service, and non geographic.
        /// </summary>
        /// <param name="npa"></param>
        /// <returns></returns>
        public static bool ValidNonGeographic(string npa)
        {
            if (!string.IsNullOrWhiteSpace(npa) && npa.Length == 3)
            {
                var checkParse = int.TryParse(npa, out var nongeo);

                if (checkParse)
                {
                    return ValidNonGeographic(nongeo);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if an NPA or phone number is a valid non-geographic number.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNonGeographic(this string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                return str.Length switch
                {
                    10 => ValidNonGeographic(str[..3]),
                    _ => ValidNonGeographic(str),
                };
            }

            return false;
        }

        /// <summary>
        /// Check if an NPA (Area Code) is in service and Tollfree.
        /// </summary>
        /// <param name="npa"></param>
        /// <returns></returns>
        public static bool ValidTollfree(int npa)
        {
            return TollFreeFlatLookup[npa];
        }

        /// <summary>
        /// Check if a string is an NPA (Area Code), is in service, and Tollfree.
        /// </summary>
        /// <param name="npa"></param>
        /// <returns></returns>
        public static bool ValidTollfree(string npa)
        {
            if (!string.IsNullOrWhiteSpace(npa) && npa.Length == 3)
            {
                var checkParse = int.TryParse(npa, out var toll);

                if (checkParse)
                {
                    return ValidTollfree(toll);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if an NPA or phone number is a valid tollfree number.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsTollfree(this string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                return str.Length switch
                {
                    10 => ValidTollfree(str[..3]),
                    _ => ValidTollfree(str),
                };
            }

            return false;
        }

        /// <summary>
        /// NPA's use the format of "NXX" where "N" is any digit between 2 and 9 and "X" is any digit from 0 to 9.
        /// https://www.nationalnanpa.com/about_us/abt_nanp.html
        /// </summary>
        /// <param name="nxx"></param>
        /// <returns></returns>
        public static bool ValidNPA(int npa)
        {
            var checkGet = AllDict.TryGetValue(npa, out int value);

            return checkGet && value == npa;

        }

        /// <summary>
        /// Check if a string is a valid NANPA phone number NPA (Area Code) sequence in the format of "NXX".
        /// </summary>
        /// <param name="npa"></param>
        /// <returns></returns>
        public static bool ValidNPA(string npa)
        {
            if (!string.IsNullOrWhiteSpace(npa) && npa.Length == 3)
            {
                var checkParse = int.TryParse(npa, out var code);

                if (checkParse)
                {
                    return ValidNPA(code);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if a string is a valid NANPA phone number NPA (Area Code) sequence in the format of "NXX".
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValidNPA(this string str)
        {
            return ValidNPA(str);
        }

        /// <summary>
        /// NXX's use the format of "NXX" where "N" is any digit between 2 and 9 and "X" is any digit from 0 to 9.
        /// https://www.nationalnanpa.com/about_us/abt_nanp.html
        /// </summary>
        /// <param name="nxx"></param>
        /// <returns></returns>
        public static bool ValidNXX(int nxx)
        {
            if (nxx >= 200 && nxx <= 999)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validate a string-ly typed 3 digit nxx sequence in the format of "NXX".
        /// </summary>
        /// <param name="nxx"></param>
        /// <returns></returns>
        public static bool ValidNXX(string nxx)
        {
            if (!string.IsNullOrWhiteSpace(nxx) && nxx.Length == 3)
            {
                var checkParse = int.TryParse(nxx, out var office);
                if (checkParse)
                {
                    return ValidNXX(office);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if a string is a valid NANPA phone number sequence in the format of "NXX".
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValidNXX(this string str)
        {
            return ValidNXX(str);
        }

        /// <summary>
        /// XXXX's use the format of "XXXX" where "X" is any digit between 0 and 9.
        /// </summary>
        /// <param name="xxxx"></param>
        /// <returns></returns>
        public static bool ValidXXXX(int xxxx)
        {
            if (xxxx >= 0 && xxxx <= 9999)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validate a string-ly typed 4 digit vanity sequence in the format of "XXXX".
        /// </summary>
        /// <param name="xxxx"></param>
        /// <returns></returns>
        public static bool ValidXXXX(string xxxx)
        {
            if (!string.IsNullOrWhiteSpace(xxxx) && xxxx.Length == 4)
            {
                var checkParse = int.TryParse(xxxx, out var vanity);

                if (checkParse)
                {
                    return ValidXXXX(vanity);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if a string is a valid NANPA phone number vanity sequence in the format of "XXXX"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValidXXXX(this string str)
        {
            return ValidXXXX(str);
        }

        /// <summary>
        /// Validate a phone number using its componet NPA, NXX, and XXXX parts.
        /// </summary>
        /// <param name="npa"></param>
        /// <param name="nxx"></param>
        /// <param name="xxxx"></param>
        /// <returns></returns>
        public static bool ValidPhoneNumber(int npa, int nxx, int xxxx)
        {
            return ValidNPA(npa) && ValidNXX(nxx) && ValidXXXX(xxxx);
        }

        /// <summary>
        /// Validate a string-ly typed 10 digit phone number.
        /// </summary>
        /// <param name="dialedNumber"></param>
        /// <returns></returns>
        public static bool ValidPhoneNumber(string dialedNumber)
        {
            if (!string.IsNullOrWhiteSpace(dialedNumber) && dialedNumber.Length == 10)
            {
                bool checkNpa = int.TryParse(dialedNumber.AsSpan(0, 3), out int npa);
                bool checkNxx = int.TryParse(dialedNumber.AsSpan(3, 3), out int nxx);
                bool checkXxxx = int.TryParse(dialedNumber.AsSpan(6, 4), out int xxxx);

                if (checkNpa && checkNxx && checkXxxx)
                {
                    return ValidPhoneNumber(npa, nxx, xxxx);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if a string is a valid NANPA phone number in the format of "NXXNXXXXXX"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValidPhoneNumber(this string input)
        {
            // Clean up the query.
            input = input.Trim().ToLowerInvariant();

            // Parse the query.
            var converted = new List<char>();
            foreach (var letter in input)
            {
                // Allow digits.
                if (char.IsDigit(letter))
                {
                    converted.Add(letter);
                }
                // Convert letters to digits.
                else if (char.IsLetter(letter))
                {
                    converted.Add(PhoneNumber.LetterToKeypadDigit(letter));
                }
                // Drop everything else.
            }

            // Drop leading 1's to improve the copy/paste experiance.
            if (converted.Count > 0 && converted[0] == '1' && converted.Count > 10)
            {
                converted.Remove('1');
            }

            return ValidPhoneNumber(converted.ToArray().AsSpan().ToString());
        }

        /// <summary>
        /// Get a list of 10 digit dialed phone numbers from an unformatted string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IEnumerable<string> ExtractDialedNumbers(this string str)
        {
            var parsedNumbers = new List<string>();

            if (!string.IsNullOrWhiteSpace(str))
            {
                var numberCanidates = str.Trim()
                    .Replace(") ", "")
                    .Replace("(", "")
                    .Replace(" ", "")
                    .Replace(",", " ")
                    .Replace("\r\n", " ")
                    .Split();

                foreach (var query in numberCanidates)
                {
                    // Parse the query.
                    var converted = new List<char>();
                    foreach (var letter in query)
                    {
                        // Allow digits.
                        if (char.IsDigit(letter))
                        {
                            converted.Add(letter);
                        }
                        // Drop everything else.
                    }

                    // Drop leading 1's to improve the copy/paste experiance.
                    if (converted.Count > 0 && converted.Count > 10 && converted[0] == '1')
                    {
                        converted.Remove('1');
                    }

                    // Only if its a perfect number do we want to return it.
                    if (converted.Count == 10)
                    {
                        parsedNumbers.Add(converted.ToArray().AsSpan().ToString());
                    }
                }
            }
            return parsedNumbers;
        }

        public static IEnumerable<PhoneNumber> ExtractPhoneNumbers(this string str)
        {
            var parsedNumbers = new List<PhoneNumber>();

            if (!string.IsNullOrWhiteSpace(str))
            {
                var cleanedInput = str.Trim()
                    .Replace(") ", "")
                    .Replace("(", "")
                    .Replace(" ", "")
                    .Replace(",", " ")
                    .Replace("\r\n", " ")
                    .AsSpan();

                var sliceIndex = 0;
                var query = new ReadOnlySpan<char>();
                while (sliceIndex > -1)
                {
                    sliceIndex = cleanedInput.IndexOf(' ');
                    if (sliceIndex != -1)
                    {
                        query = cleanedInput[..sliceIndex];
                        cleanedInput = cleanedInput[(sliceIndex + 1)..];
                    }

                    // Parse the query.
                    var converted = new List<char>();
                    foreach (var letter in query)
                    {
                        // Allow digits.
                        if (char.IsDigit(letter))
                        {
                            converted.Add(letter);
                        }
                        // Drop everything else.
                    }

                    // Drop leading 1's to improve the copy/paste experiance.
                    if (converted.Count > 0 && converted.Count > 10 && converted[0] == '1')
                    {
                        converted.Remove('1');
                    }

                    // Only if its a perfect number do we want to query for it.
                    if (converted.Count == 10)
                    {
                        var checkParse = PhoneNumber.TryParseExact(converted, out var phoneNumber);
                        if (checkParse && phoneNumber is not null)
                        {
                            parsedNumbers.Add(phoneNumber);
                        }
                    }
                }
            }
            return parsedNumbers;
        }
    }

    public enum NumberType
    {
        Local,
        Tollfree,
        NonGeographic,
        CountryOrTerritory,
        Canada
    }

    public class PhoneNumber
    {
        public string? DialedNumber { get; set; }
        public int NPA { get; set; }
        public int NXX { get; set; }
        public int XXXX { get; set; }
        public NumberType Type { get; set; }
        public DateTime DateIngested { get; set; }
        public string? IngestedFrom { get; set; }

        public static bool TryParse(string input, out PhoneNumber? number)
        {
            // Fail fast
            if (input is null || input?.Length < 10)
            {
                number = null;
                return false;
            }

            // Clean up the query.
            input = input!.Trim().ToLowerInvariant();

            var converted = new List<char>();
            foreach (var letter in input)
            {
                // Allow digits.
                if (char.IsDigit(letter))
                {
                    converted.Add(letter);
                }
                // Convert letters to digits.
                else if (char.IsLetter(letter))
                {
                    converted.Add(LetterToKeypadDigit(letter));
                }
                // Drop everything else.
            }

            // Drop leading 1's to improve the copy/paste experiance.
            if (converted.Count > 0 && converted[0] == '1' && converted.Count > 10)
            {
                converted.Remove('1');
            }

            // This input can't be parsed, so bail out.
            if (converted.Count != 10)
            {
                number = null;
                return false;
            }

            var cleanedQuery = converted.ToArray().AsSpan();

            bool checkNpa = int.TryParse(cleanedQuery.Slice(0, 3), out int npa);
            bool checkNxx = int.TryParse(cleanedQuery.Slice(3, 3), out int nxx);
            bool checkXxxx = int.TryParse(cleanedQuery.Slice(6, 4), out int xxxx);

            var checkValid = AreaCode.ValidPhoneNumber(npa, nxx, xxxx);

            if (checkNpa && checkNxx && checkXxxx && checkValid)
            {
                if (AreaCode.ValidTollfree(npa))
                {
                    number = new PhoneNumber
                    {
                        DialedNumber = cleanedQuery.ToString(),
                        NPA = npa,
                        NXX = nxx,
                        XXXX = xxxx,
                        Type = NumberType.Tollfree,
                        DateIngested = DateTime.Now
                    };
                }
                else if (AreaCode.ValidNonGeographic(npa))
                {
                    number = new PhoneNumber
                    {
                        DialedNumber = cleanedQuery.ToString(),
                        NPA = npa,
                        NXX = nxx,
                        XXXX = xxxx,
                        Type = NumberType.NonGeographic,
                        DateIngested = DateTime.Now
                    };
                }
                else if (AreaCode.ValidCanadian(npa))
                {
                    number = new PhoneNumber
                    {
                        DialedNumber = cleanedQuery.ToString(),
                        NPA = npa,
                        NXX = nxx,
                        XXXX = xxxx,
                        Type = NumberType.Canada,
                        DateIngested = DateTime.Now
                    };
                }
                else if (AreaCode.ValidCountryOrTerritory(npa))
                {
                    number = new PhoneNumber
                    {
                        DialedNumber = cleanedQuery.ToString(),
                        NPA = npa,
                        NXX = nxx,
                        XXXX = xxxx,
                        Type = NumberType.CountryOrTerritory,
                        DateIngested = DateTime.Now
                    };
                }
                else
                {
                    number = new PhoneNumber
                    {
                        DialedNumber = cleanedQuery.ToString(),
                        NPA = npa,
                        NXX = nxx,
                        XXXX = xxxx,
                        Type = NumberType.Local,
                        DateIngested = DateTime.Now
                    };
                }

                return true;
            }
            else
            {
                number = null;
                return false;
            }
        }

        public static bool TryParseExact(IEnumerable<char> input, out PhoneNumber? number)
        {
            var cleanedQuery = input.ToArray().AsSpan();

            bool checkNpa = int.TryParse(cleanedQuery.Slice(0, 3), out int npa);
            bool checkNxx = int.TryParse(cleanedQuery.Slice(3, 3), out int nxx);
            bool checkXxxx = int.TryParse(cleanedQuery.Slice(6, 4), out int xxxx);

            var checkValid = AreaCode.ValidPhoneNumber(npa, nxx, xxxx);

            if (checkNpa && checkNxx && checkXxxx && checkValid)
            {
                if (AreaCode.ValidTollfree(npa))
                {
                    number = new PhoneNumber
                    {
                        DialedNumber = cleanedQuery.ToString(),
                        NPA = npa,
                        NXX = nxx,
                        XXXX = xxxx,
                        Type = NumberType.Tollfree,
                        DateIngested = DateTime.Now
                    };
                }
                else if (AreaCode.ValidNonGeographic(npa))
                {
                    number = new PhoneNumber
                    {
                        DialedNumber = cleanedQuery.ToString(),
                        NPA = npa,
                        NXX = nxx,
                        XXXX = xxxx,
                        Type = NumberType.NonGeographic,
                        DateIngested = DateTime.Now
                    };
                }
                else if (AreaCode.ValidCanadian(npa))
                {
                    number = new PhoneNumber
                    {
                        DialedNumber = cleanedQuery.ToString(),
                        NPA = npa,
                        NXX = nxx,
                        XXXX = xxxx,
                        Type = NumberType.Canada,
                        DateIngested = DateTime.Now
                    };
                }
                else if (AreaCode.ValidCountryOrTerritory(npa))
                {
                    number = new PhoneNumber
                    {
                        DialedNumber = cleanedQuery.ToString(),
                        NPA = npa,
                        NXX = nxx,
                        XXXX = xxxx,
                        Type = NumberType.CountryOrTerritory,
                        DateIngested = DateTime.Now
                    };
                }
                else
                {
                    number = new PhoneNumber
                    {
                        DialedNumber = cleanedQuery.ToString(),
                        NPA = npa,
                        NXX = nxx,
                        XXXX = xxxx,
                        Type = NumberType.Local,
                        DateIngested = DateTime.Now
                    };
                }

                return true;
            }
            else
            {
                number = null;
                return false;
            }
        }

        /// <summary>
        /// Converts a letter in a string to numbers on a dialpad.
        /// </summary>
        /// <param name="letter"> A lowercase char. </param>
        /// <returns> The dialpad equivalent. </returns>
        public static char LetterToKeypadDigit(char letter)
        {
            // Map the chars to their keypad numerical values.
            return letter switch
            {
                '+' => '0',
                // The digit 1 isn't mapped to any chars on a phone keypad.
                'a' or 'b' or 'c' => '2',
                'd' or 'e' or 'f' => '3',
                'g' or 'h' or 'i' => '4',
                'j' or 'k' or 'l' => '5',
                'm' or 'n' or 'o' => '6',
                'p' or 'q' or 'r' or 's' => '7',
                't' or 'u' or 'v' => '8',
                'w' or 'x' or 'y' or 'z' => '9',
                // If the char isn't mapped to anything, respect it's existence by mapping it to a wildcard.
                _ => '*',
            };
        }

        public bool IsValid() => DialedNumber?.IsValidPhoneNumber() ?? false && AreaCode.ValidNPA(NPA) && AreaCode.ValidNXX(NXX) && AreaCode.ValidXXXX(XXXX);

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}