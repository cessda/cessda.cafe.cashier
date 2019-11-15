# CESSDA Café: Cashier

[![Build Status](https://jenkins-dev.cessda.eu/buildStatus/icon?job=cessda.cafe.cashier%2Fmaster)](https://jenkins-dev.cessda.eu/job/cessda.cafe.cashier/job/master/)

This project is an implementation of the CESSDA Café Cashier specification using ASP.NET Core.

## Getting Started

To quickly start this application, navigate to the Cashier directory and enter the command dotnet run.

To quickly create a docker container, build and run the Dockerfile in the base directory. Do not run the Dockerfile in the Cashier directory as this Dockerfile is for CI purposes.

Once running the application will be exposed on port 5000.

## Configuring

By default, the cashier will autoconfigure a coffee machine at http://localhost:1337/ for testing purposes. To override this use the command line argument --Cafe:DefaultCoffeeMachine=http://example.com/ or set the environment variable CAFE__DEFAULTCOFFEEMACHINE to configure a different coffee machine.

## Prerequisites

The following tools need to be installed locally to build and test the application:

* [ASP.NET Core 2.2](https://dotnet.microsoft.com/apps/aspnet)
* A CESSDA Café compliant Coffee implementation

## Technology Stack

Several frameworks are used in this application:

| Framework/Technology									| Description													|
| ----------------------------------------------------- | ------------------------------------------------------------- |
| [.Net Core](https://dotnet.microsoft.com/)			| Runtime for applications using the C# programming language	|
| [ASP.NET](https://dotnet.microsoft.com/apps/aspnet)	| Framework for building web applications using .Net			|
| [NSwag](https://github.com/RicoSuter/NSwag)			| OpenAPI tool chain for ASP.NET								|

## Resources

[Issue Tracker](https://bitbucket.org/cessda/cessda.cafe.cashier/issues?status=new&status=open)

## Authors

**Matthew Morris (matthew DOT morris AT cessda DOT eu)** - *CESSDA Technical Officer*
