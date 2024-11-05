[![CodeQL](https://github.com/uncheckederror/PhoneNumbersNA/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/uncheckederror/PhoneNumbersNA/actions/workflows/codeql-analysis.yml)
[![.NET](https://github.com/uncheckederror/PhoneNumbersNA/actions/workflows/dotnet.yml/badge.svg)](https://github.com/uncheckederror/PhoneNumbersNA/actions/workflows/dotnet.yml)
# PhoneNumbersNA
A library for parsing phone numbers. Built around the [North American Numbering Plan](https://en.wikipedia.org/wiki/North_American_Numbering_Plan) and the [NANPA](https://nationalnanpa.com/) formal specification.

Find this package on [NuGet](https://www.nuget.org/packages/PhoneNumbersNA/)! 🚀

### Helping you Parse and Validate Phone Numbers ☎️ ###

The core of this library is the 
```csharp
var checkParse = PhoneNumber.TryParse(string input, out var phoneNumber);
```
function that accepts a string that you would like to parse into a single phone number. It mimics the format of ```int.TryParse(string input, out var value)``` to make it easy to use and reason about. The PhoneNumber type that is returned in the out variable contains a variety of useful properties including a 10 digit string named DialedNumber which is what you would literally dial on a phone's keypad to place a call to that number. The components of the DialedNumber are also provided as integers to make them easy to work with and cheap to store. Please note, to convert the NPA, NXX, and XXXX properties to strings you'll need to use the ```phoneNumber.NPA.ToString("000")``` method to preserve any leading zeros that aren't represented by the integer format. To make this senario less error pront we've added ```phoneNumber.GetNPAAsString()``` to handle this formatting issue for you. We've also exposed versions of all the methods that accept strings with alternate versions that accept ```ReadOnlySpan<char>``` to help you save some memory.

To parse a string that may contain many phone numbers use the extension method on the String class included in this library:
```csharp
var stringlyTypedPhoneNumbers = "12060009999 15030006969 18750001111".ExtractDialedNumbers();
```
which will return an array of 10 digit phone numbers as strings. If you prefer the strongly typed version you can use the 
```csharp
var stronglyTypedPhoneNumbers = "12060009999 15030006969 18750001111".ExtractPhoneNumbers();
```
extension method to get an ```IEnumerable<PhoneNumber>``` result.

Alternatively you can call the parsing methods directly using:
```csharp
var stringlyTypedPhoneNumbers = PhoneNumbersNA.Parse.AsDialedNumbers("12060009999 15030006969 18750001111");
var stronglyTypedPhoneNumbers = PhoneNumbersNA.Parse.AsPhoneNumbers("12060009999 15030006969 18750001111");
```
  
If you simply want a yes or no answer to whether a string is a valid NANP phone number you can use the 
```csharp
var checkValid = "12060991111".IsValidPhoneNumber();
```
to get a Boolean where a value of true means that the string is a valid phone number. There are also extension methods in a similar format for checking if a string is a valid NPA, NXX, XXXX and regular methods that are accessible by calling
```csharp
var checkValid = PhoneNumbersNA.AreaCode.ValidPhoneNumber("maybeAPhoneNumber");
``` 
or accept integers like 
```csharp
var checkValid = PhoneNumbersNA.AreaCode.ValidPhoneNumber(int npa, int nxx, int xxxx);
```
  
A common scenario when working with 3rd party VOIP API's like the [Teli API](https://apidocs.teleapi.net/api/) or the [Call48 API](https://apicontrol.call48.com/apidocs#did-did-lookup-get) is to query for blocks of available phone numbers by the NPA (Area Code) prefix for those phone numbers. This library provides a list of every active NANP Area Code as an array on integers that is accessible by calling ```PhoneNumbersNA.AreaCode.All```. If you want a list of only the active non-local, non-geographic area codes you can call ```PhoneNumbersNA.AreaCode.NonGeographic``` and similarly tollfree only Area Codes are available under ```PhoneNumbersNA.AreaCode.TollFree```. 
  
Some APIs require you to provide the name of the state the area code you are looking for existing within geographically. To that end you can use the ```PhoneNumbers.AreaCode.AreaCodesByState``` array to get a list of objects containing strings for both the short and long versions of state names and an array of all the area codes in that specific state.
  
Often phone number purchasing APIs make a distinction between Local phone numbers and Tollfree phone numbers. The PhoneNumber class contains a Type property that is accessible after you call the TryParse method on the string representation of a phone number. This Type property is an Enum with a value of Tollfree, NonGeographic, or Local depending on how the phone number was parsed. Using this information, you can chose the correct API endpoint to submit purchase order for that number to. If you prefer to figure out the Type of the number directly you can use the string extension methods 
```csharp
var checkNonGeographic = "9990221111".IsNonGeographic()
```
or 
```csharp
var checkTollFree = "9990221111".IsTollfree()
```
to get a Boolean as an answer.

This library is used in production by [Accelerate Networks](https://github.com/AccelerateNetworks/NumberSearch) and grew organically out of a large set of utility functions that have now been condensed into PhoneNumbersNA. 🥳

### Performance 🚅 ###
You can run the benchmarks for this library on your local machine by cloning this repo and then opening the solution file in Visual Studio 2022. Select the PhoneNumbersNA.Benchmark console app and then run it as a "Release" build. The benchmarks typically take about 3 minutes to run. Alternatively you can install the .NET SDK and use .NET CLI to build the project in release mode and run it.

Here are the benchmarks for the current version of PhoneNumbersNA:
![image](https://github.com/user-attachments/assets/62639a3d-3a43-4dd5-afe9-d45fad1ba66e)

This is quite an improvement over the .NET 7 version. Parsing both valid and invalid phone numbers is more than twice as fast, while consuming just 2/3rds the memory. In the large (887) and very large (8870) phone number benchmarks we've pushed allocations down from Gen1 to Gen0, reducing pressure on the garbage collector. Although total allocated bytes is about the same, we still see benifits from reducing GC pressure like 50% better performance in the 887 and 8870 AsPhoneNumbers benchmarks and reduced Error and StdDev values across all the benchmarks. These gains are thanks to aggressive use of the ```ref``` keyword for parameters and the conversion of ```public class PhoneNumber(string DialedNumber, ...)``` to ```public readonly record struct PhoneNumber(ref readonly string DialedNumber, ...)``` .

![image](https://user-images.githubusercontent.com/11726956/223918152-cf8df516-c69c-4cf8-b63e-c6bcc8cdb8ff.png)

This is quite an improvement over the .NET 6 version. Memory consumption is cut by 2/3rds in the 84, 887, and 8870 phone number benchmarks. For single numbers and in our 84 phone number benchmark performance is also improved by about 50%. These gains come mostly from the use of ```ReadOnlySpan<char>``` and ```CollectionsMarshall.AsSpan(List<char>)``` where ```string``` and ```new string(List<char>.ToArray())``` was previously used. These changes have prevented many unnecessary string allocations which is why memory consumption has been reduced so significantly. This library will never be allocation free as we're parsing and creating new strings, but we can get close.

![image](https://user-images.githubusercontent.com/11726956/155625946-5931aa98-b577-4bad-b5d5-0618cb9e1ac4.png)

### How to Contribute 🤝 ###
Please start by creating a new issue with a description of the problem and a method to reproduce it.
  
### How to run this project locally 🏃 ###
  * Clone the repo to your machine
  * .NET 8.0 or greater is required (included in Visual Studio 2022)
  * Use [Visual Studio 2022 Community Edition](https://visualstudio.microsoft.com/vs/preview/) or VSCode with the dotnet CLI and CSharp language extension to edit, build, and run the test suite.
  * Double click the "PhoneNumbersNA.sln" file to open the solution in Visual Studio 2022.
