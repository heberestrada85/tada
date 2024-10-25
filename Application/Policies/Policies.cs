namespace Tada.Application.Policies
{
    /// <summary>
    /// Politicas para el uso de la applicacion a traves de la API
    /// </summary>
    public static class Policies
    {
        /// <summary>
        /// Nombre dela politica de uso del portal
        /// </summary>
        public const string AssociationsPolicy = "PortalManagmentPolicy";
    }

    public static class JwtClaims
    {
        public const string ApiAccess = "api_access";
    }
}
