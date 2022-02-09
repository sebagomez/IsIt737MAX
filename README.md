[![Build](https://github.com/sebagomez/IsIt737MAX/workflows/Build/badge.svg)](https://github.com/sebagomez/IsIt737MAX/actions)
[![Build & Deploy](https://github.com/sebagomez/IsIt737MAX/workflows/Build%20&%20Deploy/badge.svg)](https://github.com/sebagomez/IsIt737MAX/actions)
[![Terraform](https://github.com/sebagomez/IsIt737MAX/actions/workflows/terraform.yml/badge.svg?branch=master)](https://github.com/sebagomez/IsIt737MAX/actions/workflows/terraform.yml)

# Are you flying a 737 MAX ?

## TL;DR

Just twit your flight number (eg. AA984) to [@isit737MAX](https://twitter.com/isit737MAX) and a bot will reply with the actual aircraft type you're flying. [Try it here](https://twitter.com/intent/tweet?screen_name=isit737MAX&text=AA984)!

![](res/twit_reply.png?raw=true)


## Long(er) story and tech stuff

I'm an [avgeek](https://www.urbandictionary.com/define.php?term=avgeek), and as such, I was very curious about the recent news about the 737 MAX. Investigation is still underway but there's a good article from the New York times about it. [What You Need to Know After
Deadly Boeing 737 Max Crashes](https://www.nytimes.com/interactive/2019/business/boeing-737-crashes.html).

Every 737 MAX was grounded waiting for "the fix", so I thought I could create something to let people know if they're flying a 737 MAX or not. Why?, because I could, and because it would be a fun experiment. And that's it!.

So I created a new twiiter handle called [isit737MAX](https://twitter.com/isit737MAX), which, when you tweet a flight number to it, it will reply letting you know if you're flying a 737 MAX or not. 

#### How does it work?

The code here it's been deployed to an [Azure Function](https://docs.microsoft.com/en-us/azure/azure-functions/), which is the one sending out the twits. But who is calling that function, how do I know who to reply to and what to reply.


I used to implement that part with a [Microsoft Flow](https://flow.microsoft.com), I then moved it to an [Azure Logic app](https://docs.microsoft.com/en-us/azure/logic-apps/) and it even worked for a while with an [IFTTT applet](https://ifttt.com/explore). The problem with those integrations is that they were all pulling data from the twitter API asking for mentions. They obviously have a windows in which they work. The Azure Logic app pulled every 5 minutes, and the IFTTT applet once every hour. So I needed/wanted something faster.
That's how I discovered Twitter's [Account Activity API](https://developer.twitter.com/en/docs/twitter-api/enterprise/account-activity-api/overview) which allows you to setup a webhook from an account to your endpoint.

So right now I have a small app (not in this repo) that using the [TwitterLib](https://github.com/sebagomez/twitterlib) creates a webhook and a subscription for the [@IsIt737MAX](https://twitter.com/isit737MAX) twitter handle. The urls for that webhook is my Azure function, so now every time somebody mentions* @IsIt737MAX on a twit, the function get called. The function process the twit and send out a reply to the twit with the actual aircraft that's being used for the mentioned flight.

![](res/twitter-webhook.png?raw=true)

Give it a try, twit any flight number and let [@IsIt737MAX](https://twitter.com/intent/tweet?text=%40isit737MAX%20CM369) let you know if you're flying in a 737 MAX. 

And let [me](https://twitter.com/sebagomez) know what you think.