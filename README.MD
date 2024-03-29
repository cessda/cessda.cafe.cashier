# CESSDA Café: Cashier

[![Build Status](https://jenkins-dev.cessda.eu/buildStatus/icon?job=CESSDA.Cafe.Cashier%2Fmaster)](https://jenkins-dev.cessda.eu/job/CESSDA.Cafe.Cashier/job/master/)

This project is an implementation of the CESSDA Café Cashier specification using ASP.NET Core.

## Getting Started

To quickly start this application, navigate to the Cashier directory and enter the command dotnet run.

To quickly create a docker container, build and run the Dockerfile in the Cashier directory. Do not run the Dockerfiles in the CI directory as these Dockerfile are for setting up build environments in CI.

Once running the application will be exposed on port 5000.

## Configuring

By default, the cashier will autoconfigure a coffee machine at `http://localhost:1337/` for testing purposes. To override this use the command line argument `--Cafe:DefaultCoffeeMachine=http://example.com/` or set the environment variable `CAFE__DEFAULTCOFFEEMACHINE` to configure a different coffee machine. To configure multiple coffee machines set `CAFE__DEFAULTCOFFEEMACHINE__1`, `CAFE__DEFAULTCOFFEEMACHINE__2` to the coffee machines that should be configured.

## Prerequisites

The following tools need to be installed locally to build and test the application:

* [ASP.NET 6.0](https://dotnet.microsoft.com/apps/aspnet)
* A CESSDA Café compliant Coffee implementation

## Technology Stack

Several frameworks are used in this application:

| Framework/Technology                                  | Description                                                   |
| ----------------------------------------------------- | ------------------------------------------------------------- |
| [.NET](https://dotnet.microsoft.com/)		            | Runtime for applications using the C# programming language    |
| [ASP.NET](https://dotnet.microsoft.com/apps/aspnet)   | Framework for building web applications using .Net            |
| [NSwag](https://github.com/RicoSuter/NSwag)           | OpenAPI tool chain for ASP.NET                                |

## Resources

[Issue Tracker](https://bitbucket.org/cessda/CESSDA.Cafe.Cashier/issues?status=new&status=open)

## Authors

**Matthew Morris (matthew DOT morris AT cessda DOT eu)** - *CESSDA Technical Officer*
