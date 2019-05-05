
# Are you flying a 737 MAX ?

I'm an [AvGeek](https://www.urbandictionary.com/define.php?term=avgeek), and as such, I was very curious about the recent news about the 737 MAX. Investigation is still underway but there's a good article from the New York times about it. [What You Need to Know After
Deadly Boeing 737 Max Crashes](https://www.nytimes.com/interactive/2019/business/boeing-737-crashes.html).

As of today, every Boeing 737 MAX is grounded, waiting for "the fix", so I thought I could create something to let people know if they're flying a 737 MAX or not. Why?, because I could, and because it would be a fan experiment. And that's it!.

So I created a new twiiter handle called [IsIt737MAX](https://twitter.com/IsIt737MAX), which, when you tweet a flight number to it, it will reply letting you know if you're flying a 737 MAX or not. 

Here's an example:

![](res/twit_reply.png?raw=true)

## Tech stuff

### How does it work?

The code here it's been deployed to an Azure Function, which is the one sending out the twits. But who is calling that function, how do I know who to reply to and what to reply.

It all starts with an Azure Logic app, it used to be a Microsoft Flow but I changed it to a Logic app. This is a very simple "app" with two connectors.

The first connector is for Twitter and it does is searching for the "IsIt737MAX" on Twitter. If it does find something, it'll call this function sending the body of the twit (for every twit found).

![](res/logic_app.png?raw=true)

After that, the function kicks in. This function will get the body of the twit and it'll try to parse it as a flight number. I could not find a known algorithm for that so I created one that has been working for many years now.

```C#
public static (string airline, string number) Parse(string flightNumber)
{
	string airline = "", number = "";
	flightNumber = flightNumber.Trim().Replace(" ", "");

	for (int i = 0; i < flightNumber.Length; i++)
	{
		char c = flightNumber[i];
		if (c > 64 && string.IsNullOrEmpty(number))
			airline += c;
		else
		{
			if (i <= 1)
				airline += c;
			else
				number += c;
		}
	}

	return (airline, number);
}
```

> For some unknow reason there's no way to show a gist here, so here it is https://gist.github.com/sebagomez/c7f10fdb66a71865da152686b82ade57

With that, airline and number, I can easuly get the aircraft type by crawling to a web page. I won't explain much about that process here, but it's in the source files, take a look at the [FlightAwareHelper.cs](src/IsItAMAX/Misc/FlightAwareHelper.cs) file.

I then parse that info trying to find the aircraft type and return (twit) the according string.

And that's it!