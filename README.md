# ASP.NET MVC OpenID Connect Client

This web application is a demonstration of how to implement [OpenID Connect](http://openid.net) in ASP.NET MVC. It was built using [Visual Studio Community 2017](https://www.visualstudio.com/vs/community) with the "ASP.NET Web Application (.NET Framework) / MVC" template, following our [login.gov developer guide](https://developers.login.gov/openid-connect) using `private_key_jwt`. All of the authentication logic is in the [account controller](ASPNET%20OIDC/Controllers/AccountController.cs).

## Running the app

Open the solution in Visual Studio, click the run button with "IIS Express (Microsoft Edge)" (same as pressing `F5`), and a new browser window will open at `http://localhost:50764/`. Click "Log in »" on the home page to authenticate with the login.gov IdP in our integration environment.

## Contributing

See [CONTRIBUTING](CONTRIBUTING.md) for additional information.

## Public domain

This project is in the worldwide [public domain](LICENSE.md). As stated in [CONTRIBUTING](CONTRIBUTING.md):

> This project is in the public domain within the United States, and copyright and related rights in the work worldwide are waived through the [CC0 1.0 Universal public domain dedication](https://creativecommons.org/publicdomain/zero/1.0/).
>
> All contributions to this project will be released under the CC0 dedication. By submitting a pull request, you are agreeing to comply with this waiver of copyright interest.
