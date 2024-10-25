# Tada Project

## Development environment
1. Docker desktop

### Running development environment
1. First step is build the project locally whit the command  `docker compose build`

### About Omninauth authentication for Azure Active Directory

The following environment variables will be required if Azure Active Directory sign in will be used:

1. `AZURE_APP_ID`
2. `AZURE_APP_SECRET`
3. `AZURE_SCOPES` with value `'openid profile email user.read'`
4. `ENABLE_OMNIAUTH_AZURE_AD_AUTHENTICATION` with a value of `true`

If you want to test it in your development environment, be sure to create an application, a secret, and ensure to set it up as a `Multitenant` application, then set the callback URL to `http://localhost:3000/users/auth/microsoft_graph_auth/callback`. Of course, for production, instead of `http://localhost:3000` use the appropriate `https` domain.



# Run Migrations on dotnet command

dotnet ef migrations add init --msbuildprojectextensionspath --startup-project ./Tada --verbose
