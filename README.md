# SOAP-API Web Service Bronto Integration with C#


Bronto’s API was built on the SOAP web service. Here you will find how to properly integrate the API functions and objects to deliver extensible and testable solution in C#.
http://dev.bronto.com/category/api/soap/


### Prerequisites

Create Service Reference to the Bronto API using the bronto WSDL.
https://api.bronto.com/v4?wsdl

### Run

Step 1. Create Interface IBrontoApiWrapper.cs around the methods from the API that we need. 

Step 2. Implement the intereface from step 1, in BrontoApiWrapper.cs

Step 3. Create a service class BrontoEmailService.cs to expose the methods from step 2.

Step 4. In the service class use translator helper methods to map and convert the specific client objects to the corresponding API objects needed by the API (example contacts, messages, tokens, orderData etc)

Step 5. In the service class provide, logging and error handling around each API method call.

Step 6. Write Unit tests for the API methods.

Step 7. Use a client (web app, ajax, windows app etc) to call the service class methods that expose the API.




