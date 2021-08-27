Feature: ExchangeCurrency

Scenario Outline: Verify that two given valid currencies will result in an OK response code
	Given I do a "/rate" GET request
	When I have the following currencies "<currency1>" and "<currency2>"
	And I send a request to the API
	Then the response code should be 200 with response message "OK"
	Examples:
	| currency1 | currency2 |
	| NOK       | SEK       |
	| USD       | EUR       |
	| SEK       | USD       |

Scenario Outline: Verify that any combination of invalid currencies results in a Bad Request response code
	Given I do a "/rate" GET request
	When I have the following currencies "<currency1>" and "<currency2>"
	And I send a request to the API
	Then the response code should be <responseCode> with error message "<errorMessage>"
	Examples: 
	| currency1 | currency2 | responseCode | errorMessage	  |
	| AAA       | SEK       | 400          | Invalid currency |
	| NOR       | AAA       | 400          | Invalid currency |
	| AAA       | BBB       | 400          | Invalid currency |

Scenario Outline: To verify that inputs given have the same values in response
	Given I do a "/convert" GET request
	And I want to convert <amount> "<fromCurrency>" to "<toCurrency>"
	When I send a request to the API
	Then The given input values: <amount> "<fromCurrency>" "<toCurrency>" are the same in response
	Examples: 
	| amount | fromCurrency | toCurrency |
	| 1547   | USD          | NOK        |
	| 152070 | NOK          | EUR        |
	| 18900  | NOK          | SEK        |

Scenario Outline: To Verify that Given two currencies and amount converts successfully with data from fixer
	Given I do a "/convert" GET request
	And I want to convert <amount> "<fromCurrency>" to "<toCurrency>"
	When I send a request to the API
	Then Verify that the response after conversion is valid
	Examples: 
	| amount | fromCurrency | toCurrency |
	| 1547   | USD          | NOK        |
	| 152070 | NOK          | EUR        |
	| 18900  | NOK          | SEK        |