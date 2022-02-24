# PhoneNumbersNA
A library for parsing phone numbers. Built around the [North American Numbering Plan](https://en.wikipedia.org/wiki/North_American_Numbering_Plan) and the [NANPA](https://nationalnanpa.com/) formal specification.

Find this package on [NuGet](https://www.nuget.org/packages/PhoneNumbersNA/)! 🚀

### Helping you Parse and Validate Phone Numbers ☎️ ###

The core of this library is the 
```csharp
var checkParse = PhoneNumber.TryParse(string input, out var phoneNumber);
```
function that accepts a string that you would like to parse into a single phone number. It mimics the format of ```int.TryParse(string input, out var value)``` to make it easy to use and reason about. The PhoneNumber type that is return in the out variable contains variety of useful properties including a 10 digit string named DialedNumber which is what you would literally dial on a phone's keypad to place a call to that number. The components of the DialedNumber are also provided as integers to make them easy to work with and store. Please note, to convert the NPA, NXX, and XXXX properties to strings you'll need to use the ```phoneNumber.NPA.ToString("000")``` method to preserve any leading zeros that aren't represented by the integer format.

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
  
Often phone number purchasing APIs make a distinction between Local phone numbers and Tollfree phone numbers. The PhoneNumber type contains a Type property that is accessible after you call the TryParse method on the string representation of a phone number. This Type property is an Enum with a value of Tollfree, NonGeographic, or Local depending on how the phone number was parsed. Using this information, you can chose the correct API endpoint to submit purchase order for that number to. If you prefer to figure out the Type of the number directly you can use the string extension methods 
```csharp
var checkNonGeographic = "9990221111".IsNonGeographic()
```
or 
```csharp
var checkTollFree = "9990221111".IsTollfree()
```
to get a Boolean as an answer.

This library is used in production by [Accelerate Networks](https://github.com/AccelerateNetworks/NumberSearch) and grew organically out of a large set of utility functions that have now been condensed into PhoneNumbersNA. 🥳

Update 2/22/2022: Discussion of updates and improvements version 1.0.4 in this [Twitter thread.](https://twitter.com/UncheckedError/status/1496217725559005186)

### Performance 🚅 ###
You can run the benchmarks for this library on your local machine by cloning this repo and then opening the solution file in Visual Studio 2022. Select the PhoneNumbersNA.Benchmark console app and then run it as a "Release" build. The benchmarks typically take about 4 minutes to run. Alternatively you can install the .NET SDK and use .NET CLI to build the project in release mode and run it.

Here are the benchmarks for the current version of PhoneNumbersNA:

![image](https://user-images.githubusercontent.com/11726956/155625946-5931aa98-b577-4bad-b5d5-0618cb9e1ac4.png)

### How to Contribute 🤝 ###
Please start by creating a new issue with a description of the problem and a method to reproduce it.
  
### How to run this project locally 🏃 ###
  * Clone the repo to your machine
  * dotnet 6.0 or greater is required (included in Visual Studio 2022)
  * Use [Visual Studio 2022 Community Edition](https://visualstudio.microsoft.com/vs/preview/) or VSCode with the dotnet CLI and CSharp language extension to edit, build, and run the test suite.
  * Double click the "PhoneNumbersNA.sln" file to open the solution in Visual Studio 2022.
