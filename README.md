# consoleApplication
A testing environment in C# to work with the Onspring API v2 SDK.

The console app when run will...
	
- get a random character from thebreakingbadapi.com
- check if this character exists in the target alpha instance
	- if the character exists the console app will retrieve all quotes by the character from thebreakingbadapi.com
		- if any of the quotes don't exist in Onspring the console app will add them.
	- if the character doesn't exist the console app will add this character in Onspring then will retrieve all quotes by the character from thebreakingbadapi.com
		- If any of the quotes don't exist in Onspring the console app will add them.
