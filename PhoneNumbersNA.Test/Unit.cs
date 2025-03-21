using nietras.SeparatedValues;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Xunit;
using Xunit.Abstractions;

using static System.Net.Mime.MediaTypeNames;

namespace PhoneNumbersNA.Test
{
    public class Unit(ITestOutputHelper output)
    {

        // https://nationalnanpa.com/contact_us/NANP_Country_Contacts.pdf
        readonly string[] NANPContacts =
        [
                "264 497-2651", "264-497-3651", "268- 468-4616", "242-393-0234",
                "242-393-0153", "246- 535-2502", "441-405-6000", "441-474-6048",
                "284-468-2183", "284-468-3090", "284-468-4165", "284-494- 6786",
                "819-934-6352","819-997-4610","613-702-0016","613-702-0017",
                "345-946-4282","345-945-8284","767-266-3294","767-448-0182",
                "767-440-0627","767-440-0835","829-732-5555","473-440-2271",
                "473-440-4122","473-435-6872","473-435-2132","758-458-1701",
                "876-968-6053","876-929-3635","664-491-2521","664-491-6659",
                "664-491-3789","869-467-2812","869-465-0198","869-466-6872",
                "869-466-6817","758-458-1701","721-542-4699","721-542-4817",
                "784-457-2279","784-457-2834","868-675-8288","868-674-1055",
                "649-946-1900","649-946-1119","202-418-1500","202-418-2825",
                "202-418-1525","202-418-1413","925-420-0340","571-363-3838",
        ];

        readonly string[] CenturyLinkShortCodes =
        [
                "67378", "58865", "275285", "30471"
        ];

        readonly string manyNumbers = "+1 206-858-9310\r\n2024561414\r\n(206)858-8757\r\nRandom Gibberish that should be stripped";

        readonly string[] badNumbers =
        [
                "5558675309", "0000000000", "1111111111", string.Empty, "Choose..."
        ];

        [Fact]
        public void ExtractNumbers()
        {
            var numbers = manyNumbers.ExtractDialedNumbers();
            Assert.NotEqual(0, numbers.Length);
            Assert.True(numbers.Length is 3);
            foreach (var number in numbers)
            {
                var checkParse = PhoneNumber.TryParse(number, out var phoneNumber);
                Assert.True(checkParse);
                Assert.True(phoneNumber.DialedNumber.IsValidPhoneNumber());
                Assert.True(phoneNumber.IsValid());
            }

            numbers = string.Join(", ", badNumbers).ExtractDialedNumbers();
            Assert.Equal(0, numbers.Length);

            numbers = AreaCode.ExtractDialedNumbers(manyNumbers.AsSpan());
            Assert.NotEqual(0, numbers.Length);
            Assert.True(numbers.Length is 3);
            foreach (var number in numbers)
            {
                var checkParse = PhoneNumber.TryParse(number, out var phoneNumber);
                Assert.True(checkParse);
                Assert.True(phoneNumber.DialedNumber.IsValidPhoneNumber());
                Assert.True(phoneNumber.IsValid());
            }

            numbers = AreaCode.ExtractDialedNumbers(string.Join(", ", badNumbers).AsSpan());
            Assert.Equal(0, numbers.Length);

            var phoneNumbers = manyNumbers.ExtractPhoneNumbers();
            Assert.NotEqual(0, phoneNumbers.Length);
            Assert.True(phoneNumbers.Length is 3);
            foreach (var number in phoneNumbers)
            {
                Assert.True(number.IsValid());
            }

            phoneNumbers = string.Join(", ", badNumbers).ExtractPhoneNumbers();
            Assert.Equal(0, phoneNumbers.Length);
        }

        [Fact]
        public void TryParseBadNumbers()
        {
            foreach (var number in badNumbers)
            {
                var checkParse = PhoneNumber.TryParse(number, out var phoneNumber);

                Assert.False(checkParse);
                Assert.True(phoneNumber.DialedNumber == string.Empty);
            }
        }

        [Fact]
        public void TryParseBenchmark()
        {
            var checkParse = PhoneNumber.TryParse("ppboinine", out var phoneNumber);

            Assert.False(checkParse);
            Assert.True(phoneNumber.DialedNumber == string.Empty);

            checkParse = PhoneNumber.TryParse("2sma", out phoneNumber);

            Assert.False(checkParse);
            Assert.True(phoneNumber.DialedNumber == string.Empty);

            checkParse = PhoneNumber.TryParse("1 (111) 111-1111", out phoneNumber);

            Assert.False(checkParse);
            Assert.True(phoneNumber.DialedNumber == string.Empty);

            checkParse = PhoneNumber.TryParse("15555551212", out phoneNumber);

            Assert.False(checkParse);
            Assert.True(phoneNumber.DialedNumber == string.Empty);

            checkParse = PhoneNumber.TryParse("12068589310", out phoneNumber);

            Assert.True(checkParse);
            Assert.True(phoneNumber.DialedNumber != string.Empty);

            // Short code
            checkParse = PhoneNumber.TryParse("FUNNY", out phoneNumber);

            Assert.True(checkParse);
            Assert.True(phoneNumber.DialedNumber != string.Empty);

            // Short code with incorrect 1 prefix.
            checkParse = PhoneNumber.TryParse("140404", out phoneNumber);

            Assert.True(checkParse);
            Assert.True(phoneNumber.DialedNumber != string.Empty);
        }

        [Fact]
        public void TryParseNANPAContactNumbers()
        {
            foreach (var number in NANPContacts)
            {
                var checkParse = PhoneNumber.TryParse(number, out PhoneNumber phoneNumber);
                Assert.True(checkParse);
                Assert.False(string.IsNullOrWhiteSpace(phoneNumber.DialedNumber));
            }
        }

        [Fact]
        public void ValidNumbers()
        {
            foreach (var number in NANPContacts)
            {
                Assert.True(number.Replace(" ", string.Empty).Replace("-", string.Empty).ToString().IsValidPhoneNumber());
                Assert.True(AreaCode.ValidPhoneNumber(number.Replace(" ", string.Empty).Replace("-", string.Empty)));
            }
        }

        [Fact]
        public void ShortCodes()
        {
            foreach (var number in CenturyLinkShortCodes)
            {
                Assert.False(number.IsValidPhoneNumber());
                Assert.False(AreaCode.ValidPhoneNumber(number));
                Assert.True(PhoneNumber.TryParse(number, out PhoneNumber parsedNumber));
                Assert.Equal(number, parsedNumber.DialedNumber);
                Assert.Equal(PhoneNumbersNA.NumberType.ShortCode, parsedNumber.Type);
            }
        }


        [Fact]
        public void InvalidNumbers()
        {
            foreach (var number in badNumbers)
            {
                Assert.False(number.IsValidPhoneNumber());
            }
        }

        [Fact]
        public void ValidNPAs()
        {
            Assert.NotEmpty(AreaCodes.All);
            foreach (var npa in AreaCodes.All)
            {
                var local = npa;
                Assert.True(npa.ToString().IsValidNPA());
                Assert.True(AreaCode.ValidNPA(ref local));
                Assert.True(AreaCode.ValidNPA(local.ToString()));
            }
        }

        [Fact]
        public void ValidTollfreeNPAs()
        {
            Assert.NotEmpty(AreaCodes.TollFree);
            foreach (var npa in AreaCodes.TollFree)
            {
                var local = npa;
                output.WriteLine($"{local}");
                Assert.True(npa.ToString().IsValidNPA());
                Assert.True(npa.ToString().IsTollfree());
                Assert.True(npa.ToString().IsNonGeographic());
                Assert.True(AreaCode.ValidNPA(ref local));
                Assert.True(AreaCode.ValidTollfree(ref local));
                Assert.True(AreaCode.ValidNonGeographic(ref local));
                Assert.True(AreaCode.ValidNPA(local.ToString()));
                Assert.True(AreaCode.ValidTollfree(local.ToString()));
                Assert.True(AreaCode.ValidNonGeographic(local.ToString()));
            }
        }

        [Fact]
        public void ValidNonGeographicNPAs()
        {
            Assert.NotEmpty(AreaCodes.NonGeographic);
            foreach (var npa in AreaCodes.NonGeographic)
            {
                var local = npa;
                Assert.True(npa.ToString().IsValidNPA());
                Assert.True(npa.ToString().IsNonGeographic());
                Assert.True(AreaCode.ValidNPA(ref local));
                Assert.True(AreaCode.ValidNonGeographic(ref local));
                Assert.True(AreaCode.ValidNPA(local.ToString()));
                Assert.True(AreaCode.ValidNonGeographic(local.ToString()));
            }
        }

        [Fact]
        public void ValidNXXs()
        {
            Assert.True(AreaCode.ValidNXX(201));
            Assert.True(AreaCode.ValidNXX(999));
            Assert.True(AreaCode.ValidNXX("201"));
            Assert.True(AreaCode.ValidNXX("999"));
            Assert.True("201".IsValidNXX());
            Assert.True("999".IsValidNXX());
        }

        [Fact]
        public void InvalidNXXs()
        {
            Assert.False(AreaCode.ValidNXX(199));
            Assert.False(AreaCode.ValidNXX(1000));
            Assert.False(AreaCode.ValidNXX("199"));
            Assert.False(AreaCode.ValidNXX("1000"));
            Assert.False("199".IsValidNXX());
            Assert.False("1000".IsValidNXX());
        }

        [Fact]
        public void ValidXXXXs()
        {
            Assert.True(AreaCode.ValidXXXX(0001));
            Assert.True(AreaCode.ValidXXXX(0010));
            Assert.True(AreaCode.ValidXXXX(0100));
            Assert.True(AreaCode.ValidXXXX(1000));
            Assert.True(AreaCode.ValidXXXX(9999));
            Assert.True(AreaCode.ValidXXXX("0001"));
            Assert.True(AreaCode.ValidXXXX("0010"));
            Assert.True(AreaCode.ValidXXXX("0100"));
            Assert.True(AreaCode.ValidXXXX("1000"));
            Assert.True(AreaCode.ValidXXXX("9999"));
            Assert.True("0001".IsValidXXXX());
            Assert.True("0010".IsValidXXXX());
            Assert.True("0100".IsValidXXXX());
            Assert.True("1000".IsValidXXXX());
            Assert.True("9999".IsValidXXXX());
        }

        [Fact]
        public void InvalidXXXXs()
        {
            Assert.False(AreaCode.ValidXXXX(-1));
            Assert.False(AreaCode.ValidXXXX(10000));
            Assert.False(AreaCode.ValidXXXX("-1"));
            Assert.False(AreaCode.ValidXXXX("10000"));
            Assert.False("-1".IsValidXXXX());
            Assert.False("10000".IsValidXXXX());
        }

        [Fact]
        public void UseCases()
        {
            var input = "1800KROGERS";
            var checkValidNumber = input.IsValidPhoneNumber();
            Assert.True(checkValidNumber);
            var checkParse = PhoneNumber.TryParse(input, out PhoneNumber phoneNumber);
            Assert.True(checkParse);
            Assert.True(phoneNumber.DialedNumber?.IsValidPhoneNumber() ?? false);
            Assert.True(AreaCode.ValidNPA(phoneNumber.NPA));
            Assert.True(AreaCode.ValidNXX(phoneNumber.NXX));
            Assert.True(AreaCode.ValidXXXX(phoneNumber.XXXX));
            Assert.True(AreaCode.ValidTollfree(phoneNumber.NPA));

        }

        [Fact]
        public void CodesByState()
        {
            Assert.NotEmpty(AreaCode.States);
            foreach (var state in AreaCode.States)
            {
                if (state.AreaCodes is not null && state.AreaCodes.Length != 0)
                {
                    foreach (var code in state.AreaCodes)
                    {
                        Assert.True(AreaCode.ValidNPA(code));
                        Assert.False(AreaCode.ValidNonGeographic(code));
                        Assert.False(AreaCode.ValidTollfree(code));
                    }
                }
            }
        }

        // Disabled because NANPA removed this report from their website.
        [Fact]
        public void VerifyGeographicCodesFromNANPA()
        {
            string path = "./Geographic.csv";

            string text = File.ReadAllText(path);

            using var reader = Sep.New(',').Reader().FromText(text);

            List<Geographic> geos = [];

            foreach (var row in reader)
            {
                Geographic geo = new(row["NPA"].Parse<int>(), row["Location"].ToString());

                if (geo.NPA > 200 && geo.NPA < 999)
                {
                    Assert.True(AreaCode.AllFlatLookup[geo.NPA], $"NPA {geo.NPA}, {geo.Location} is {AreaCode.AllFlatLookup[geo.NPA]} in the lookup.");
                }

                geos.Add(geo);
            }

            output.WriteLine($"{geos.Count} Geographic NPAs found in NANPA.");
            output.WriteLine($"{AreaCodes.All.Length} NPAs found in All PhoneNumberNA.");
        }

        public readonly record struct Geographic(int NPA, string Location);

        // Disabled because NANPA removed this report from their website.
        [Fact]
        public void VerifyNonGeographicCodesFromNANPA()
        {
            string path = "./NonGeographic.csv";

            string text = File.ReadAllText(path);

            using var reader = Sep.New(',').Reader().FromText(text);

            List<Geographic> geos = [];

            foreach (var row in reader)
            {
                Geographic geo = new(row["NPA"].Parse<int>(), row[1].ToString());

                if (geo.NPA > 499 && geo.NPA < 901)
                {
                    Assert.True(AreaCode.NonGeographicFlatLookup[geo.NPA], $"NPA {geo.NPA}, {geo.Location} is {AreaCode.NonGeographicFlatLookup[geo.NPA]} in the lookup.");
                }

                geos.Add(geo);
            }

            output.WriteLine($"{geos.Count} NonGeographic NPAs found in NANPA.");
            output.WriteLine($"{AreaCodes.NonGeographic.Length} NPAs found in NonGeographic PhoneNumberNA.");
            Assert.Equal(geos.Count, AreaCodes.NonGeographic.Length);
        }

        // Disabled because NANPA removed this report from their website.
        [Fact]
        public void VerifyTollfreeCodesFromNANPA()
        {
            string path = "./All.csv";

            string text = File.ReadAllText(path);

            using var reader = Sep.New(',').Reader().FromText(text);

            List<NPADetail> geos = [];

            foreach (var row in reader)
            {
                if (row["SERVICE"].ToString().Contains("Toll-Free") && row["IN_SERVICE"].ToString() is "Y")
                {
                    NPADetail geo = new(row["NPA_ID"].Parse<int>(), row["SERVICE"].ToString());

                    if (geo.NPA_ID > 799 && geo.NPA_ID < 889)
                    {
                        output.WriteLine(geo.NPA_ID.ToString());
                        Assert.True(AreaCode.TollFreeFlatLookup[geo.NPA_ID], $"NPA {geo.NPA_ID}, {geo.SERVICE} is {AreaCode.TollFreeFlatLookup[geo.NPA_ID]} not in the lookup.");
                    }

                    geos.Add(geo);
                }
            }

            output.WriteLine($"{geos.Count} Tollfree NPAs found in NANPA.");
            output.WriteLine($"{AreaCodes.TollFree.Length} NPAs found in Tollfree PhoneNumberNA.");
            Assert.Equal(geos.Count, AreaCodes.TollFree.Length);
        }

        public readonly record struct NPADetail(int NPA_ID, string SERVICE);

        // Disabled because NANPA removed this report from their website.
        [Fact]
        public void VerifyAllAreaCodesFromNANPA()
        {
            string path = "./All.csv";

            string text = File.ReadAllText(path);

            using var reader = Sep.New(',').Reader().FromText(text);

            List<NPADetail> geos = [];

            foreach (var row in reader)
            {
                if (row["IN_SERVICE"].ToString() is "Y")
                {
                    NPADetail geo = new(row["NPA_ID"].Parse<int>(), row["SERVICE"].ToString());

                    if (geo.NPA_ID > 499 && geo.NPA_ID < 999)
                    {
                        Assert.True(AreaCode.AllFlatLookup[geo.NPA_ID], $"NPA {geo.NPA_ID} is {AreaCode.AllFlatLookup[geo.NPA_ID]} not in the lookup.");
                    }

                    geos.Add(geo);
                }
            }

            foreach (var code in AreaCodes.All)
            {
                var match = geos.FirstOrDefault(x => x.NPA_ID == code);
                if (match.NPA_ID != code)
                {
                    output.WriteLine($"{code} Not Found");
                }
            }

            output.WriteLine($"{geos.Count} NPAs found in NANPA.");
            output.WriteLine($"{AreaCodes.All.Length} NPAs found in All PhoneNumberNA.");
            Assert.Equal(geos.Count, AreaCodes.All.Length);
        }

        // This doesn't work because the CNA website is written using VueJS.
        // The NPA's aren't rendered unless the JS in the initial response is executed, which doesn't happen because this is not a browser.
        [Fact]
        public void VerifyCanadianFromCNA()
        {
            string path = "./All.csv";

            string text = File.ReadAllText(path);

            using var reader = Sep.New(',').Reader().FromText(text);

            List<NPADetail> geos = [];

            foreach (var row in reader)
            {
                if (row["IN_SERVICE"].ToString() is "Y" && row["COUNTRY"].ToString() is "CANADA")
                {
                    NPADetail geo = new(row["NPA_ID"].Parse<int>(), row["SERVICE"].ToString());

                    if (geo.NPA_ID > 499 && geo.NPA_ID < 999)
                    {
                        Assert.True(AreaCode.CanadianFlatLookup[geo.NPA_ID], $"NPA {geo.NPA_ID} is {AreaCode.CanadianFlatLookup[geo.NPA_ID]} not in the lookup.");
                    }

                    geos.Add(geo);
                }
            }

            foreach (var code in AreaCodes.Canadian)
            {
                var match = geos.FirstOrDefault(x => x.NPA_ID == code);
                if (match.NPA_ID != code)
                {
                    output.WriteLine($"{code} Not Found");
                }
            }

            output.WriteLine($"{geos.Count} Canadian NPAs found in NANPA.");
            output.WriteLine($"{AreaCodes.Canadian.Length} NPAs found in Canadian PhoneNumberNA.");
            Assert.Equal(geos.Count, AreaCodes.Canadian.Length);
        }

        // This is also broken, perhaps we can use the All CSV?
        // https://www.nanpa.com/resources/area-code-map/territories
        [Fact]
        public async void VerifyCountryOrTerritoryFromNANPA()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            string? s = null;
            var content = await client.GetAsync("https://www.nanpa.com/resources/area-code-map/territories");
            using (var sr = new StreamReader(await content.Content.ReadAsStreamAsync(), Encoding.GetEncoding("iso-8859-1")))
            {
                s = sr.ReadToEnd();
            }
            var pattern = "(?:[>])(\\d\\d\\d)(?:[<])";
            var rgx = new Regex(pattern);

            output.WriteLine(rgx.Matches(s).Count.ToString());

            foreach (Match match in rgx.Matches(s).Cast<Match>())
            {
                var checkParse = int.TryParse(match.ValueSpan, out int npa);
                if (checkParse)
                {
                    if (npa > 200 && npa < 999)
                    {
                        Assert.True(AreaCode.CountryOrTerritoryFlatLookup[npa], $"NPA {npa} is {AreaCode.CountryOrTerritoryFlatLookup[npa]} in the lookup.");
                    }
                }
            }
        }
    }
}