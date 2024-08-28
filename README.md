# MessageExchangeApp

Current application is not for commercial use and do not represent final development of complete app, it's just a sketch to the basis of application

In order to fully utilize this app - firstly you need docker up and running on your machine, then you have to locate the folder and write in a console "docker-compose up --build" to build the application.
Your application will be up and running in port 5001, so you have to just go to localhost:5001 to utilize this app.
Swagger is located at localhost:5001/swagger if you want to test the operations with database.
Logs are stored at "Logs" folder and everyday that you use it will create you a new file.
init.sql file is going to be automatically run to create a table for you to use.