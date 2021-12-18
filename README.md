# consoleApplication
A testing environment in C# to work with the Onspring API v2 SDK.

The console app when run will...
	
- get a random character from [thebreakingbadapi.com](https://thebreakingbadapi.com/).
- check if this character exists in the target alpha instance
	- if the character exists the console app will retrieve all quotes by the character from [thebreakingbadapi.com](https://thebreakingbadapi.com/)
		- if any of the quotes don't exist in Onspring the console app will add them.
	- if the character doesn't exist the console app will add this character in Onspring then will retrieve all quotes by the character from thebreakingbadapi.com
		- If any of the quotes don't exist in Onspring the console app will add them.

Note you will need to have apps and fields configured in an Onspring instance to catch the data being pulled and then pushed from the breakingbadapi. The ids for these apps and fields will need to be set within app.config along with the proper api key.
