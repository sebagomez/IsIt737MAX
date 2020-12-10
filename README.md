[![Build](https://github.com/sebagomez/IsIt737MAX/workflows/Build/badge.svg)](https://github.com/sebagomez/IsIt737MAX/actions)
[![Build & Deploy](https://github.com/sebagomez/IsIt737MAX/workflows/Build%20&%20Deploy/badge.svg)](https://github.com/sebagomez/IsIt737MAX/actions)

# Are you flying a 737 MAX ?

## TL;DR

Just twit your flight number (eg. AA984) to [@isit737MAX](https://twitter.com/isit737MAX) and a bot will reply with the actual aircraft type you're flying. [Try it here](https://twitter.com/intent/tweet?screen_name=isit737MAX&text=AA984)!

![](res/twit_reply.png?raw=true)

## Long(er) story and tech stuff

I'm an [avgeek](https://www.urbandictionary.com/define.php?term=avgeek), and as such, I was very curious about the recent news about the 737 MAX. Investigation is still underway but there's a good article from the New York times about it. [What You Need to Know After
Deadly Boeing 737 Max Crashes](https://www.nytimes.com/interactive/2019/business/boeing-737-crashes.html).

As of today, every Boeing 737 MAX is grounded, waiting for "the fix", so I thought I could create something to let people know if they're flying a 737 MAX or not. Why?, because I could, and because it would be a fun experiment. And that's it!.

So I created a new twiiter handle called [isit737MAX](https://twitter.com/isit737MAX), which, when you tweet a flight number to it, it will reply letting you know if you're flying a 737 MAX or not. 

#### How does it work?

The code here it's been deployed to an [Azure Function](https://docs.microsoft.com/en-us/azure/azure-functions/), which is the one sending out the twits. But who is calling that function, how do I know who to reply to and what to reply.

It all starts with an [Azure Logic app](https://docs.microsoft.com/en-us/azure/logic-apps/), it used to be a [Microsoft Flow](https://flow.microsoft.com) but I changed it to a Logic app. This is a very simple "app" with two connectors.

The first connector is for Twitter and all it does is searching for the "isit737MAX" string on Twitter. If it finds something, it'll call this function sending the body of the twit (for every twit found).

![](res/logic_app.png?raw=true)

After that, the function kicks in. This function will get the body of the twit and it'll try to parse its text as a flight number. I could not find a known algorithm for that so I created one that has been working for many years now.

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

With that, airline and number, I can easily get the aircraft type by crawling to a web page. I won't explain much about that process here, but it's in the source files, take a look at the [FlightDataHelper.cs](src/IsItAMAX/Misc/FlightDataHelper.cs) file.

I then parse that info trying to find the aircraft type and return (twit) the according string.

Give it a try! and let me know what you think.