namespace Frontend.WebUI.Utility
{
	public class Constants
	{
		public static string CouponAPIBase { get; set; }
		public static string AuthAPIBase { get; set; }
        public static string ProductAPIBase { get; set; }
        public static string ShoppingCartAPIBase { get; set; }
        public static string OrderAPIBase { get; set; }
        public const string RoleAdmin = "ADMIN";
        public const string RoleUser = "USER";
        public const string TokenCookie = "JWTToken";
        public enum ApiType
		{
			GET,
			POST,
			PUT,
			DELETE
		}
	}
}
